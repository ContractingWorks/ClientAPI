# REST API principles and examples

Contracting Works use a REST API, documented through [OpenAPI 3.0](https://contracting-extest-clientapi.azurewebsites.net/swagger/v1/swagger.json) and available for interactive testing through a [Swagger page.](https://contracting-extest-clientapi.azurewebsites.net/swagger/index.html)

The API is used for altering data only. To query for data, please refer to the [GraphQl API](ClientApi.GraphQL.md)

Note that while the API uses REST style abstractions, it is *not* RESTful - that is entity paths are not expressed thrugh the API URLs. Instead, entity identifiers is provided as part of the API method payloads.

## Basic principles 

All methods provided by the API follow the same principles, described here. In brief:
  - As part of the method URL, the client id (tenant id) must be provided. Without this, all method calls will fail. See [URL structure](#url_structure).
  - A valid bearer token must be set as a header on the request.
  - We only provide POST methods, taking a JSON payload as input.
  - Two primary methods are provided for each [aggregate root](DataModel.md#aggregate-definitions): *Upsert* and *UpsertMultiple*.  
  - The API will automatically perform bulk operations for storing data. For performance, provide lists of items rather than sending multiple requests.
  - The API will automatically perform merge operations where appropriate. For performance, do not send data that do not need to be changed.
  - The API will automatically send change notification events to integrated systems on data changes. This behavior can be suppressed through request parameters.
  - The API is stateless and runs on multiple nodes.

# Notes - please disregard anything below
TODO

## Using Compression

The REST API allows the use of compression on the request and the response, using the standards defined by the HTTP 1.1 specification. Compression is automatically supported by some clients, and can be manually added to others. Visit [Salesforce](https://developer.salesforce.com/page/Web_Services_API) for more information on particular clients.

To use compression, include the HTTP header Accept-Encoding: gzip or Accept-Encoding: deflate in a request. The REST API compresses the response if the client properly specifies this header. The response includes the header Content-Encoding: gzip or Accept-Encoding: deflate. You can also compress any request by including a Content-Encoding: gzip or Content-Encoding: deflate header.

### Response Compression

The REST API can optionally compress responses. Responses are compressed only if the client sends an Accept-Encoding header. The REST API is not required to compress the response even if you have specified Accept-Encoding, but it normally does. If the REST API compresses the response, it also specifies a Content-Encoding header.

### Request Compression

Clients can also compress requests. The REST API decompresses any requests before processing. The client must send a Content-Encoding HTTP header in the request with the name of the appropriate compression algorithm. For more information, see:

- Content-Encoding at: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11)
- Accept-Encoding at: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.3](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.3)
- Content Codings at: [www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.5](http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.5)

## Authorization Through Connected Apps and OAuth 2.0

For a client application to access REST API resources, it must be authorized as a safe visitor. To implement this authorization, use a connected app and an OAuth 2.0 authorization flow.

### Configure a Connected App

A connected app requests access to REST API resources on behalf of the client application. For a connected app to request access, it must be integrated with your org’s REST API using the OAuth 2.0 protocol. OAuth 2.0 is an open protocol that authorizes secure data sharing between applications through the exchange of tokens.

For instructions to configure a connected app, see the Devico.Connect Authorization section. 

### Apply an OAuth Authorization Flow

OAuth authorization flows grant a client app restricted access to REST API resources on a resource server. Each OAuth flow offers a different process for approving access to a client app, but in general the flows consist of three main steps.

- To initiate an authorization flow, a connected app, on behalf of a client app, requests access to a REST API resource.
- In response, an authorizing server grants access tokens to the connected app.
- A resource server validates these access tokens and approves access to the protected REST API resource.

After reviewing and selecting an OAuth authorization flow, apply it to your connected app. For details about each supported flow, see  the Devico.Connect Authorization section. 



### Special request and response headers

- `X-AUSERNAME` – response header that contains either username of the authenticated user or 'anonymous'.
- `X-XXX-Token` – methods that accept multipart/form-data will only process requests with `X-XXX-Token: no-check` header.

### Error responses

Most resources will return a response body in addition to the status code. Usually, the JSON schema of the entity returned is the following:

Copy

```
1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 ``{    "id": "https://XXXX/REST/schema/error-collection#",    "title": "Error Collection",    "type": "object",    "properties": {        "errorMessages": {            "type": "array",            "items": {                "type": "string"            }        },        "errors": {            "type": "object",            "patternProperties": {                ".+": {                    "type": "string"                }            },            "additionalProperties": false        },        "status": {            "type": "integer"        }    },    "additionalProperties": false }
```

#### 

## Examples

This guide contains a range of examples, including examples of requests for creating issues, updating issues, searching for issues, and more.

We've also provided a simple example below to get you started. The example shows you how to create an issue using the Jira REST API. The sample code uses [curl](https://curl.haxx.se/) to make requests, but you can use any tool you prefer.

Note:

- The input file is denoted by the `--data @filename` syntax. The data is shown separately, and uses the JSON format.
- Make sure the content type in the request is set to `application/json`, as shown in the example.
- POST the JSON to your  server. In the example, the server is `http://localhost:8080/XXX/rest/api/2/issue/`.
- The example uses basic authentication with admin/admin credentials.
- You'll need to add a project to the instance before running and get the project ID of the project to which you want to add the issue beforehand.

To create an issue using the Jira REST API, follow these steps:

1. Create the data file that contains the POST data. For this example, we'll assume the file is named `data.txt`.

2. Add the following JSON to the file:

   Copy

   `1 2 3 4 5 6 7 8 9 10 11 12 13 ``{    "fields": {       "project":       {          "id": "10000"       },       "summary": "No REST for the Wicked.",       "description": "Creating of an issue using ids for projects and issue types using the REST API",       "issuetype": {          "id": "3"       }   } }`

   In this data, the project ID is 10000 and the issue type in our case is 3, which represents a task. You should pick an ID of a project in your instance and whichever issue type you prefer.

   Note that instead of the `id` you can also use the key and name for the `project` and `issuetype` respectively. For example,`"key": "TEST"` for the project and `"name": "Task"` for the `issuetype`.

3. In Terminal window, run the following command:

   Copy

   `1 ``curl -u admin:admin -X POST --data @data.txt -H "Content-Type: application/json" http://localhost:8080/XXX/rest/api/2/issue/`

   As before, adjust details for your environment, such as the hostname or port of the Jira instance. Note that a cloud instance or most public instances would require the use of HTTPS and, of course, valid credentials for the instance.

4. When your issue is created, check the response that will look something like this: 

   Copy

   `1 2 3 4 5 ``{   "id":"10009",   "key":"TEST-10",    "self":"http://localhost:8080/XXX/rest/api/2/issue/10009" } `

   That's it! You can use the issue ID, issue key, and the URL to the issue for additional requests, if you wish.

To get an issue you just created, use `http://localhost:8080/XXX/rest/api/2/issue/{issueIdOrKey}` endpoint:

Copy

```
1 ``curl -u admin:admin http://localhost:8080/XXX/rest/api/2/issue/TEST-10 | python -mjson.tool
```

We use `python -mjson.tool` to pretty print json.nk to Swagger / OpenAPI 

## Externally available test environments 

## Dealing with authorization & client data access  

