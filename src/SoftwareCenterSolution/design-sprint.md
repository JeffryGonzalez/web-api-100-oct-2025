## Removing a Catalog Item

### If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?
- There are at least 2 ways to do this, both sending a DELETE request to:
    - /catalog-items/{catalogItemId} (Probably this one)
    - /vendors/{vendorId}/catalog-item/{catalogItemId} (If we are requiring the vendor to be known)
- The response should be a 200 ok unless 400/500 level errors occur

### We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.
- When we receive the request we need to examine the user:
    - Read the HttpContext and pull the Name claim from the User
    - After reading the item from the database, we would examine the Name that added the entity
    - If the name on the entity matches the name of the User making the Http request, delete it.
    - If the name does not match, return a 403. 

### The name of each catalog item for a vendor must be unique. Would this change our POST for catalog items?
- The POST request body itself would not change, but the implementation would.
- We would need to pull the catalog items for the given vendor and check for an existing catalog item with the name in the request.

### What would you return if it is not unique? 
#### note: Names of catalog items only have to be unique per vendor. Different vendors can have items with the same names.
- If its not unique I would return a 400 bad request.

Sketch out how you would implement this in your API here, and/or code it up.