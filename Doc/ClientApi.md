
# Document status
This file is currently NOT representative for the REST API, and contains mainly examples for consideration in the documentation. Please disregard it.



# REST API principles and examples


## Basic principles 

REST (Representational State Transfer) is an architecture style that is designed for communications between different interfaces in the simplest way. As the REST API uses simple HTTP calls, it is much more accessible from different interfaces

REST API provides a powerful, convenient, and simple Web services API for interacting with Contracting.Works Platform. Its advantages include ease of integration and development, and it’s an excellent choice of technology for use with mobile applications and Web 2.0 projects. 

REST API uses the folowwing  underlying data model and standard objects ... . REST API also follows the same limits as describedn in the [Reference Guide](Reference.md). See also the Limits section in the Reference guide.

To use the API requires basic familiarity with software development, web services, and the Salesforce user interface.

Use this introduction to understand:

- The key characteristics and architecture of REST API. This will help you understand how your applications can best use the Contracting.Works REST resources.
- How to use REST API by following a quick start that leads you step by step through a typical use case.



## Contracting.Works REST Resources

A REST resource is an abstraction of a piece of information or an action, such as a single data record, a collection of records, or a query. Each resource in REST API is identified by a named Uniform Resource Identifier (URI) and is accessed using standard HTTP methods (HEAD, GET, POST, PATCH, DELETE). REST API is based on the usage of resources, their URIs, and the links between them.

You use a resource to interact with your Contracting.Works. For example, you can:

- Retrieve summary information about the API versions available to you.
- Obtain detailed information about a Contracting.Works object, such as Account, User, or a custom object.
- Perform a query or search.
- Update or delete records.

Suppose you want to retrieve information about the Contracting.Works version. Submit a request for the [Versions](XXX) resource.

```
curl https://XXX/services/data/
```



The output from this request is as follows.

```
[    {        "version":"20.0",        "url":"/services/data/v20.0",        "label":"Winter '11"    }    ...]
```



Contracting.Works runs on multiple server instances. The examples in this guide use yourInstance in place of a specific instance. Replace that text with the instance for your org.

Important characteristics of the Contracting.Works REST API resources and architecture:

- Stateless

  Each request from client to server must contain all the information necessary to understand the request, and not use any stored context on the server. However, the representations of the resources are interconnected using URLs, which allow the client to progress between states.

- Caching behavior

  Responses are labeled as cacheable or non-cacheable.

- Uniform interface

  All resources are accessed with a generic interface over HTTP.

- Named resources

  All resources are named using a base URI that follows yourContracting.Works URI.

- Layered components

  The Contracting.Works REST API architecture allows for the existence of such intermediaries as proxy servers and gateways to exist between the client and the resources.

- Authentication

  The Contracting.Works REST API supports OAuth 2.0 (an open protocol to allow secure API authorization). See Authorization documentation for more details.

- Support for JSON and XML

  JSON is the default. You can use the HTTP ACCEPT header to select either JSON or XML, or append .json or .xml to the URI (for example, /Account/001D000000INjVe.json).

  The JavaScript Object Notation (JSON) format is supported with UTF-8. Date-time information is in [ISO8601](http://www.iso.org/iso/catalogue_detail?csnumber=40874) format.

  XML serialization is similar to SOAP API. XML requests are supported in UTF-8 and UTF-16, and XML responses are provided in UTF-8.

- Friendly URLs

  Why make two API calls when you can make just one? A friendly URL provides an intuitive way to construct REST API requests and minimizes the number of round-trips between your app and Contracting.Works. Friendly URLs are available in API version XXX and later.Accessing a contact’s parent account without a friendly URL involves requesting the contact record using the SObject Rows resource. Then you examine the account relationship field to obtain the account ID and request the account record with another call to SObject Rows. Using a friendly URL, you can access the account in a single call directly from the contact’s path: /services/data/XXX/sobjects/contact/id/account.This functionality is exposed via the [SObject Relationships](XXX) resource. For more examples of using friendly URLs to access relationship fields, see [Traverse Relationships with Friendly URLs](XXX).



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

## Using Conditional Requests

To support response caching, REST API allows conditional request headers that follow the standards defined by the HTTP 1.1 specification.

For strong validation, include either the 

If-Match

 or 

If-None-Match

 header in a request, and reference the entity tags (ETag) of the records you want to match against. For weak validation, include either the 

If-Modified-Since

 or 

If-Unmodified-Since

 header in a request along with the date and time you want to check against. The REST API conditional headers follow the HTTP 1.1 specification with the following exceptions.

- When you include an invalid header value for If-Match, If-None-Match, or If-Unmodified-Since on a PATCH or POST request, a 400 Bad Request status code is returned.
- The If-Range header isn’t supported.
- DELETE requests are not supported with these headers.

- ETag

  HTTP 1.1 specification: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.19](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.19)

  The ETag header is a response header that’s returned when you access the SObject Rows resource. It’s a hash of the content that’s used by the If-Match and If-None-Match request headers in subsequent requests to determine if the content has changed.

  Supported resources: SObject Rows (account records only)

  Example: ETag: "U5iWijwWbQD18jeiXwsqxeGpZQk=-gzip"

