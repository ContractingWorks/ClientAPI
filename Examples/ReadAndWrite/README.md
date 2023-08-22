# ReadAndWrite

A read and write example for interacting with the Contracting Works API using C#.

## Overview

Contracting Works provides a [GraphQL endpoint](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/) endpoint for reading data and a [REST endpoint](https://contracting-extest-clientapi.azurewebsites.net/swagger/index.html) for writing data. This example demonstrates how to interact with these endpoints using C#.

Based on our firsthand experience, we recommend using [NSwag](https://github.com/RicoSuter/NSwag) to generate a C# client for the REST API and [Strawberry Shake](https://chillicream.com/docs/strawberryshake/) for the GraphQL API.

## Building the example

Open and build in visual studio or run the following in the terminal:

```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the graph QL tools
dotnet tool restore

# Build the example
#   Should build without errors
dotnet build
```

### Secrets

Before running the example, you need to set up the necessary secrets to connect to your Contracting Works client.

#### Using Visual Studio

1. Right-click the C# project and select `Manage User Secrets`.
2. Add the following information:

```json
{
  "contractingWorks": {
    "clientId"          : "<YOUR_CLIENT_ID>",
    "subjectId"         : "<YOUR_USER_SUBJECT_ID>",
    "apiKey"            : "<YOUR_USER_API_KEY>",
  }
}
```

#### Using Command Line

```bash
# Setup the secrets
dotnet user-secrets set "contractingWorks:clientId" "<YOUR_CLIENT_ID>"
dotnet user-secrets set "contractingWorks:subjectId" "<YOUR_USER_SUBJECT_ID>"
dotnet user-secrets set "contractingWorks:apiKey" "<YOUR_USER_API_KEY>"

# List the secrets (optional)
dotnet user-secrets list
```

## Running the example

You can run the example from Visual Studio or the terminal using the following command:

```bash
dotnet run
```

With some luck, the sample program will:

1. Authenticate successfully using Devinco Connect.
2. Read 10 Payment Terms and 10 Customers from Contracting Works using the GraphQL API.
3. Update Payment Terms of the 10 Customers received using the REST API.

If you want to learn more details on how to update your queries, work with [Strawberry Shake](https://chillicream.com/docs/strawberryshake/) and [NSwag](https://github.com/RicoSuter/NSwag) see [DETAILED.md](DETAILED.md).

