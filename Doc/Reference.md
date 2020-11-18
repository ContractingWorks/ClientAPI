# Common API reference
Here we describe API behavior and elements common for all our API services. For a quick reference, please refer to [Getting started guide](Getting%20started.md).

## URL Structure
For all client (tenant) specific data in Contracting Works, the client id is part of the query URL. The URL has the following pattern:
***https://[base url]/client/[client id]/[operation path]***

The keyword "client" is a fixed part of the URL, with the primary purpose of separating client specific and cross client operations at a glance.

Example: Upserting assignment data for client *b-dummydata*:
https://contracting-extest-clientapi.azurewebsites.net/client/b-dummydata/Assignment/Upsert

Note that version information is not part of the URL here. Version information will instead be part of the service base URL, and will be added when needed. See [API versioning](api_versioning) for further information.


## API versioning
All Contracting Works servieces may be updated frequently, and are subject to change. We stive to keep the APIs backwards compatible, but expect that there will be breaking chages to the APIs from time to time.

**Overall version principles:**
- Correct URLs to the currently supported versions of the Contracting Works services will be available through metadata obtained through the Catalog service (not yet launched).
- The Catalog service will also provide information about expected life time for older versions.
- 3rd party developers may sign up to get notifications whenever the API changes or is planned to change. Notifications will be sent to registered email addresses only. The registration process is currently manual but will be automated later on. for now, please send an email to asmund@contracting.works to be added to the email list.
- Old versions will maintained for up to 3 months, depending on usage.
- If serious problems are discovered in old versions, such as security issues or behaviors causing data loss, the misbehaving API versions will be stopped immediately. 
- API principles and behavior is maintained independently of the underlying data model and business logic, and is also versioned independently for convenience.

The APIs are considered under heavy development until January 2021. Until then, frequent breaking changes are to be expected, and only a single version of any API will be running in an environment at any given time.


## Custom HTTP headers

**On the request:**
  - ExceptionHttpResponseAsText = true: Format any error message as text instead of json. Improves error readability when debugging manually.
  - x-correlation-id = <ID>: Sets the correlation ID for the current request. The ID will be used in all logs, responses and outgoing HTTP requests to Contracting services performed during the request.
  
**On the response:**
  - x-correlation-id = <ID>: The same ID as on the incoming request. If no ID was provided, a new guid will be generated and used as the ID. This is the ID to search for in system logs when debugging any issues.
  - x-perf-server-response-start: The time from the incoming request is started and until the API starts writing the response body. This is a good indicator of internal server and database performance; if this time is low any performance issues originate in the network / server infrastructure, not the service itself.


## Date and time handling
All time information is stored in UTC (Coordinated Universal Time) internally in Contracting Works. We use [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) for date time format specification, and prefer full UTC specification when a time component is present.

When fetching data through GraphQL, the API will return the following formats:

| Type     | Format                    | Example                  | Comment                                                          |           
|----------|---------------------------|--------------------------|------------------------------------------------------------------|
| DateTime | yyyy-MM-ddTHH:mm:ss.fffZ  | 2020-11-18T10:34:10.123Z | Fractions are optional, and will only be provided where relevant |
| Date     | yyyy-MM-dd                | 2020-11-18               |                                                                  |

The examples above will be parsed correctly and are what we test internally. Note that we allow more flexible date time inputs when changing data. If a time zone is not explicitly specified, Contracting will always assume that the time is specified in UTC.

**Known issue (fix pending): In some cases, returned data from Contracting are currently missing the time zone specifier (the Z above). The correct time zone is UTC here - local time zone should not be assumed.**


## Error responses  - to be looked at

Most resources will return a response body in addition to the status code. Usually, the JSON schema of the entity returned is the following:



```
{
    "id": "https://XXX/REST/schema/error-collection#",
    "title": "Error Collection",
    "type": "object",
    "properties": {
        "errorMessages": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "errors": {
            "type": "object",
            "patternProperties": {
                ".+": {
                    "type": "string"
                }
            },
            "additionalProperties": false
        },
        "status": {
            "type": "integer"
        }
    },
    "additionalProperties": false
```

## Error types 

This API uses standard HTTP response codes to indicate whether a method completed successfully. A `200` response indicates success. A `400` type response indicates a failure, and a `500` type response indicates an internal system error.

### Specific errors 

#### ErrorResponse

#### ErrorAuhthentification

#### ErrorHTML

## Delta updates

We only update entities that have been entered after the last sync.



## Null handling  - to be checked/updated



If a field is present in the JSON, it will be set to the given value, including null values. If it is not present, the field is skipped.

Example

```
{   
"Name":"Åsmund",  // set value   
"Phone":null      // set value to null   //
"Country": null // not altered (not part of JSON) 
}
```



## Batch handling - to be decided on the policy for the third parties



## Authentication

More on  authetification can be found here: ...

## Resilience  - to be implemented

We use Polly (http://www.thepollyproject.org/) to handle the resilience policies. 

### Timeouts

### Retries

### Backoffs

### Idempotency 

### In case of total failure

### Dead Letter Queue



## Logging

## Versioning - to be implemented



## Limits - to be implemented

## Service catalog - to be implemented

## API End-of-Life

Contracting.Works is committed to supporting each API version for a minimum of XXX years from the date of first release. In order to mature and improve the quality and performance of the API, versions that are more than three years old might cease to be supported.

When an API version is to be deprecated, advance notice is given at least one year before support ends. Contracting.Works will directly notify customers using API versions planned for deprecation.


### Search

#### Run a full text search

#### Run property query search


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



### Error responses

Most resources will return a response body in addition to the status code. Usually, the JSON schema of the entity returned is the following:

Copy

```
1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 ``{    "id": "https://XXXX/REST/schema/error-collection#",    "title": "Error Collection",    "type": "object",    "properties": {        "errorMessages": {            "type": "array",            "items": {                "type": "string"            }        },        "errors": {            "type": "object",            "patternProperties": {                ".+": {                    "type": "string"                }            },            "additionalProperties": false        },        "status": {            "type": "integer"        }    },    "additionalProperties": false }
```

#### 


## Externally available test environments 

## Dealing with authorization & client data access  
