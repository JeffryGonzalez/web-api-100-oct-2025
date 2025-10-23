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
// dont' send arrays, always send "documents"

```http
200 Ok
Content-Type:application-json

{

"data": [
  { id: 33, name: 'Microsoft'}
],

}

```
```http
POST http://localhost:1337/vendors
Content-Type: application/json
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN1ZUBhb2wuY29tIiwic3ViIjoic3VlQGFvbC5jb20iLCJqdGkiOiJhOWMyY2UxZSIsInJvbGUiOlsiU29mdHdhcmVDZW50ZXIiLCJNYW5hZ2VyIl0sImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTMzNyIsIm5iZiI6MTc2MTI0ODIzNCwiZXhwIjoxODU1ODU2MjM0LCJpYXQiOjE3NjEyNDgyMzUsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.vMmRh-FcuO5dHt0TBirwpp_ZWGEN7wy_y-mez2GASqE

{
  "name": "Microsoft",
  "pointOfContact": {
    "name": "Tony Bob",
    "companyName": "All State",
    "phone": "some-phone",
    "email": "some@email.com"

  }
}
```

```http
GET http://localhost:1337/vendors/tacos

```

PUT  http://localhost:1337/vendors/8bb13b4a-a6e3-4e24-bf0f-0d74c60ea149/point-of-contact

{
  "name": "brenda",
  "email": blah,
  "phone": 939399
}

DELETE http://localhost:1337/vendors/8bb13b4a-a6e3-4e24-bf0f-0d74c60ea149/



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

- none of this stuff can be used unless you are verified (intentified) as an employee.
- some employees are:
  - members of the software center team
    - and some of them are managers of that team


## The Catalog Items

### Find a Vendor
```http
GET http://localhost:1337/vendors
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN1ZUBhb2wuY29tIiwic3ViIjoic3VlQGFvbC5jb20iLCJqdGkiOiJhOWMyY2UxZSIsInJvbGUiOlsiU29mdHdhcmVDZW50ZXIiLCJNYW5hZ2VyIl0sImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTMzNyIsIm5iZiI6MTc2MTI0ODIzNCwiZXhwIjoxODU1ODU2MjM0LCJpYXQiOjE3NjEyNDgyMzUsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.vMmRh-FcuO5dHt0TBirwpp_ZWGEN7wy_y-mez2GASqE
```

### Get a List of Catalog Items For That Vendor 

```http
GET http://localhost:1337/vendors/9f906499-8f1f-40df-97ff-790edb38f1b3/catalog
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN1ZUBhb2wuY29tIiwic3ViIjoic3VlQGFvbC5jb20iLCJqdGkiOiJhOWMyY2UxZSIsInJvbGUiOlsiU29mdHdhcmVDZW50ZXIiLCJNYW5hZ2VyIl0sImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTMzNyIsIm5iZiI6MTc2MTI0ODIzNCwiZXhwIjoxODU1ODU2MjM0LCJpYXQiOjE3NjEyNDgyMzUsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.vMmRh-FcuO5dHt0TBirwpp_ZWGEN7wy_y-mez2GASqE
```

### Get All Catalog Items

```http
GET http://localhost:1337/catalog-items
```

- if that vendor doesn't exist, return a 404

### Add A Catalog Item

- Must be a member of the software team

```http
POST http://localhost:1337/vendors/9f906499-8f1f-40df-97ff-790edb38f1b3/catalog
Content-Type: application/json
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN1ZUBhb2wuY29tIiwic3ViIjoic3VlQGFvbC5jb20iLCJqdGkiOiJhOWMyY2UxZSIsInJvbGUiOlsiU29mdHdhcmVDZW50ZXIiLCJNYW5hZ2VyIl0sImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTMzNyIsIm5iZiI6MTc2MTI0ODIzNCwiZXhwIjoxODU1ODU2MjM0LCJpYXQiOjE3NjEyNDgyMzUsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.vMmRh-FcuO5dHt0TBirwpp_ZWGEN7wy_y-mez2GASqE

{
  "name": "Visual Studio Code",
  "description": "An Editor For Developers"
}
```

```http
DELETE http://localhost:1337/vendors/9f906499-8f1f-40df-97ff-790edb38f1b3/catalog-items
Content-Type: application/json
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN1ZUBhb2wuY29tIiwic3ViIjoic3VlQGFvbC5jb20iLCJqdGkiOiJhOWMyY2UxZSIsInJvbGUiOlsiU29mdHdhcmVDZW50ZXIiLCJNYW5hZ2VyIl0sImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MTMzNyIsIm5iZiI6MTc2MTI0ODIzNCwiZXhwIjoxODU1ODU2MjM0LCJpYXQiOjE3NjEyNDgyMzUsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.vMmRh-FcuO5dHt0TBirwpp_ZWGEN7wy_y-mez2GASqE

{
  "Name": "Visual Studio Code"
}
```