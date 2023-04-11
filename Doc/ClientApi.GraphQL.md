# GraphQL API principles and examples 

## Overview

The Contracting.Works Client API is based on CQRS (Command Query Resource Segregation), where read operations are powered by a GraphQL API. GraphQL is a structured query language allowing the client fine control over how much or how little data a request returns. The data model is treated as a graph (thus the name GraphQL), where entities can be queried and relations within the entity graph can be traversed easily.

### GraphQL basics

[GraphQL](https://graphql.org/) is a flexible data query language that allows you to define API call responses to match your use case and technical needs (and much more). If you are new to the technology, here are some great resources to get you up to speed:

- [Introduction to GraphQL (↗)](https://graphql.org/learn/)
- [How to GraphQL (↗)](https://www.howtographql.com/)
- [Guides and Best Practices (↗)](https://www.graphql.com/guides/)

## Services and endpoints
The same GraphQL library and functionality as is used here is in use on several other parts of Contracting.Works, such as services for retrieving translated user interface texts or user settings. In addition to the main GraphQL endpoint, all GraphQL interfaces in Contracting.Works contain several utilities listed below.

### GraphQL Banana Cake Pop

Banana Cake Pop is an interactive editor for testing out your GraphQL queries, accessible through your web browser.

Using Banana Cake Pop is straight forward (just try it!). The query editor supports code completion based on the current model. The model is also available in the *Schema Reference*(this tab shows *Type View*, for each type defined) and *Schema Definition*(full schema definition) tabs. These tabs list all operations available in the API. The editor allows you to quickly familiarize yourself with the API, perform example operations, and send your first queries.

[The editor is available here](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/)

*_Note: in Contracting.Works, all GraphQL queries must specify the current client (tenant) and also provide a valid bearer token with access to the client._*

- The GraphQL schema endpoint path (available in Connection Settings - gear button located in the upper right corner) needs to be of the following format (replace "a-anonymisert" with your clientId): https://contracting-extest-clientapi-graphql.azurewebsites.net/client/b-dummydata/graphql
- Under Authorization Tab next to General, the following must be specified: Type (choose Bearer) and Token (valid bearer token)

### Swagger API
While the GraphqQl API is not REST based, all Contracting.Works services also contain a Swagger API. This is useful for operations such as checking permissions, checking service health and getting a valid bearer token for interactive testing purposes (see [Getting a valid bearer token](#getting-a-valid-bearer-token)).

[The Swagger UI is located here](https://contracting-extest-clientapi-graphql.azurewebsites.net/swagger/index.html)

#### Getting a valid bearer token

A valid bearer token can be fetched by accessing the site's [Swagger UI](https://contracting-extest-clientapi-graphql.azurewebsites.net/swagger/index.html), and performing a login for your user (please remember to tick the "Scopes" checkbox).

Click "Try it out" on a command which require authenticated access, for example AuthInfo. You do not need to provide a valid clientId here - you are not interested in the response, but rather the Curl-query itself. This will contain a valid bearer token value. 

Copy the token text (after "Bearer " and until the closing quote).

## The GraphQL language
Contracting.Works uses a custom GraphQL implementation, focusing on query performance and ease of use. Compared to some other implementations, only a subset of the operations are supported, and some useful extensions are added. Most notably:

- Mutations are not supported (use the [ClientApi REST API](ClientApi.md) for this).
- Subscriptions are not supported (use the [SignalR API](SignalR.md) for this).
- Full ["Edges and nodes"](https://www.apollographql.com/blog/explaining-graphql-connections-c48b7c3d6976/) semantics is not supported. Instead, a simplified version is supported (and indeed required), wrapping returned items in an "items" node. The purpose of this node is to give simple support for total counts on lists of items when fetching paged data. See [Query structure](#query-structure).
- A custom filter expression is supported on all sets of items, see [Filter expressions](#filter-expressions).
- Pagination is supported, see [Pagination](#pagination).
- Sorting is supported, see [Sorting the results](#pagination).
- Performing multiple simultaneous queries is supported, see [Multiple queries](#multiple-queries).

Also note that we are utilizing [HotChocolates GraphQL query parser](https://chillicream.com/docs/hotchocolate/v10/advanced/). This gives us support for most standard GraphQl structures in principle - but we have focused on implementing the behaviors used by our front-end and current integrations. The following are currently not supported:
- Aliases
- Fragments


### Query structure

Queries have the following structure:

```javascript
query {
  customers {
    items {
      name
    }
  }
}
```
[Try it in Banana Cake Pop](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/ui/?query=query%20%7B%0A%20%20customers%20%7B%0A%20%20%20%20items%20%7B%0A%20%20%20%20%20%20name%0A%20%20%20%20%7D%0A%20%20%7D%0A%7D%0A)


Note that all words are **case sensitive** here, including entity and property names. For sets of entities, the "items" node is mandatory. Any root query in Contracting.Works is a set of items - even if only a single item is returned.

To drill down further in the data set following relations, query the relation as part of the root entity as follows:

```javascript
query {
  customers {
    items {
      name, contacts {items {name}}
    }
  }
}
```

For all sets of items, the parameters "take", "skip", "filter" and "orderBy" are supported. This is expressed as follows:
```javascript
query {
  customers (filter: "name='OB Kristiansen'"){
    items {
      name, contacts {items {name}}
    }
  }
}
```

The same construct is supported on the levels below, for example:
```javascript
query {
  customers (filter: "name='OB Kristiansen'"){
    items {
      name, contacts(take:1) {items {name}}
    }
  }
}
```

The "items" node serves a single purpose: to provide a place for a total item count when using [pagination](#pagination). The total count can be retrieved as follows:

```javascript
query {
  customers (filter: "name='OB Kristiansen'"){
    totalCount,
    items {
      name, contacts(take:1) {items {name}}
    }
  }
}
```

Please be aware that getting the total count has a (small) cost on the underlying database query. Therefore, it should be avoided if not strictly needed. If all records are returned (unpaginated data), the total count will be the number of returned records.

Queries may take variables as input. This is a useful technique for improving readability and reusability of queries. Below is a parametrized query, using $filter0 and $take0 as variables. Note that the query must specify the types of the input variables.
To use the variables, reference them with the full name including the leading dollar sign (which is mandatory).

```javascript
query($filter0: String, $take0: int) {
  customers(take: $take0, filter: $filter0) {
    items {
      customerId, contacts {items {name}}
    }
  }
}
```

To set the parameter values on a query in Banana Cake Pop, a JSON structure must be used under "Variables" at the bottom of the screen:

```javascript
{"take0": 10, "filter0":"customerId!='D2AFDDC5-88A0-5C0C-AF24-001220D51881'"}
```

GraphQL Fragments are currently not supported, but will likely be so in the future.


### Pagination

All sets of data may be paginated using "take" and "skip" - behaving similarly to the same operations in .Net LINQ. ***For large data sets, pagination should always be used.*** Example:

```javascript
query {
  customers (take: 10){
    totalCount,
    items {
      name, assignments {items {assignmentNumber}}
    }
  }
}
```

The same type of pagination can be applied to any returned set of data, for example: 

```javascript
query {
  customers (take: 10){
    totalCount,
    items {
      name, assignments(skip:2, take:3) {totalCount, items {assignmentNumber}}
    }
  }
}
```

### Paths
A *path* originate on the currently filtered object, and may represent any field or direct relations. In general, any field ending with "Id" except for the Id of the current entity itself indicate that path navigation is possible if dropping the "Id"-part. For example, an assignment has a field named "addressId", that indicates a single address. This address can be navigated in a path expression. Example:

```javascript
query {
  assignments(take:10, filter: "address.address1 != null" ) {
    items {
      address {
        address1
      }
    }
  }
}
```

Paths are used both when filtering, as in the example above, and when sorting data.


### Sorting the results

For sorting returned data, the "orderBy" parameter can be used. Note that orderBy requires an input object containing a path and optionally a direction. 

In its simplest form:

```javascript
query {
  countries(orderBy: { path: "name" }) {
    items {
      countryId
      name
    }
  }
}
```

Reversing direction:

```javascript
query {
  countries(orderBy: { path: "name", descending: true }) {
    items {
      countryId
      name
    }
  }
}
```

Ordering on multiple levels:

```javascript
query {
  customers (take: 10, orderBy: {path: "name"}){
    totalCount,
    items {
      name, assignments(orderBy:{path:"assignmentNumber"}) {totalCount, items {assignmentNumber}}
    }
  }
}
```

You can also follow more complex [path](#paths):
```javascript
query {
  assignments(
    take: 10, 
    filter: "address.address1 != null"
    orderBy: { path: "address.address1" }
  ) {
    items {
      address {
        address1
      }
    }
  }
}
```


  - [First five products sorted by code in ascending order]
  - [First five products sorted by code in descending order]
  - [Workorders sorted by order date in ascending order, with order date greater than that specified]


### Filter expressions

Like [pagination](#pagination) and [sorting](#sorting-the-results), filtering may be applied to any set of returned data.

In its simplest form, filter expressions filter on column values:
```javascript
query {
  countries(filter:"countryId=NO") {
    items {
      countryId
      name
    }
  }
}
```

More complex comparisons and normal boolean logic can be used for filters. Normal operator precedence is used for NOT(!), AND(&), OR(|) and handling of parenthesis. Thus a filter expression of the form "a=1|a=2&b=3" is functionally equivalent to the expression  "a=1|(a=2&b=3)".
Boolean operators and the equality operator also support C-style ==, && and ||, though they behave identically to their single-character equivalent. Spaces are also allowed. The previous example may therefore be written "a == 1 || (a == 2 && b == 3)" for readability.
Any level of nesting is supported for parenthesis.

Boolean values always originate with a comparison expression between a [path](#paths) and a value, in that order. The value cannot currently be fetched from a [path](#paths). 

The following comparison expressions are supported:
| Comparison         | Short form | Comment                                                      |
| ------------------ | ---------- | ------------------------------------------------------------ |
| Equal              | =, ==      |                                                              |
| NotEqual           | !=         |                                                              |
| GreaterThan        | >          |                                                              |
| GreaterThanOrEqual | >=         |                                                              |
| LessThan           | <          |                                                              |
| LessThanOrEqual    | <=         |                                                              |
| Like               |            |                                                              |
| Contains           |            | Equivalent to Like                                           |
| NotContains        |            | Equivalent to !Contains ...                                  |
| StartsWith         |            |                                                              |
| EndsWith           |            |                                                              |
| In                 |            | Operates on a set of values separated by comma and surrounded by square brackets, for example [1,2,3] |
| NotIn              |            | Equivalent to !In ...                                        |


Based on field types, some comparison operations may not be supported, such as Contains on a DateTime.

Null values are handled "C#-style" for fields which are nullable, that is a null value is considered a legal value for any comparison.

**Note: Comparison names are case sensitive.** 


  - [Workorders for an order date]
  - [Details and line items for a sales invoice]
  - [Workorder details for customers in specific locations]



### Multiple queries

When fetching multiple small datasets, this can be efficiently achieved with a single request simply by adding multiple top-level nodes within the same query.

**Note: consider using a local cache for stable data rather than reading them from the GraphQL API multiple times.** It is not often the list of countries change, after all.

```javascript
query {
      countries(filter:"countryId=NO") {
    items {
      countryId
      name
    }
  }
  assignments(
    take: 2, 
    filter: "address.address1 != null"
    orderBy: { path: "address.address1" }
  ) {
    items {
      address {
        address1
      }
    }
  }
}
```

## Authentication

Any client using the API needs to provide following access tokens:

- The `Authorization` header, specifically, `Authorization: Bearer MY_TOKEN`.
- The `access_token` URL query parameter.

The token must have an access to the client environment you are targeting. For example, if you create an access token that only has access to the particular client, you cannot use that token to access another client's data.

To learn more about authentication in Contracting.Works and how to create your own access tokens take a look to the [Authentication reference documentation](Devinco.Connect.md).



#### HTTP Methods

This is the query used in the example below:

```javascript
query ($filter0: String) {
    employees(filter: $filter0) {
        items { employeeId, lastName, firstName, dateOfBirth }
    }
}    
query variables: {"filter0": "sys_Deactivated = false"}
HTTP headers:    {"Authorization" :"Bearer XXX"}

```

#### GET

The HTTPS GET method requires that the query is included in the URL string as a parameter. You can also send any required variables in an additional "variables" parameter in JSON format.

```json
curl "https://contracting-extest-clientapi-graphql.azurewebsites.net/client/XXX/graphql" ^
  -H "Connection: keep-alive" ^
  -H "accept: */*" ^
  -H "Authorization: Bearer XXX" ^
  -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36" ^
  -H "content-type: application/json" ^
  -H "Origin: https://contracting-extest-clientapi-graphql.azurewebsites.net" ^
  -H "Sec-Fetch-Site: same-origin" ^
  -H "Sec-Fetch-Mode: cors" ^
  -H "Sec-Fetch-Dest: empty" ^
  -H "Referer: https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/playground/" ^
  -H "Accept-Language: nb-NO,nb;q=0.9,no;q=0.8,nn;q=0.7,en-US;q=0.6,en;q=0.5" ^
  --data-binary "^{^\^"operationName^\^":null,^\^"variables^\^":^{^\^"filter0^\^":^\^"sys_Deactivated = false^\^"^},^\^"query^\^":^\^"query (^$filter0: String) ^{^\^\n  employees(filter: ^$filter0) ^{^\^\n    items ^{^\^\n      employeeId^\^\n      lastName^\^\n      firstName^\^\n      dateOfBirth^\^\n    ^}^\^\n  ^}^\^\n^}^\^\n^\^"^}" ^
  --compressed
```



## Query the GraphQL Demo Endpoint Directly

Yes, you can query our GraphQL demo endpoint direct from the command line, or your own App!

The demo endpoint is unauthenticated, and although we have imposed read-only access, with a maximum return of 20 results per query, you can quickly demonstrate a working integration and/or proof of concept.

- [Using cURL]
- [Using Powershell]
- [TypeScript]
- [JavaScript]

### Using cURL

A simple example, which demonstrates how you can query our GraphQL demo endpoint direct from the command line:

```json
curl "https://contracting-extest-clientapi-graphql.azurewebsites.net/client/d4a668d1-d5fa-4aff-91f2-a9615281efa7/graphql" ^
  -H "Connection: keep-alive" ^
  -H "accept: */*" ^
  -H "Authorization: Bearer XXX" ^
  -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36" ^
  -H "content-type: application/json" ^
  -H "Origin: https://contracting-extest-clientapi-graphql.azurewebsites.net" ^
  -H "Sec-Fetch-Site: same-origin" ^
  -H "Sec-Fetch-Mode: cors" ^
  -H "Sec-Fetch-Dest: empty" ^
  -H "Referer: https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/playground/" ^
  -H "Accept-Language: nb-NO,nb;q=0.9,no;q=0.8,nn;q=0.7,en-US;q=0.6,en;q=0.5" ^
  --data-binary "^{^\^"operationName^\^":null,^\^"variables^\^":^{^\^"filter0^\^":^\^"sys_Deactivated = false^\^"^},^\^"query^\^":^\^"query (^$filter0: String) ^{^\^\n  employees(filter: ^$filter0) ^{^\^\n    items ^{^\^\n      employeeId^\^\n      lastName^\^\n      firstName^\^\n      dateOfBirth^\^\n    ^}^\^\n  ^}^\^\n^}^\^\n^\^"^}" ^
  --compressed
```

### Using PowerShell
```powershell
Invoke-WebRequest -Uri "https://contracting-extest-clientapi-graphql.azurewebsites.net/client/d4a668d1-d5fa-4aff-91f2-a9615281efa7/graphql" `
-Method "POST" `
-Headers @{
"accept"="*/*"
  "Authorization"="Bearer XXX"
  "User-Agent"="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36"
  "Origin"="https://contracting-extest-clientapi-graphql.azurewebsites.net"
  "Sec-Fetch-Site"="same-origin"
  "Sec-Fetch-Mode"="cors"
  "Sec-Fetch-Dest"="empty"
  "Referer"="https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/playground/"
  "Accept-Encoding"="gzip, deflate, br"
  "Accept-Language"="nb-NO,nb;q=0.9,no;q=0.8,nn;q=0.7,en-US;q=0.6,en;q=0.5"
} `
-ContentType "application/json" `
-Body "{`"operationName`":null,`"variables`":{`"filter0`":`"sys_Deactivated = false`"},`"query`":`"query (`$filter0: String) {\n  employees(filter: `$filter0) {\n    items {\n      employeeId\n      lastName\n      firstName\n      dateOfBirth\n    }\n  }\n}\n`"}"
```

### TypeScript

Our development language of choice; a typed superset of JavaScript that compiles to plain JavaScript. Here’s an example of how you can use it to query our GraphQL demo endpoint:

```typescript
         ---nee to be updated
     function callTestEndpoint():void {
        var request = new XMLHttpRequest();

        request.open('POST', "https://contracting-extest-clientapi-graphql.azurewebsites.net/client/d4a668d1-d5fa-4aff-91f2-a9615281efa7/graphql", true);
        var data = {"query":"{sage {emSales { salesOrder (first: 5, orderBy: \"{ id: 1 }\") {  edges { node { id, soldToCustomer {companyName } } } } } } }","variable":""};

        request.setRequestHeader("content-type", "application/json");
        request.send(JSON.stringify(data));
        request.onreadystatechange = function () {

            if (request.readyState == 4 && (request.status == 200 || request.status == 0)) {
                // If no error
                var result = JSON.parse(request.responseText);
            } else if (request.readyState == 4) {
                // If error occurred
            }
        };
    }
```

### JavaScript

A slight difference to our TypeScript example (see above), but we did it anyway.

```javascript
        ---nee to be updated
    function callTestEndpoint() {
        var request = new XMLHttpRequest();

        request.open('POST', "https://contracting-extest-clientapi-graphql.azurewebsites.net/client/d4a668d1-d5fa-4aff-91f2-a9615281efa7/graphql", true);
        var data = {"query":"{sage {emSales { salesOrder (first: 5, orderBy: \"{ id: 1 }\") {  edges { node { id, soldToCustomer {companyName } } } } } } }","variable":""};

        request.setRequestHeader("content-type", "application/json");
        request.send(JSON.stringify(data));
        request.onreadystatechange = function () {

            if (request.readyState == 4 && (request.status == 200 || request.status == 0)) {
                // If no error
                var result = JSON.parse(request.responseText);
            } else if (request.readyState == 4) {
                // If error occurred
            }
        };
    }
```





## Error types and handling

The list can be found in [Reference.md](Reference.md) description
