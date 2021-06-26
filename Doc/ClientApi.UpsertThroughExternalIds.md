# Upsert through external identifiers

Main Contracting Works REST API (_ClientApi_) supports two ways of upserting entities:
 * Regular upsert
 * External identifiers upsert
  ---
  To use **external identifiers upsert** external system needs to be registered in contracting.
  Identifier received after system registration should be used as a query parameter called **externalSystemId**, and represents internal (contracting works) id of external system from where external identifiers come. 
  

## Regular upsert example
```json
[                                                              
 {                                                               
   "assignmentId": 17,                 
   "customerId" : 10,                  
   "note":"Test",                                 
   "externalSystemAssignments":                           
     [                                                           
       {                                    
         "externalId":"20",                             
         "externalSystemId":1,                                   
         "enum_SyncStatusId": 0                                 
       }                                                         
     ],                                                       
    "address":                                                      
     {                                                              
       "addressId":20,                                              
       "name":"Test",                                               
       "externalSystemAddresses":                                   
         [                                                          
           {                                                        
             "externalId":"1",                                      
             "externalSystemId":1,                                 
             "enum_SyncStatusId": 0                                
           }                                                        
         ]                                                          
      }                                                             
 }                                                                  
]
 ```
 
 ## External identifiers upsert example
```json
[
  {
  "assignmentIdExternal": "20", 
  "customerIdExternal" : "50",  
  "note":"Test",     
  "address": 
    {
      "addressIdExternal":"1", 
      "name":"Test"
    }
  }
 ]
 ```
* This request has identical effects in database as previous example
* Instead of sending contracting id (*17*), identifier from external system is sent
* If *assignmentIdExternal* *20* does not exist assignment will be created
* If *customerIdExternal* is not present in database, save will be stopped.
* If *addressIdExternal* is not present in database, address will be created.
* It is possible to use *externalSystemAssignments* and send the same ids in which case provided data will be saved
  * If not passed *assignmentIdExternal* will be automatically added to *externalSystemAssignments* as *externalId* 
  * If there is need to set specific *enum_SyncStatusId* to *externalSystemAssignments* then *externalSystemAssignmentDto* needs to be specifically populated with values 
* In address and all other dtos, external identifiers function the same.

It is possible to combine this two modes. For example, *customerId* could be used, without using *assignmentId*.

