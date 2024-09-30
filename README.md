# Log and Blob Storage Service with Azure Functions

## Introduction
This project demonstrates the usage of Azure Table Storage and Blob Storage with .NET Core 6 and Azure Functions. The solution includes a logging service (`LogService`) that stores logs in Azure Table Storage, and a blob storage service (`BlobService`) that stores payloads in Azure Blob Storage.

## Running the Application

1. **Run the Azure Functions project**
    ```bash
    func start
    ```

2. **Test the application** by calling the HTTP endpoints. Example HTTP requests:

   - **GET Logs:**
     ```
     GET http://localhost:7071/api/logs?from=2023-09-01T00:00:00Z&to=2023-09-30T23:59:59Z
     ```

   - **GET Blob Payload:**
     ```
     GET http://localhost:7071/api/payload/{logId}
     ```

## Azure Storage Emulator

If you're using the Azure Storage Emulator (Azurite), ensure it's running before starting the application.
