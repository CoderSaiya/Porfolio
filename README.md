# Portfolio Project

A full-stack portfolio application built with React, .NET API, and SQL Server.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Database Setup](#database-setup)
- [Backend Setup](#backend-setup)
- [Frontend Setup](#frontend-setup)
- [Running with ngrok (Optional)](#running-with-ngrok)

## Prerequisites

Before you begin, ensure you have the following installed:
- SQL Server 2019+
- .NET 7.0 SDK or later
- Node.js 16.0 or later
- npm or yarn
- ngrok (optional, for tunnel access)

## Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Execute the SQL script from `portfolioDB.sql`:
```sql
USE master
GO

-- Run the script content from portfolioDB.sql
```

4. Update the connection string in `BE/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PortfolioDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

## Backend Setup

1. Navigate to the backend directory:
```bash
cd BE
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the API:
```bash
dotnet run
```

The API will start on `https://localhost:5001` and `http://localhost:5000`

## Frontend Setup

1. Navigate to the frontend directory:
```bash
cd FE
```

2. Install dependencies:
```bash
npm install
# or
yarn install
```

3. Update the API URL in `src/config.ts` or `.env`:
```javascript
// config.ts
export const API_URL = 'https://localhost:5001';

// or .env
VITE_BASE_URL=https://localhost:7000
```

4. Start the development server:
```bash
npm run dev
# or
yarn dev
```

The frontend will be available at `http://localhost:5173`

## Running with ngrok

If you want to expose your backend API to the internet:

1. Install ngrok if you haven't already:
```bash
npm install -g ngrok
# or download from https://ngrok.com/download
```

2. Start your backend API first (follow Backend Setup steps)

3. Start ngrok tunnel:
```bash
ngrok http https://localhost:5001
```

4. Update your frontend API URL to use the ngrok URL:
```javascript
// config.js or .env
export const API_URL = 'https://your-ngrok-url.ngrok.io/api';
```

5. Update CORS settings in `BE/Program.cs` (Optional):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
```

## Additional Configuration

### Environment Variables
Create a `.env` file in the frontend directory:
```bash
VITE_API_URL=https://localhost:5001
```

### Production Build

For frontend:
```bash
cd FE
npm run build
# or
yarn build
```

For backend:
```bash
cd BE
dotnet publish -c Release
```

## Troubleshooting

1. If you encounter SSL certificate errors:
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

2. If database connection fails:
- Verify SQL Server is running
- Check connection string
- Ensure Windows Authentication is enabled

3. If CORS errors occur:
- Verify API URL in frontend config
- Check CORS policy in backend
- Ensure ngrok URL is properly configured if using ngrok

## License

This project is licensed under the MIT License - see the LICENSE file for details