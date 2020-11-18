# Model overview & principles

Both the [GraphQL API](ClientApi.GraphQL.md) and [REST API](ClientApi.md) express aspects of the same data model, the difference being that
the [REST API](ClientApi.md) only allows writing data according to Contracting.Works' [aggregate definitions](#aggregate-definitions).

The complete read model is available through GraphQL metadata. See [GraphQL Voyager](https://contracting-extest-clientapi-graphql.azurewebsites.net/graphql/voyager/) for a visual representation of the model.

## Database principles and conventions

### Field names

| **Postfix** | **Meaning**                                                  | **Examples**    |
| :---------- | :----------------------------------------------------------- | :-------------- |
| ID          | Internal ID for use in the DB. Generally a GUI (uniqueidentifier), but can be integer types for reference data.For Guids, never displayed to end users. | AddressID       |
| Ext         | Data Originating in an external system. Can be skipped where the context is obvious. | EDokNoExt       |
| IDExt       | External ID, originating in an external system.Can be displayed to end users depending on usage needs. | AssignmentIDExt |
| No          | Number presented to end-users in Contracting. Used for searching and quick data entry. | AssignmentNo    |




## Main entities
The list of [aggregate definitions](#aggregate-definitions) provides a complete overview of available aggregates. The main entities and their behavior as seen from an integrated system is described in sub-chapters here. The descriptions are not exhaustive, and are intended to provide an overall understanding of how to interact with the system rather than describing entity details. For details, please refer to the GraphQL metadata and the OpenAPI documentation, together with example projects located on GitHub.


### Settings
All client settings are provided in entities prefixed with *Settings_*. The system expects a single settings entity of each type to be present. Thus the only legal write operation on settings is an update-operation, including the settings' ID (which is always 1). *While this may seem slightly odd at first, it gives us the ability to provide identical read and write-behaviors for settings as for all other data.*

Where the settings express default values from a system table, the setting will have a navigable relation to the related data. For example, the default IndustryType can easily be accessed from the Settings_Main.IndustryType relation through GraphQL.


### Employees


### Products and prices
An integral part of handling products and prices in Contracting.Works is **industry types**. Product names, regulations, price agreements and other similar data is strongly connection to this industry type.

In Contracting.Works, a supplier will serve one or more industry types. Thus, most product and price information is connected to the supplier in context of an industry type, named ***SupplierIndustryType***.


### Hours

### Customers

### Assignments

### Invoices


## Aggregate definitions
Aggregates are a concept from Domain Driven Design, as described [here](https://www.martinfowler.com/bliki/DDD_Aggregate.html). For performance reasons, DDD is not used fully in Contracting.Works, but we do use many of the same terms and abstractions for clarity.

In practice, an the address of an assignment does not make sense alone - it is conceptually a part of the assignment and edited together with the assignment. For most kinds of data, Contracting.Works allow manipulation of the datasets independently to allow for a flexible user experience.

The following aggregates are defined in Contracting.Works, where a bullet indicates the aggregate is an aggregate root, while arrows indicate aggregate parts. Certain aggregates are not roots, but are available through their parent roots. An example of this is an **address**, which is part of both the assignment and customer aggregates. Rether than defining the address elements twice, a single definision is reused.


Address
  -> ExternalSystemAddress

* Assignment                            
  -> Default_Contact						
  -> Address							
  -> AssignmentProductAgreement		
  -> AssignmentParticipant
  -> AssignmentDimension
  -> DimensionValueUse
  -> ExternalSystemAssignment
		
Contact
  -> ExternalSystemContact

* Customer						 
  -> Default_Address					
  -> Invoice_Address					 
  -> Default_Contact					 
  -> CustomerProductAgreement
  -> ExternalSystemCustomer

* Department
  -> ExternalSystemDepartment
									
* Dimension
	-> DimensionValue

* Employee						 
  -> Default_Address					
  -> Employee_Competency
  -> ExternalSystemEmployee

* Invoice
  -> InvoiceDetail

* LedgerAccount
  -> ExternalSystemLedgerAccount

* PaymentTerm
  -> ExternalSystemPaymentTerm

* Product 
  -> ProductExtendedInfo
  -> ExternalSystemProduct

* ProductAgreement
  -> ProductAgreementDetail			
									
* Project
	-> ProjectDimension
	-> DimensionValueUse
	-> ExternalSystemProject
									
* PurchaseAgreement				 
  -> PurchaseAgreementDiscountGroup	
  -> PurchaseAgreementDiscountProduct	

* Service
	-> ServiceDimensionValue

* ServiceAgreement
  -> ServiceAgreementDetail			
									
* Storage			 
  -> Stock								
  -> StockTransaction		
 
* Supplier
  -> ExternalSystemSupplier

* ProductNote
  -> ProductNoteDetail

ProductNoteDetail
  -> ProductNoteDetailDimensionValue

* ProjectAccountReportCategory
  -> ProjectAccountReportCategoryDetail

* SupplierIndustryType
  -> Default_PurchaseAgreement

* VATRate
  -> ExternalSystemVATRate

* WageCode
  -> ExternalSystemWageCode

* AssignmentCategory						 
* Country									 
* DiscountGroup
* EmployeeInvoiceCategory
* ExternalSystem
* ImportFile
* IntegratedSystemConfig
* IndustryType							 
* JobCategory								 
* ProductSupplierIndustryType				 
* ProjectAccount
* ProjectAccountCategory
* Settings_Assignment						 
* Settings_Main							 
* SupplierIndustryTypeFtpInfo
* SupplierInvoice		
* WageGroup								 
* WagePeriod								 
* WageRate								 
* WageRateEmployee						 