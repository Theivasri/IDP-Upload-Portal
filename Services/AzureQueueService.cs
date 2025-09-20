using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IDPUpload_Portal.Services
{
    public class AzureFunctionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _functionUrl;

        public AzureFunctionService(HttpClient httpClient, IConfiguration configuration)
        {
            var baseUrl = configuration["AzureFunction:BaseUrl"];
            var endpoint = configuration["AzureFunction:ProcessUploadEndpoint"];
            var functionKey = configuration["AzureFunction:FunctionKey"];

            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException("AzureFunction:BaseUrl or ProcessUploadEndpoint not configured");

            // ✅ Construct full function URL with function key
            _functionUrl = $"{baseUrl.TrimEnd('/')}{endpoint}?code={functionKey}";
            _httpClient = httpClient;
        }

        public async Task<string> SendToFunctionAsync(IFormFile zipFile, Dictionary<string, string> metadata)
        {
            // Ensure we have a valid metadata dictionary
            metadata ??= new Dictionary<string, string>();

            // Create the multipart form data content with a specific boundary
            var boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            using var content = new MultipartFormDataContent(boundary);
            
            // Create metadata JSON object
            var metadataObj = new Dictionary<string, string>
            {
                ["patient_id"] = metadata.GetValueOrDefault("patient_id"),
                ["patient_name"] = metadata.GetValueOrDefault("patient_name"),
                ["patient_age"] = metadata.GetValueOrDefault("patient_age"),
                ["hospital_name"] = metadata.GetValueOrDefault("hospital_name"),
                ["category"] = metadata.GetValueOrDefault("category")
            };
            
            var metadataJson = JsonSerializer.Serialize(metadataObj);
            var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
            metadataContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "\"metadata\""
            };
            content.Add(metadataContent);

            // Add file
            if (zipFile == null || zipFile.Length == 0)
            {
                throw new ArgumentException("No file was provided for upload.");
            }

            // Read file into memory
            using var memoryStream = new MemoryStream();
            await zipFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Add file content with the exact field name "file"
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = $"\"{zipFile.FileName}\""
            };
            content.Add(fileContent);

            // 🔎 DEBUG: Dump multipart request
            Console.WriteLine("==== Multipart Form Data Preview ====");
            foreach (var part in content)
            {
                var name = part.Headers.ContentDisposition?.Name?.Trim('"');
                var fileName = part.Headers.ContentDisposition?.FileName?.Trim('"');
                var contentType = part.Headers.ContentType?.ToString();

                Console.WriteLine($"Part Name: {name}");
                if (!string.IsNullOrEmpty(fileName))
                    Console.WriteLine($"FileName: {fileName}");
                if (!string.IsNullOrEmpty(contentType))
                    Console.WriteLine($"Content-Type: {contentType}");

                if (part is StringContent)
                {
                    var str = await part.ReadAsStringAsync();
                    Console.WriteLine($"Content (string): {str}");
                }
                else
                {
                    var bytes = await part.ReadAsByteArrayAsync();
                    Console.WriteLine($"Content (bytes): {bytes.Length} bytes");
                }

                Console.WriteLine("-----------------------------------");
            }
            Console.WriteLine("===================================");
            Console.WriteLine($"Sending to Azure Function URL: {_functionUrl}");

            try
            {
                var response = await _httpClient.PostAsync(_functionUrl, content);

                Console.WriteLine($"Response status: {(int)response.StatusCode} {response.StatusCode}");
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Azure Function response body: {responseBody}");

               return responseBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while calling Azure Function: {ex.Message}");
                 return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }
    }
}
