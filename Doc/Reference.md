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
  - x-trace-id = <ID>: Sets the trace ID for the current request. The ID will be used in all logs, responses and outgoing HTTP requests to Contracting services performed during the request.

**On the response:**
  - x-trace-id = <ID>: The same ID as on the incoming request. If no ID was provided, a new guid will be generated and used as the ID. This is the ID to search for in system logs when debugging any issues.
  - x-perf-server-response-start: The time from the incoming request is started and until the API starts writing the response body. This is a good indicator of internal server and database performance; if this time is low any performance issues originate in the network / server infrastructure, not the service itself.

Note: *correlationId* may sometimes be used in place of *traceId*. This is the same thing in Contracting Works, and we are we are standardizing on the *traceId* name here - mostly because it is shorter and thus more convenient.


## Date and time handling

All time information is stored in UTC (Coordinated Universal Time) internally in Contracting Works. We use [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) for date time format specification, and prefer full UTC specification when a time component is present.

When fetching data through GraphQL, the API will return the following formats:

| Type     | Format                    | Example                  | Comment                                                          |
|----------|---------------------------|--------------------------|------------------------------------------------------------------|
| DateTime | yyyy-MM-ddTHH:mm:ss.fffZ  | 2020-11-18T10:34:10.123Z | Fractions are optional, and will only be provided where relevant |
| Date     | yyyy-MM-dd                | 2020-11-18               |                                                                  |

The examples above will be parsed correctly and are what we test internally. Note that we allow more flexible date time inputs when changing data. If a time zone is not explicitly specified, Contracting will always assume that the time is specified in UTC.

**Known issue (fix pending): In some cases, returned data from Contracting are currently missing the time zone specifier (the Z above). The correct time zone is UTC here - local time zone should not be assumed.**


## Error responses

In general, normal HTTP response codes are used for errors. An exception from this rule is that errors encountered when processing GraphQL queries are returned with HTTP response code 200 and an [error response object by GraphQL conventions](https://blog.graphqleditor.com/graphql-vs-rest-errors/).

We define two main types of errors: **Technical** and **User presentable**.

**Technical errors**

- The most common error type.
- Not intended for end-users.
- Has two formats: Json and Text, configurable on the service level. This setting can be overriden by the [custom header](#custom-headers) *ExceptionHttpResponseAsText* for convenience.
- Contains an error message.
- May contain stack trace and other technical details, depending on service configuration and user permissions.
- Contains a *traceId*. The traceId can be found in the [logs](#logging), and is identical to the **x-trace-id** on the request.

Example of a technical error:
```javascript
{
  "message": "Exception: Everything went wrong.",
  "traceId": "8002aadc-0001-b700-b63f-84710c7967bb"
}
```

**User presentable errors**

- Intended for presentation to end-users.
- Json only. 
- Contains an error message text ID, translatable through CW's normal translation mechanism, and optionally a set of parameters (key value pairs) for merging into the translated error message.
- Contains a *traceId*. The traceId can be found in the [logs](#logging), and is identical to the **x-trace-id** on the request.

Example of a user presentable error with parameters for merging:
```javascript
{
  "message": "Exception of type 'Contracting.Lib.Common.Exceptions.Example.ExampleDomainParamException' was thrown.",
  "traceId": "8002aaee-0001-b700-b63f-84710c7967bb",
  "translationKey": "Exception.ParamExample",
  "parameters": {
    "exampleParamKey": "exampleParamValue"
  }
}
```

The error output formats may be tried out on all CW services through the [common admin functions](common-admin-functions) on the service for convenience.


## Request tracing

A common **traceId** (sometimes called a *correlationId*) is used for all requests. This id is passed along to any requests to other services performed internally by the API, and is stored in the technical logs.
See **x-trace-id** in [Custom HTTP headers](#custom-http-headers), the **traceId** field in [error messages](#error-responses) and **traceId** in [Logging](#logging).


## Logging



## Authentication



## Common admin functions

## Validation