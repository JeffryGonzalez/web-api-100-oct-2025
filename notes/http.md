# Http Notes

For [Http as an Application Layer Protocol See](https://services.hypertheory.com/explainers/http-protocol/)

## Resources

"Resource oriented architecture"

An importing *thingy* we are exposing through our API in the "ubiquitous language" of the domain using our API.

Identified through a **URI** (Uniform Resource Indicator).

> note: technically, there are two kinds of URI: URL (Uniform Resource Locator - the *id* of the thing), and URN (Uniform Resource Name) - only used in very rare situations, mostly library science, etc.


## Resource Types

### Collections

a name of a group of related things (documents). It's plural.

e.g. "vendors", "employees", "policies", etc.

### Documents

Instances of an item in a collection (usually).

`/employees/39242734078` - "entities"

Documents can have subordinate resoures as well, and those can be either other 
documetns or other collections.


`/employees/{employee-id}/manager`

`/employees/bob-smith`

`/employees/jill-jones`

`/employees/bob-smith/manager` - would be an alias for `/employees/jill-jones`

In documents you can:

- embed data

```http
GET /vendors/3793797397`

200 Ok
Content-Type: application/json


{
    "id": "3793797397",
    "name": "Microsoft",
    "pointOfContact": {
        "name": "Satya Nadella"
    }
}

```

"RESTful"

"representational state transfer"

- Resources
    - Use finite methods per resource
- Representations (more in a sec)
    - at a point in time, good from then until the cache-control says it's expired.
- Links.


```http


GET /employees/bob-smith
Accept: image/png

200 OK
Content-Type: application/json

{
    "name": "Bob Smith",
    "department": "DEV",
   
   
}

GET /employees/bob-smith/manager

{
        "name": "Tricia",
        "department": "CEO"
}

GET /employees/bob-smith/performance-history
Cache-Control: expires at end of the year.

200 Ok

[

]

GET /employees/bob-smith/salary

{
    "current": 18000
}

- or link to data



### Stores

/shopping-carts

The "WHO" question in APIs is ALWAYS* answered by the authorization header.

GET /user/vehicles
Authorization: "jeff gonzalez"



GET /policies/37937973973/vehicles




### Controllers (Sorry for the confusing name)

last resource, get out of jail free - you sort of suck at HTTP, but
we have to ship code - usually have a verb in the resource name.



- POST /add-vehicle-to-policy
+ POST /user/vehicles

- POST /remove-vehicle-from-policy
+ DELETE /user/vehicles/72342093409723

DELETE /user/vehicles
DELETE /user/shopping-card (abandon this cart)

POST /user/claims

{

}

201 Created
Location: /user/claims/3783979783

{

}

POST /user/claims/3783979783/resubmit

{

}


## Http Methods are the ones we use primarily as API developers

## GET
- return a representation of this resource
- on a collection, it means return me something about this collection,
  - but doesn't always mean "return all"

- these MUST be "safe" operations 
- cacheable (should contain cache control header, even if it is `no-cache`)
- idempotent - doing it multiple times is the same as doing it once.

## POST

- collection (e.g. employees)
    - "please consider adding this representation I'm sending to you as a new subordinate resource in your collection"
- document (rare) - "submit this entity for processing"

- Unsafe. Does stuff
- Not cacheable
- not idempotent.

## PUT

- collection (rare) - replace this entire collection with this new representation I'm sending you.
- document - replace this entire document with this new representation

- unsafe
- not cacheable
- is idempotent

**NOTE** - PUT is *not* update like in SQL. It is **REPLACE**

## DELETE

-

## OPTIONS

Super rare for APIs, but returns the set of allowed operations for a resource.

Used by browsers to request permission for cross origin resource sharing.


## HEAD

Like doing a GET, but leave out the data you'd return.

HEAD /vendors/378938938





-- Microservices don't share databases.
