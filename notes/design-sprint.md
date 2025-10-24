Removing a Catalog Item

If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?

    We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.

The name of each catalog item for a vendor must be unique.

Would this change our POST for catalog items?

What would you return if it is not unique?

    note: Names of catalog items only have to be unique per vendor. Different vendors can have items with the same names.

Sketch out how you would implement this in your API here, and/or code it up.

```http
Delete http://localhost:1338/api/shows
Content-Type: application/json

{
    "name":"Twin Peaks the Return",
    "description":"David Lynch at his best",
    "streamingService":"Amazon Prime"
}
```