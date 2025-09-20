# IDP Upload Portal

ASP.NET Core web application for document upload and processing with Azure integration.

## Features

- **Category Selection**: Dynamic dropdown populated from Cosmos DB
- **Dynamic Form Generation**: Forms generated based on category metadata
- **File Upload**: Zip file upload with validation
- **Azure Integration**: Sends data to Azure Function for processing
- **Bootstrap UI**: Modern, responsive interface

## Prerequisites

- .NET 9.0 SDK
- Azure Cosmos DB account
- Azure Function App

## Configuration

1. Update `appsettings.json` with your actual values:

```json
{
  "CosmosDb": {
    "ConnectionString": "your-cosmos-db-connection-string",
    "DatabaseName": "IDP_LookupManagement",
    "CategoryContainer": "Category",
    "CategoryMetadataContainer": "CategoryMetadata"
  },
  "AzureFunction": {
    "BaseUrl": "your-azure-function-base-url",
    "Endpoint": "/api/Servicebus_handler",
    "FunctionKey": "your-azure-function-key"
  }
}
```

## Running the Application

1. Clone the repository
2. Configure your connection strings in `appsettings.json`
3. Run the application:
   ```bash
   dotnet run
   ```
4. Navigate to `http://localhost:5000` (or the displayed URL)

## Azure Function Integration

The application sends multipart form data to the Azure Function with:

- **File field**: Zip file with filename
- **Metadata field**: JSON data with category and form fields

Expected metadata format:

```json
{
  "category": "CategoryName",
  "patient_id": "P001",
  "patient_name": "Patient Name",
  "patient_age": "25",
  "hospital_name": "Hospital Name"
}
```

## Project Structure

- `Controllers/`: MVC controllers
- `Models/`: Data models
- `Services/`: Business logic services
- `Views/`: Razor views
- `wwwroot/`: Static files (CSS, JS, libraries)

## Dependencies

- MongoDB.Driver (Cosmos DB integration)
- Azure.Storage.Queues (Azure integration)
- Bootstrap 5 (UI framework)
- jQuery (client-side scripting)
