# How to Use - Millennium Falcon Challenge

## Quick Start with Docker (Recommended)

The easiest way for external users to run this project:

### Prerequisites
- Docker and Docker Compose installed
- No other setup required!

### Run the Application
```bash
# Clone the repository
git clone <your-repo-url>
cd millenium-falcon-challenge

# Start all services
docker-compose up -d

# Access the application
# Frontend: http://localhost:4200
# Engine API: http://localhost:5000  
# Onboard Computer API: http://localhost:5001
```

### Stop the Application
```bash
docker-compose down
```

## Manual Setup (Alternative)

If you prefer to run without Docker:

### Prerequisites
- Node.js 18+ (for frontend)
- .NET 8 SDK (for backend services)

### 1. Start Onboard Computer (Backend)
specify the file's path in the argument
```bash
cd onboard_computer
dotnet run mypath/my_millennium_falcon_file.json
# Runs on http://localhost:5001
```

### 2. Start Engine (Middle Service)
```bash
cd engine/src
dotnet run  
# Runs on http://localhost:5000
```

### 3. Start C3PO (Frontend)
```bash
cd c3po
npm install
npm start
# Runs on http://localhost:4200
```