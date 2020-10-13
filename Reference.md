# Reference guide


## Getting started

If you haven't integrated with Contracting.Works before, read the [Getting started guide](Getting%20started.md) in the documentation. You may also want to read our [REST API overview](ClientApi.md), which describes how the Contracting.Works REST APIs work, including a simple example of a REST call.

## Database principles and conventions

### Field names

| **Postfix** | **Meaning**                                                  | **Examples**    |
| :---------- | :----------------------------------------------------------- | :-------------- |
| ID          | Internal ID for use in the DB. Generally a GUI (uniqueidentifier), but can be integer types for reference data.For Guids, never displayed to end users. | AddressID       |
| Ext         | Data Originating in an external system. Can be skipped where the context is obvious. | EDokNoExt       |
| IDExt       | External ID, originating in an external system.Can be displayed to end users depending on usage needs. | AssignmentIDExt |
| No          | Number presented to end-users in Contracting. Used for searching and quick data entry. | AssignmentNo    |



## Date handling

Dates are stored in UTC format in the database.  For Contracting.Works data presentation to users purpose , date conversion is handled by the GUI.

Datetime:   yyyy-MM-ddTHH:mm:ss.fffZ

Date:  yyyy-MM-dd

However, currently it is not rendered correctly by  GraphQL ... - 

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

## URI structure

Contracting.Works REST APIs provide access to resources (data entities) via URI paths. To use a REST API, your application will make an HTTP request and parse the response. The Jira REST API uses [JSON](http://en.wikipedia.org/wiki/JSON) as its communication format, and the standard HTTP methods like `GET`, `PUT`, `POST` and `DELETE` (see API descriptions below for which methods are available for each resource). URIs for Jira's REST API resource have the following structure:

```
http://host:port/context/rest/api-name/api-version/resource-name
```

Currently there are two API names available, which will be discussed further below:

- `auth` - for authentication-related operations, and
- `api` - for everything else.

There is a [WADL](http://en.wikipedia.org/wiki/Web_Application_Description_Language) document that contains the documentation for each resource in the Contracting.Works REST API. It is available XXX

## Methods

### System

#### Get the liveness status of the system

#### Get the readiness status of the system

#### Get the storage metadata

### Bulk operations

### Graph

#### Get information about the specified index

#### List existing indices

#### Run a graph query

### Jobs

#### Clean up job data

#### Stop a given  job

#### List all jobs

#### Get information for a given  job

### Model

### Records

See GraphQL.md for getting records

See REST.md for operations on the records

### Search

#### Run a full text search

#### Run property query search