using IDPUpload_Portal.Models;
using IDPUpload_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IDPUpload_Portal.Controllers
{
    public class UploadController : Controller
    {
        private readonly CosmosMongoService _cosmosMongoService;
        private readonly AzureFunctionService _azureFunctionService;

        public UploadController(CosmosMongoService cosmosMongoService, AzureFunctionService azureFunctionService)
        {
            _cosmosMongoService = cosmosMongoService;
            _azureFunctionService = azureFunctionService;
        }

        // GET: UploadForm
        public async Task<IActionResult> UploadForm(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
                return RedirectToAction("Index", "Home");

            // ✅ Fetch metadata by CategoryID (string, e.g., "C123")
            var metadataList = await _cosmosMongoService.GetMetadataByCategoryIdAsync(categoryId);

            // ✅ Pass values to view
            ViewBag.CategoryId = categoryId;

            // Fetch category name (for title)
            var category = await _cosmosMongoService.GetCategoryByIdAsync(categoryId);
            ViewBag.CategoryName = category?.CategoryName ?? "Unknown Category";

            return View(metadataList);
        }

        // POST: Submit
        [HttpPost]
        [RequestSizeLimit(100_000_000)] // 100MB file size limit
        public async Task<IActionResult> Submit(IFormFile file, string categoryId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { error = "Please upload a valid zip file." });
                }

                // Log file details
                Console.WriteLine($"File received: {file.FileName}, Size: {file.Length} bytes");

                // Get form data
                var form = await Request.ReadFormAsync();
                var fieldData = new Dictionary<string, string>();

                // Process form fields that start with 'fieldData'
                foreach (var key in form.Keys)
                {
                    if (key.StartsWith("fieldData") && !string.IsNullOrEmpty(form[key]))
                    {
                        // Map fields to match Azure Function's expected names
                        var cleanKey = key.ToLower() switch
                        {
                            var k when k.Contains("patientid") => "patient_id",
                            var k when k.Contains("patientname") => "patient_name",
                            var k when k.Contains("age") => "patient_age",
                            var k when k.Contains("hospital") => "hospital_name",
                            _ => key
                                .Replace("fieldData[", "")
                                .Replace("]", "")
                                .Trim()
                                .ToLower()
                                .Replace(" ", "_")
                                .Replace("-", "_")
                        };

                        fieldData[cleanKey] = form[key];
                    }
                }

                Console.WriteLine($"Received {fieldData.Count} metadata fields");
                foreach (var item in fieldData)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }

                // Fetch metadata (for validation/logging)
                var metadataList = await _cosmosMongoService.GetMetadataByCategoryIdAsync(categoryId);
                Console.WriteLine($"Metadata count from DB: {metadataList.Count}");

                // Add category name to metadata
                var category = await _cosmosMongoService.GetCategoryByIdAsync(categoryId);
                fieldData["category"] = category?.CategoryName ?? categoryId;

                Console.WriteLine("Normalized metadata: " + JsonSerializer.Serialize(fieldData));

                // Send to Azure Function
                Console.WriteLine("Sending request to Azure Function...");
                var success = await _azureFunctionService.SendToFunctionAsync(file, fieldData);
                Console.WriteLine($"Response received from Azure Function: {success}");

                return Content(success, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Submit action: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
