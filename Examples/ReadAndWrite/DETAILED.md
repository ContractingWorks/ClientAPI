## Strawberry Shake

We use a tool called [Strawberry Shake](https://chillicream.com/docs/strawberryshake/) to generate the GraphQL client.

GraphQL is similar to SQL in that it allows you to select only specific fields from a large dataset, minimizing the data sent over the network. It also enables traversing relationships and applying filters, making the returned data highly customizable.

[Strawberry Shake](https://chillicream.com/docs/strawberryshake/) considers both the query and the schema during code generation, resulting in an easy-to-use C# client.

### Workflow

1. Create the desired query in the [GraphQL playground](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/).
2. Add or update the query in the [GQLQueries.graphql](GQLQueries.graphql) file.
3. Generate the GraphQL client code using the terminal:
```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the GraphQL dotnet tool (only mandatory first time)
dotnet tool restore

# Generate the GraphQL client code
dotnet graphql generate
```
4. Consume the new query in C#.

### Creating a query

1. Navigate to the [GraphQL playground](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/).
2. Create a new document and provide the GraphQL schema URL. For example: `https://contracting-extest-clientapi-graphql.azurewebsites.net/client/001-demo-pog/graphql`
3. Craft your query. For instance, a supplier query could look like this:
```graphql
query GetSupplier($top: Int!, $filter: String!) {
  suppliers(
    top: $top
    filter: $filter
    orderBy: { path: "sys_RowVersion", descending: false }
  ) {
    items {
      supplierId
      supplierName
      supplierNumber
    }
  }
}
```
4. Add the query on the bottom of the [GQLQueries.graphql](GQLQueries.graphql) file.
5. Generate the GraphQL client code using the terminal:
```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the GraphQL dotnet tool (only mandatory first time)
dotnet tool restore

# Generate the GraphQL client code
dotnet graphql generate
```
6. Add some C# code to get 10 suppliers and print them to the `MainApp.Run` method

```csharp
// Give us the first 10 suppliers that are not deactivated (soft deleted)
var supplierResponse = await _graphQl.GetSupplier.ExecuteAsync(
    top:10,
    filter:"sys_Deactivated = false");

supplierResponse.EnsureNoErrors();

var suppliers = supplierResponse?.Data?.Suppliers?.Items;
if (suppliers is null)
{
    // No suppliers found
    return;
}

foreach (var supplier in suppliers)
{
    _logger.LogInformation($"Supplier - {supplier.SupplierNumber} - {supplier.SupplierName}");
}
```

### Updating a query

To update a query, copy the query from [GQLQueries.graphql](GQLQueries.graphql) to the [GraphQL playground](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/). Modify the query and copy it back to [GQLQueries.graphql](GQLQueries.graphql), then regenerate the client code using the same terminal commands.

```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the GraphQL dotnet tool (only mandatory first time)
dotnet tool restore

# Generate the GraphQL client code
dotnet graphql generate
```

### Regenerating GraphQL Client Code

To regenerate the GraphQL client code:

```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the GraphQL dotnet tool (only mandatory first time)
dotnet tool restore

# Generate the GraphQL client code
dotnet graphql generate
```

### Downloading the Latest Contracting Works GraphQL Schema

The GraphQL schema periodically changes. To access the latest updates, download the newest schema and regenerate the code:

```bash
# Assuming you are in the root of the ReadAndWrite example folder

# Restore the GraphQL dotnet tool (only mandatory first time)
dotnet tool restore

# Download the latest GraphQL schema from Contracting Works
dotnet graphql update

# Generate the GraphQL client code
dotnet graphql generate
```
### Installing the GraphQL Tool

If you're starting a new repository and plan to use the GraphQL tool, you need to install it. Here's how:

```bash
# Create tool manifest in your repository
dotnet new tool-manifest

# Install Strawberry Shake tools in your repository (or later version)
dotnet tool install StrawberryShake.Tools --version 13.4.0
```

While you can install the GraphQL tool globally, installing it in the repository simplifies build pipelines.

### Filters

The Contracting Works GraphQL API offers considerable flexibility in filtering, resulting in a diverse array of filtering possibilities. While there isn't a definitive list of examples, here are some illustrative scenarios:

#### Paging through rows

To navigate through rows, we often employ the `sys_RowVersion` field to select rows between two distinct versions. When advancing to the next page, the filter's upper and lower limits are adjusted accordingly:

```
sys_RowVersion > 1000  & sys_RowVersion <= 2000
```

#### Filtering on child objects

Filtering based on child objects is fully supported. In this context, we can examine child objects like `customer` and `externalSystemAddresses` in the filter:

```
sys_Historic = false & sys_Incomplete = false & customer.sys_Historic = false & customer.customerId != null & externalSystemAddresses.externalSystemId = 6 & externalSystemAddresses.externalId != null
```

#### Various Comparison Approaches

When it comes to comparing values in our GraphQL filters, there are several methods at your disposal. Typically, the filter structure follows this pattern: `x.y.z = 1`. This structure comprises a path to the value, a comparison function, and the value for comparison.

We have incorporated common comparison operators, including: `=, !=, <, <=, >, >=`.

Furthermore, we've introduced named comparison functions that exhibit similar behavior to their SQL counterparts: `Contains, NotContains, StartsWith, EndsWith, Like, In, NotIn`.

When you're comparing a value against `null` or `undefined`, it implies that there's no match. The inclusion of `undefined` has been a recent addition, aimed at enhancing the ease of using GraphQL filtering from JavaScript. This adjustment accommodates the likelihood of expressions inadvertently resulting in `undefined`.

#### Complete filtering syntax

For those who find it beneficial, here is the EBNF syntax that outlines the complete filtering structure:

```ebnf
<expression>    ::= <term> { <or> <term> }
<term>          ::= <factor> {<and> <factor>}
<factor>        ::= <predicate> | <not><factor> | (<expression>)
<or>            ::='|'
<and>           ::='&'
<not>           ::='!'
<predicate>     ::= <path> <operator> <value> | path <function> <functionParam>
<path>          ::= identifier{.path}
<functionParam> ::= <value> | <valueList>
<value>         ::= 'stringvalue' | nonstringvalue
<valueList>     ::= [<value>{, <value>, ...}]
```

Supported comparison operators: `=, !=, <, <=, >, >=`

Supported functions: `Contains, NotContains, StartsWith, EndsWith, Like, In, NotIn`

## NSwag

We use [NSwag](https://github.com/RicoSuter/NSwag) to consume the Contracting Works REST API. Provided here is an example of how it can be used.

### Getting NSwag

Download Zip file it from [latest NSWag release](https://github.com/RicoSuter/NSwag/releases). Unzip it to a place of your choice.

### Generating REST API Client

Using the terminal:
```bash
# Assuming you are in the root of the ReadAndWrite example folder
cd WriteApi

# Generate the REST Client from the OpenAPI specification, takes a few seconds
<UNZIP_LOCATION_OF_NSWAG>\Net70\dotnet-nswag run
```

### Configuring NSwag code generation

The code generation is controlled by the file [nswag.json](WriteApi/nswag.json). This file contains a lot of settings but most important is `namespace` setting which control in what namespace the generated code ends up in.

### Extending NSwag code generation

In order to support Contracting Works Authentication the client class `ContractingWorksClient` has been extended through a partial class. In addition, it's important to make sure that null values are not serialized as most of the times when the C# client code sets a null value it means leave this value as is.

This can be seen in the source code for [`ContractingWorksClient`(Ext)](WriteApi/ContractingWorksExt.cs).

```csharp
// We need to extend the NSwag client so that we can set the Authorization header
partial class ContractingWorksClient
{
    public required string CwToken { get; init; }
    public required ILogger Logger { get; init; }

    // Inject the Authorization header when the request is prepared
    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        request.Headers.Authorization = new ("Bearer", CwToken);

#if DEBUG
        var content = request.Content;
        if (content is not null)
        {
            // Bit messy code to dump the JSON doc to the log
            //  in DEBUG.
            //  This is just to let you see the JSON doc
            //  we send to the REST Api.
            var bs = content.ReadAsByteArrayAsync().Result;
            var json = UTF8Encoding.UTF8.GetString(bs);
            Logger.LogInformation("Request:{json}", json);
        }
#endif
    }

    partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings)
    {
        // It's important to not include null values in JSON doc to the
        //  REST Api. A null value means that the REST API will try to
        //  set the value in the table to null.
        //  A null value typically means "leave the value as is".
        //  `JsonIgnoreCondition.WhenWritingNull` means we don't include
        //  null values in the JSON which will leave the value as is
        settings.DefaultIgnoreCondition  = JsonIgnoreCondition.WhenWritingNull  ;
        // The REST API assumes camelCasing which is the idiom for JavaScript
        settings.PropertyNamingPolicy    = JsonNamingPolicy.CamelCase           ;
#if DEBUG
        settings.WriteIndented           = true                                 ;
#else
        settings.WriteIndented           = false                                ;
#endif
    }

}
```

### Tips and tricks

When working with the generated REST client code, it's important to note that it can become quite extensive, sometimes reaching around 90,000 lines of code. This substantial size may affect compilation times, particularly in larger projects. One strategy to mitigate this impact is to place the REST client code in a separate project. This approach ensures that the project containing the REST client code is recompiled only when changes are made specifically to that code, which tends to be less frequent.

In this example, we've chosen to keep all the code within a single project to simplify the learning process and reduce the complexity of dealing with multiple projects and concepts. Depending on your project's requirements and priorities, you may decide to adopt either approach for managing the generated code effectively.