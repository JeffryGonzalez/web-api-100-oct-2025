## Removing a Catalog Item
If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?
- We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.
```http
DELETE /vendors/{vendorId}/catalog/{catalogItemId}
Authentication: bearer {token here}
```
- Check authentication. If not authenticated return a 401
- Lookup the vendor. If the vendor doesn't exist return a 404.
- If the vendor does exist lookup it's catalog and check if the item exists. If it does not exist then return a 200.

- If the vendor and item exist then check if the user is a manager or the user that added the item. You would use the 
    - If the user is a manager (check for the "Manager" role to determine) then delete the item from the database and return a 200
    - If the user is not a manager check if they match the "createdBy" property of the item (check the "Name" claim type to determine). If they match then delete and return a 200.
    - If they do not match either criteria then return a 403


## The name of each catalog item for a vendor must be unique.
Would this change our POST for catalog items?
- Yes we would need to look up the items for the vendor and verify that a item with the name from the request does not already exist before adding the new item to the database. 

What would you return if it is not unique?
- Return a 400 and an error message indicating that the name already exists. I want the user to be aware that the item already exists in case they don't want to add it afterall. And if they do want to add it then I want them to be in control of what it is named.

>note: Names of catalog items only have to be unique per vendor. Different vendors can have items with the same names.

Sketch out how you would implement this in your API here, and/or code it up.