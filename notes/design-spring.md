Copy this into a file in your repo called "design-sprint.md"

copy the following into that file and include your notes, and suggestions:

Removing a Catalog Item

If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?
* We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.

The name of each catalog item for a vendor must be unique.

Would this change our POST for catalog items?

What would you return if it is not unique?
* note: Names of catalog items only have to be unique per vendor. Different vendors can have items with the same names.

Sketch out how you would implement this in your API here, and/or code it up.

My Response:

HTTP DELETE
/vendors/{vendor_guid}/catalog/{catalog_guid}
The POST would change since it would need to check whether that catalog's name was already stored the list of catalogs for that Vendor.
If the name was not unique in the list, then I would return a 400 response.