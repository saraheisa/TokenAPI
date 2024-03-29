# BLP Token Supply API

## Overview:
The BLP Token Supply API is a simple ASP.NET Web API designed to operate with the supply of the BLP (BullPerks) token on the Binance Smart Chain (BSC). It provides endpoints for retrieving token data, calculating token supplies, and JWT-based authentication for accessing restricted endpoints.

## Demo
[Demo link](https://master--helpful-sunburst-4fbe90.netlify.app)

## Features:
1. **JWT Authentication**: The API supports JWT-based authentication for secure access to restricted endpoints.
2. **Token Data Endpoint**: An endpoint is provided to retrieve stored token data, including the name, total supply, and circulating supply of the BLP token.
3. **Supply Calculation Endpoint**: A POST endpoint is available for authenticated users to calculate the total and circulating supply of the BLP token based on predefined non-circulating addresses.
4. **Entity Framework Core**: Data Access Layer is implemented using Entity Framework Core with a Code First approach, storing token data in a database.
5. **Swagger Documentation**: Swagger is integrated to provide interactive API documentation, making it easy to explore and test the available endpoints.

## How to Use:

### 1. Authentication:
- To obtain a JWT token, send a POST request to `/api/token/login` with any username/password combination. The API will return a bearer token that can be used to access authenticated endpoints.

### 2. Get Token Data:
- Send a GET request to `/api/token` to retrieve stored token data. This endpoint is publicly accessible and does not require authentication.

### 3. Calculate Token Data:
- Send a POST request to `/api/token` with a valid JWT token in the Authorization header to calculate the total and circulating supply of the BLP token. The calculated data will be stored in the database.

### 4. Swagger Documentation:
- Access Swagger documentation by navigating to `/swagger` in your web browser. Here you can explore and test all available endpoints with ease.

## Prerequisites:
- .NET 5 SDK or later
- MySQL Database

## Setup Instructions:
1. Clone the repository to your local machine.
2. Configure the MySQL connection string in `appsettings.json`.
3. Run the database migrations to create the required tables: `dotnet ef database update`.
4. Build the project using the command: `dotnet build`.
5. Run the project using the command: `dotnet run`.
6. Access the API endpoints using a tool like Postman or a web browser.

## BscScan API Usage:
- The BscScan API is utilized to retrieve token supply information. Ensure that the BscScan API key is configured in the `appsettings.json` file under the `BscScanApiKey` key.