- If-Match

  HTTP 1.1 specification: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.24](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.24)

  The If-Match header is a request header for SObject Rows that includes a list of ETags. If the ETag of the record you’re requesting matches an ETag specified in the header, the request is processed. Otherwise, a 412 Precondition Failed status code is returned, and the request isn’t processed.

  Supported resources: SObject Rows (account records only)

  Example: If-Match: "Jbjuzw7dbhaEG3fd90kJbx6A0ow=-gzip", "U5iWijwWbQD18jeiXwsqxeGpZQk=-gzip"

- If-None-Match

  HTTP 1.1 specification: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.26](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.26)

  The If-None-Match header is a request header for SObject Rows that’s the inverse of If-Match. If the ETag of the record you’re requesting matches an ETag specified in the header, the request isn’t processed. A 304 Not Modified status code is returned for GET or HEAD requests, and a 412 Precondition Failed status code is returned for PATCH requests.

  Supported resources: SObject Rows (account records only)

  Example: If-None-Match: "Jbjuzw7dbhaEG3fd90kJbx6A0ow=-gzip", "U5iWijwWbQD18jeiXwsqxeGpZQk=-gzip"

- If-Modified-Since

  HTTP 1.1 specification: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.25](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.25)

  The If-Modified-Since header is a time-based request header. The request is processed only if the data has changed since the date and time specified in the header. Otherwise, a 304 Not Modified status code is returned, and the request isn’t processed.

  Supported resources: SObject Rows, SObject Describe, Describe Global, and Invocable Actions

  Example: If-Modified-Since: Tue, 10 Aug 2015 00:00:00 GMT

- If-Unmodified-Since

  HTTP 1.1 specification: [www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.28](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.28)

  The If-Unmodified-Since header is a request header that’s the inverse of If-Modified-Since. If you make a request and include the If-Unmodified-Since header, the request is processed only if the data hasn’t changed since the specified date. Otherwise, a 412 Precondition Failed status code is returned, and the request isn’t processed.

  Supported resources: SObject Rows, SObject Describe, Describe Global, and Invocable Actions

  Example: If-Unmodified-Since: Tue, 10 Aug 2015 00:00:00 GMT

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



## Our usage of OPENAPI

### 

## Make REST API calls

In REST API calls, include the URL to the API service for the environment:

- Test: https://XXX
- Production: https://XXX

Also, include your access token to prove your identity and access protected resources.

This sample call, which shows details for a web experience profile, includes a *bearer* token in the `Authorization` request header. This type of token lets you complete an action on behalf of a resource owner.

```bash
curl -v -X GET https://api.XXX/v1/payment-experience/web-profiles/XP-8YTH-NNP3-WSVN-3C76 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer Access-Token"
BASHcopy
```

The response shows the web experience profile details:

```json
{
  "id": "XP-8YTH-NNP3-WSVN-3C76",
  "name": "exampleProfile",
  "temporary": false,
  "flow_config": {
    "landing_page_type": "billing",
    "bank_txn_pending_url": "https://example.com/flow_config/"
  },
  "input_fields": {
    "no_shipping": 1,
    "address_override": 1
  },
  "presentation": {
    "logo_image": "https://example.com/logo_image/"
  }
}
```



## LiURI structure

Jira REST APIs provide access to resources (that is, data entities) via URI paths. To use a REST API, your application makes an HTTP request and parse the response.

