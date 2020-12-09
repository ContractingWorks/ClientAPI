# REST API principles and examples

Contracting Works use a REST API, documented through [OpenAPI 3.0](https://contracting-extest-clientapi.azurewebsites.net/swagger/v1/swagger.json) and is available for interactive testing through a [Swagger page.](https://contracting-extest-clientapi.azurewebsites.net/swagger/index.html)

The API is used for altering data only. To query for data, please refer to the [GraphQl API](ClientApi.GraphQL.md)

Note that while the API uses REST style abstractions, it is *not* RESTful - that is entity paths are not expressed through the API URLs. Instead, entity identifiers are provided as part of the API method payloads.

## Basic principles 

All methods provided by the API follow the same principles, described here. In brief:
  - As part of the method URL, the client id (tenant id) must be provided. Without this, all method calls will fail. See [URL structure](Reference.md#url-structure).
  - A valid bearer token must be set as a header on the request. Without this, all method calls will fail. See [Authentication](Reference.md#authentication).
  - All requests are performed in context of a request specific trace id, which can also be found in the system logs. See [Request tracing](Reference.md#request-tracing).
  - The API provide POST methods only, taking a JSON payload as input. The JSON payload consists of one or more [partial entity specifications](partial_entity-specifications).
  - Two primary methods are provided for each [aggregate root](DataModel.md#aggregate-definitions): **Upsert** and **UpsertMultiple**. See [Operations](#operations).
  - The API will automatically perform bulk operations for storing data. To ensure performance, please provide lists of items rather than sending multiple requests. See [Bulk handling](#bulk-handling).
  - The API will automatically perform merge operations where appropriate. To ensure performance, please do not send data that do not need to be changed. See [Bulk handling](#bulk-handling).
  - The API will automatically send change notification events to integrated systems on data changes. This behavior can be suppressed through request parameters. See [Standard parameters](#standard-parameters).
  - The API is stateless and runs on multiple load balanced nodes.


## Operations

For all [aggregate roots](DataModel.md#aggregate-definitions), the API supports a single primary operation: **Upsert**. The operation will perform a create or update operation as needed.

Basic **CRUD** operation breakdown:
  - **Create**: Upsert, with all mandatory fields filled in. If a legal ID is provided, the entity will be created with the given ID. Otherwise, a new ID will be created by the system. The request will return the entity's ID along with HTTP code 200 (Ok). If creation fails, an error messge will be provided instead, with detail level according to environment configuration. See [Error handling](Reference.md#errors).
  - **Read**: Not supported. For reading entity data, please use the [GraphQl API](ClientApi.GraphQL.md).
  - **Update**: Upsert, with the ID of an existing entity provided. Only provided fields will be updated (in addition to the system fields Sys_LastUpdated and Sys_LastUpdatedBy). The return values are identical to the return values when creating entities.
  - **Delete**: Hard deletes are not supported through the API. For soft deletes, please set the column *Sys_Deactivated* to true.

In addition to **Upsert**, the API supports **UpsertMultiple**. The behavior of Upsert and UpsertMultiple is identical - in fact, under the hood **UpsertMultiple** is used internally. The only difference between the methods is that **UpsertMultiple** will take a list of DTOs (Data Transfer Objects) representing the methods [root aggregate](DataModel.md#aggregate-definitions), and return a list of created or updated IDs.

Please note that due to automatic bulk handling, **UpsertMultiple** is preferred when dealing with multiple root aggregates (for example if importing a number of products).

It should also be noted that due to the support for update operations, the API supports and expects [partial entity specifications](partial-entity-specifications) in the JSON payload.


## Partial entity specifications

When specifying data to be upserted, there is a difference between ignoring a property and setting it to a null value. Using Microsoft-style Json comments to explain the property behavior:
```javascript
{
   "Name":"Åsmund",  // set value
   "Phone":null      // set value to null
   //"Country": null // not altered (not part of JSON)
}
```

The actual JSON sent in the request body would probably looks something like this:
```javascript
{
   "Name":"Åsmund",
   "Phone":null    
}
```

**Note: beaceuse all fields may be omitted from the input json, all fields are defined as nullable in the DTOs.**


## Updating aggregate parts

Each part of an [aggregate](DataModel.md#aggregate-definitions) may either be a single item, such as an the customer.default_Address or a list of items, such as customer.contacts. For customers, the default_Address and invoice_Address both appear as part of the addresses on the customer, but more addresses may be present there.

For example, we might have the following structure on a customer (the example is incomplete):
```javascript
{
   "customerId":"C1",
   "default_AddressId":"A1",
   "invoice_AddressId":"A2",
   "addresses":[{"addressId":"A1"}, {"addressId":"A2"}, {"addressId":"A3"}]
}
```

For direct relations to a single aggregate part (such as default_Address) on the customer, this is represented ***twice*** in the DTOs: as an aggregate part DTO of type Address, and as a field on the entity named default_Address***Id***.
To switch defaultAddress to another already existing address with a known ID, it is sufficient to update the default_Address***Id*** field on the customer DTO.

Example: Setting the default address to the address with ID A3
```javascript
{
   "customerId":"C1",
   "default_AddressId":"A3"
}
```

To update the name field on the existing default address, there are two options:
```javascript
{
   "customerId":"C1",
   "default_Address":{
      "addressId":"A3",
	  "name":"Åsmund"
	  }
}
```

Or, through the addresses list:
```javascript
{
   "customerId":"C1",
   "addresses":[{
      "addressId":"A3",
	  "name":"Åsmund"
	  }]
}
```

The syntax above will *NOT* remove the other addresses from the list, it will instead ignore the other addresses and update the address provided. To remove addresses from the list, set them as deactivated by setting the field Sys_Deactivated to true.

**Do not specify the details twice (such as through both the addresses list and the default_Address relation. This is considered an error, and will not be accepted by the API.**

To create a new entity, either provide a new, unused ID for the entity to be created (most relevant during data migration), or simply omit the ID. To create a new customer with a new default address, the example from above becomes:
```javascript
{
   "default_Address":{
     "name":"Åsmund"
	  }
}
```


## Bulk handling

The API will automatically use bulk upserts when there are more than a threshold level of items in a list. Note that bulk operations may be used when processing a single aggregate root, as the aggregate may contain lists of items.
For example, a customer with a number of (minimal) addresses:

```javascript
{
   "customerId":"C1",
   "addresses":[{"addressId":"A1", "name":"Åsmund"}, {"addressId":"A2", "name":"Cato"}, {"addressId":"A3", "name":"Ruben"}]
}
```
When partially specifying a set of entities, the same set of properties must be present on all elements in the list. For example, omitting the "name" property from the list of addresses.

This is also true if the DTOs originate in different places, as they are merged to a single list and handled together by the API. For example, the following would cause problems:
```javascript
{
   "customerId":"C1",
   "default_Address":{
      "addressId":"A4",
	  "name":"Aleksandra",
	  "address1":"Somewhere"
	  }
   "addresses":[{"addressId":"A1", "name":"Åsmund"}, {"addressId":"A2", "name":"Cato"}, {"addressId":"A3", "name":"Ruben"}]
}
```

The reason for the issue is that only the address with ID A4 specifies the "address1" field, so the field is missing from the specifications given thrugh the "addresses" list.

Note that it is the total amount of a specific DTO which causes bulk operations to be used. Given the following list of customers:
```javascript
[{
   "customerId":"C1",
   "default_Address":{
      "addressId":"A4",
	  "name":"Aleksandra",
	  "address1":"Somewhere"
	  }
   "addresses":[{"addressId":"A1", "name":"Åsmund"}, {"addressId":"A2", "name":"Cato"}, {"addressId":"A3", "name":"Ruben"}, {"addressId":"A9", "name":"Susann"}]
},
{
   "customerId":"C2",
   "default_Address":{
      "addressId":"A5",
	  "name":"Haris",
	  }
   "addresses":[{"addressId":"A6", "name":"Jitesh"}, {"addressId":"A7", "name":"Per-Christian", }, {"addressId":"A8", "name":"Jan"}, {"addressId":"A10", "name":"Jan-Morten"}]
}]
```

When performing the upsert, there are actually 10 addresses and 2 customer objects present here. As it happens, bulk operations currently start when there are 10 objects or more updated, so here a bulk upsert to the DB will happen under the hood on the addresses, while the customer objects will be upserted one by one.


## Standard parameters

In addition to the mandatory clientId (part of the request URL), all methods support "***noEvents***" (boolean query parameter): If set to true, the upsert operation will not cause data change events on the event bus. Most useful when migrating data, but also useful for avoiding integration event loops.

Also, have a look at the supported [custom HTTP headers](Reference.md#custom-http-headers).
