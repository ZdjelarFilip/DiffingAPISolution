# Diffing API

This is a simple API for comparing two base64-encoded binary data inputs. It provides endpoints to store left and right inputs and returns a diff result.

## Features

- **Upload Data**: Accepts base64-encoded binary data for left and right values.
- **Compare Data**: Returns whether they are:
  - `Equals` (identical)
  - `SizeDoNotMatch` (different sizes)
  - `ContentDoNotMatch` (same size but different content, with offset & length of differences)
- **Thread-Safe Storage**: Uses `ConcurrentDictionary` for handling concurrent requests.
- **Unit & Integration Tested**: Ensures correctness through automated tests.

## API Endpoints

All of the endpoints are available on:
**http://localhost:5279/swagger**

![Swagger](https://i.imgur.com/iCOZr2V.png)

### 1. Upload Left Data
```
PUT /v1/diff/{id}/left
Content-Type: application/json

{
  "data": "BASE64_ENCODED_STRING"
}
```
_Response:_
```
201 Created
```

### 2. Upload Right Data
```
PUT /v1/diff/{id}/right
Content-Type: application/json

{
  "data": "BASE64_ENCODED_STRING"
}
```
_Response:_
```
201 Created
```

### 3. Get Diff Result
```
GET /v1/diff/{id}
```
_Response Examples:_
```json
// If data is identical
{ "diffResultType": "Equals" }

// If sizes are different
{ "diffResultType": "SizeDoNotMatch" }

// If content differs
{
  "diffResultType": "ContentDoNotMatch",
  "diffs": [
    { "offset": 1, "length": 2 },
    { "offset": 5, "length": 1 }
  ]
}
```

## Running the Project

1. **Clone the repository**
   ```sh
   git clone https://github.com/ZdjelarFilip/DiffingAPISolution
   cd DiffingAPI
   ```

2. **Build & Run the API**
   ```sh
   dotnet build
   dotnet run
   ```

3. **Run Tests**
   ```sh
   dotnet test
   ```

## Assumptions & Notes
- The API assumes the input data is always valid base64.
- It does **not** perform deep validation on input beyond base64 decoding.
- The diffing logic finds **offsets and lengths of mismatches**, but not exact values.

## Tech Stack
- **C#** (.NET 9)
- **ASP.NET Core** for API
- **xUnit** for testing