The Jira REST API uses JSON as its communication format and the standard HTTP methods like `GET`, `PUT`, `POST`, and `DELETE`. URIs for Jira REST API resource have the following structure:

Copy

```
1 ``http://host:port/context/rest/api-name/api-version/resource-name
```

Currently there are two API names available, which will be discussed later on this page:

- `auth:` – for authentication-related operations.
- `api:` – for everything else.

The current API version is `2`. However, there is also a symbolic version called `latest` that resolves to the latest version supported by the given Jira instance.

As an example, if you wanted to retrieve the JSON representation of issue JRA-9 from Atlassian's public issue tracker, you would access:

Copy

```
1 ``https://jira.atlassian.com/rest/api/latest/issue/JRA-9
```

## Using the REST APIs

The following topics describe how the Jira REST APIs are structured and how you can interact with them.

### Expansion

To simplify API responses, the Jira REST API uses resource expansion. This means that the API will only return parts of the resource when explicitly requested. This helps you avoid problems that can occur when you request too little information (for example, you need to make many requests) or too much information (for example, performance impact on Jira).

You can use the `expand` query parameter to specify a comma-separated list of entities that you want expanded, identifying each of them by name. For example, appending `?expand=names,renderedFields` to an issue's URI requests the inclusion of the translated field names and the HTML-rendered field values in the response.

The following example expands the `name` and `renderedFields` fields for workorder JRA-9:

Copy

```
1 ``https://XXX/rest/api/latest/workorder/JRA-9?expand=names,renderedFields
```

To find out which fields are expandable, look at the `expand` property in the parent object. In the following example, the `widgets` field is expandable:

Copy

```
1 2 3 4 5 ``{    "expand": "widgets",    "self": "http://XXX/rest/api/resource/KEY-1",    "widgets": { "widgets": [], "size": 5 } }
```

You can use the dot notation to specify expansion of entities within another entity. For example, `?expand=widgets.fringels` would expand the `widgets` collection and also the `fringels` property on each widget.

### Pagination

Contracting.Works uses pagination to limit the response size for resources that return a potentially large collection of items. A request to a paged API will result in a `values` array wrapped in a JSON object with some paging metadata, for example:

Copy

```
1 2 3 4 5 6 7 8 9 10 ``{    "startAt" : 0,    "maxResults" : 10,    "total": 200,    "values": [        { /* result 0 */ },        { /* result 1 */ },        { /* result 2 */ }    ] }
```

- `startAt:` – the item used as the first item in the page of results.
- `maxResults:` – number of items to return per page.
- `total:` – total number of items to return, subject to server-enforced limits. This number *may change* as the client requests the subsequent pages. A client should always assume that the requested page can be empty. REST API consumers should also consider the field to be optional. In cases when calculating this value is too expensive it may not be included in the response.
- `isLastPage:` – indicates whether the current page returned is the last page of results.

Clients can use the `startAt`, `maxResults`, and `total` parameters to retrieve the desired number of results. Note that each API resource or method may have a different limit on the number of items returned, which means you can ask for more than you are given. The actual number of items returned is an implementation detail and this can be changed over time.

### Ordering

Some resources support ordering by a specific field. This is provided by the `orderBy` query parameter.

Ordering can be ascending or descending. To specify the type of ordering, use the "+" or "-" symbols for ascending or descending respectively. By default, ordering is ascending. For example, `?orderBy=+name` will order the results by name in ascending order. 

### Self links

Many fields have a `self` link that takes you to the canonical location for that resource. For example:

Copy

```
1 2 3 4 5 6 7 ``"reporter": {    "self": "http://XXXX/rest/api/2/user?username=admin",    "name": "admin",    "emailAddress": "admin@example.com",    "displayName": "Administrator",    "active": true },
```

Making a GET request to the `self` link can sometimes provide you with additional information about the field. For example, if we make a GET request for the `self` link for the `reporter` field above, the response will contain additional information about the user, including the timezone and groups. 

### Special request and response headers

- `X-AUSERNAME` – response header that contains either username of the authenticated user or 'anonymous'.
- `X-XXX-Token` – methods that accept multipart/form-data will only process requests with `X-XXX-Token: no-check` header.

### Error responses

Most resources will return a response body in addition to the status code. Usually, the JSON schema of the entity returned is the following:

Copy

