# Software Center 

They maintain the list of supported software for our company.

We are building them an API.

## Vendors

We have arrangements with vendors. Each vendor has:

- And ID we assign
- A Name
- A Website URL
- A Point of Contact
  - Name
  - Email
  - Phone Number

Vendors have a set of software they provide that we support.

Resource: `/vendors` - (collection resource)

```http
GET http://localhost:1337/vendors
Accept: application/json
```

```http
POST http://localhost:1337/vendors
Content-Type: application/json

{
  "name": "Apple",
  "pointOfContact": {
    "name": "Tim Apple",
    "email": "tim@apple.com",
    "phone": "888 999-9999"
  }
}
```

Resources have a name, the name is technically a URI in the form of:

example: `https://api.company.com/software-center/vendors`

- "The Scheme" (https://) - can be either http or https. 
  - the port.
    - http uses tcp port 80 by default
    - https uses tcp port 443 by default.
    - if you are using something else, you have to specify it.
- "Authority" - api.company.com - server, the "origin" 
- the path - /software-center/vendors - the part we have control over as developers.


## Catalog Items

Catalog items are instances of software a vendor provides.

A catalog item has:
- An ID we assign
- A vendor the item is associated with
- The name of the software item
- A description
- A version number (we prefer SEMVER, but not all vendors use it)



Missing stuff on the request - like name, description, etc. - 400
Vendor Id: has to be in the "form" of a Guid, and...... it could be for a vendor we don't currently support.



Note - One catalog item may have several versions. Each is it's own item.

## Use Cases

The Software Center needs a way for managers to add vendors. Normal members of the team cannot add vendors.
Software Center team members may add catalog items to a vendor.
Software Center team members may add versions of catalog items.
Software Center may deprecate a catalog items. (effectively retiring them, so they don't show up on the catalog)

Any employee in the company can use our API to get a full list of the software catalog we currently support.
