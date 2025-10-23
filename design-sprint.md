Removing a Catalog Item
If we were to add the ability to remove a catalog item from a vendor, what would that look like as an HTTP request?
_For deleting a catalog item with specific permissions, the HTTP request would be a DELETE request with an authorization header containing a security token. The request body is typically empty, and the item's unique identifier is included in the request URL. The server-side API then uses the token to authenticate the user and check their permissions._
We only want Software Center Managers, or the Software Center team member that added it to be able to remove an item.
The name of each catalog item for a vendor must be unique.
Would this change our POST for catalog items?

What would you return if it is not unique?
