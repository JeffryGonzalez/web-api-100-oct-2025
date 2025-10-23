Copy this into a file in your repo called "design-sprint.md"

copy the following into that file and include your notes, and suggestions:

Removing a Catalog Item
If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?
- DELETE /vendors/{vendorId}/catalog-items/{itemName}
Authorization: Bearer <your-token>



We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.
The name of each catalog item for a vendor must be unique.
Would this change our POST for catalog items?
- You could store the user who created it during the post to make sure you check it later to verify

What would you return if it is not unique?
- I'm assuming this means item names, and the item names should probably be unique for each vendor, can be same name but difference in the vendor

note: Names of catalog items only have to be unique per vendor. Different vendors can have items with the same names.

Sketch out how you would implement this in your API here, and/or code it up.