```
1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 ``{    "id": "https://XXXX/REST/schema/error-collection#",    "title": "Error Collection",    "type": "object",    "properties": {        "errorMessages": {            "type": "array",            "items": {                "type": "string"            }        },        "errors": {            "type": "object",            "patternProperties": {                ".+": {                    "type": "string"                }            },            "additionalProperties": false        },        "status": {            "type": "integer"        }    },    "additionalProperties": false }
```

### Field input formats

**Summary**: A system field that is a single line of text.

Copy

```
1 ``"summary": "This is an example summary"
```

**Description**: A system field that is multiple lines of text.

Copy

```
1 ``"description": "This is an example description with multiples lines of text\n separated by\n line feeds"
```

**Components**: A system field that is multiple values addressed by 'name'.

Copy

```
1 ``"components" : [ { "name": "Active Directory"} , { "name": "Network Switch" } ]
```

**Due date**: A system field that is a date in 'YYYY-MM-DD' format.

Copy

```
1 ``"duedate" : "2015-11-18"
```

**Labels**: A system field that is an array of string values.

Copy

```
1 ``"labels" : ["examplelabelnumber1", "examplelabelnumber2"]
```

**Checkbox custom field**: A custom field that allows you to select a multiple values from a defined list of values. You can address them by `value` or by `ID`.

Copy

```
1 ``"customfield_11440" : [{ "value" : "option1"}, {"value" : "option2"}]
```

or

Copy

```
1 ``"customfield_11440" : [{ "id" : 10112}, {"id" : 10115}]
```

**Date picker custom field**: A custom field that is a date in `YYYY-MM-DD` format.

Copy

```
1 ``"customfield_11441" : "2015-11-18"
```

**Date time picker custom field**: A custom field that is a date time in ISO 8601 `YYYY-MM-DDThh:mm:ss.sTZD` format.

Copy

```
1 ``"customfield_11442" : "2015-11-18T14:39:00.000+1100"
```

**Labels custom field**: A custom field that is an array of strings.

Copy

```
1 ``"customfield_11443" : [ "rest_label1", "rest_label2" ]
```

**Number custom field**: A custom field that contains a number.

Copy

```
1 ``"customfield_11444" : 123
```

**Radio button custom field**: A custom field that allows you to select a single value from a defined list of values. You can address them by `value` or by `ID`.

Copy

```
1 ``"customfield_11445" : { "value": "option2" }
```

or

Copy

```
1 ``"customfield_11445" : { "id": 10112 }
```

**Cascading select custom field**: A custom field that allows you to select a single parent value and then a related child value. You can address them by `value` or by `ID`.

Copy

```
1 ``"customfield_11447" : { "value": "parent_option1", "child": { "value" : "p1_child1"} }
```

or

Copy

```
1 ``"customfield_11447" : { "id": 10112, "child": { "id" : 10115 } }
```

**Multi-select custom field**: A custom field that allows you to select a multiple values from a defined list of values. You can address them by `value` or by `ID`.

Copy

```
1 ``"customfield_11448" : [ { "value": "option1" }, { "value": "option2" } ]
```

or

Copy

```
1 ``"customfield_11448" : [ { "id": 10112 }, { "id": 10115 } ]
```

**Single-select custom field**: A custom field that allows you to select a single value from a defined list of values. You can address them by `value` or by `ID`.

Copy

```
1 ``"customfield_11449" : { "value": "option3" }
```

or

Copy

```
1 ``"customfield_11449" : { "id": 10112 }
```

**Multi-line text custom field**: A custom field that allows multiple lines of text.

Copy

```
1 ``"customfield_11450": "An example of multiples lines of text\n separated by\n line feeds"
```

**Text custom field**: A custom field that allows a single line of text.

Copy

```
1 ``"customfield_11450": "An example of a single line of text"
```

**URL custom field**: A custom field that allows a URL to be entered.

Copy

```
1 ``"customfield_11452" : "http://www.contracting.works"
```

**Single-user picker custom field**: A custom field that allows a single user to be selected.

Copy

```
1 ``"customfield_11453" : { "name":"tommytomtomahawk" }
```

**Multi-user picker custom field**: A custom field that allows multiple users to be selected.

Copy

```
1 ``"customfield_11458" : [ { "name":"inigomontoya" }, { "name":"tommytomtomahawk" }]
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

