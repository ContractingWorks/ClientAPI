# Upsert through external identifiers

Main Contracting Works REST API (_ClientApi_) supports two modes when upserting entities:
 * Regular upsert
 * External identifiers upsert
  
  To use **external identifiers upsert** external system needs to be registered in contracting.
  Identifier received after system registration should be used as query parameter called **externalSystemId**, and represents internal (contracting works) id of external system from where external identifiers come. 
  
## Comparison of upsert modes
  
  | Regular upsert example  | External identifiers upsert example | Comparison |
| ------------- | ------------- | ------------- |
|[                                   |[                               ||
| {                                  | {                              | * This two request have identical effects in database|
|   "assignmentId": 17,              |   "assignmentIdExternal": "20",| * Instead of sending contracting id (17), identifier from external system is sent|
|   "customerId" : 10,               |   "customerIdExternal" : "50", | * If *assignmentIdExternal* "20" does not exist assignment will be created|
|   "note":"Test",                   |   "note":"Test",               | * In second example it is possible to use *externalSystemAssignments* and send the same ids in which case provided data will be saved|
|   "externalSystemAssignments":     |   "address":                   | * If not passed *assignmentIdExternal* will be automatically added to *externalSystemAssignments* as *externalId* |
|     [                              |    {                           | * If there is need to set specific *enum_SyncStatusId* to *externalSystemAssignments* then *externalSystemAssignmentDto* needs to be specifically populated with values |
|       {                            |      "addressIdExternal":"1",  | * If *customerIdExternal* is not present in database, save will be stopped.|
|         "externalId":"20",         |      "name":"Test"             | * In address and all other dtos, external identifiers function the same.|
|         "externalSystemId":1,      |    }                           ||
|         "enum_SyncStatusId": 0     |  }                             ||
|       }                            |]                               ||
|     ]                              |                                ||
|    "address":                      |                                ||
|     {                              |                                ||
|       "addressId":20,              |                                ||
|       "name":"Test",               |                                ||
|       "externalSystemAddresses":   |                                ||
|         [                          |                                ||
|           {                        |                                ||
|             "externalId":"1",      |                                ||
|              "externalSystemId":1, |                                ||
|              "enum_SyncStatusId": 0|                                ||
|           }                        |                                ||
|         ]                          |                                ||
|      }                             |                                ||
| }                                  |                                ||
|]                                   |                                ||

 
It is possible to combine this two modes. For example, *customerId* could be used, without using *assignmentId*.
