# Web API 100 Lab

Welcome to the lab!

The instructor will walk you through the gist of the lab before you start.

I'll walk you through this: 

- Remove your `dev-environment` from Docker.
- Use the new `dev-environment` for this lab.

## Requirements

We have a frontend application that is served from our API project's `wwwroot` folder.

It needs an API that provides the following basic functionality:

### Adding a Show

When in the UI (at [http://localhost:1338](http://localhost:1338) during development) they are presented a list of shows other users have shared. Right now it will display an error. We'll fix that below.

We'll start with the ability to add a show. When the user clicks the "Share Your Favorite Show" button, they will see a form asking them for the title, description and streaming service of the show to add. 

When they click the "Add Show" button, and if the form is valid (we'll look at the validation rules later), the app will attempt to do this:

```http
POST http://localhost:1338/api/shows
Content-Type: application/json

{
    "name":"Twin Peaks the Return",
    "description":"David Lynch at his best",
    "streamingService":"Amazon Prime"
}
```

This should return something like this:

```http
200 Ok
Content-Type: application/json

{
    "id": "f8d2a439-e9ee-4f3c-9639-6165745e4f50"
    "name":"Twin Peaks the Return",
    "description":"David Lynch at his best",
    "streamingService":"Amazon Prime",
    "createdAt": "2025-07-31T17:21:55.065Z"
}
```

#### Testing and Implementing the Post

I have the *beginnings* of a test in `Shows.Tests/Api/Shows/AddingAShow.cs`. 

> **Note About This Test**: This class is using a `CollectionDefinition`, indicated by the `[Collection("SystemTest")]` attribute. If you look at the `Shows.Tests/Api/Fixtures/SystemTestFixture.cs` file, there is a class that will create and expose the `Host` (an `AlbaHost` here), and make sure it is cleaned up after use. At the bottom of the fixture file, I've created a `[CollectionDefinition]` named `SystemTestFixture`. This means *any* test that uses this collection will *share* the same instance of the running API. You can read more about this here [https://services.hypertheory.com/how-to/xunit-fixtures/](https://services.hypertheory.com/how-to/xunit-fixtures/)

Right now the test in `AddingAShow.cs` called `AddShow()` is using the host to run a scenario that is posting a raw JSON string to the `/api/shows` endpoint. If you run this test, it will fail with a 404.

**Steps**:

1. There is no controller or other endpoint in the `Shows.Api` to handle this request. Write the smallest amount of code possible to make this test pass. I recommend creating a `Shows.Api/Shows/Controller.cs` class in the API project and putting your endpoint there. (You can look at our work from class as a reference).
2. Once you have this test passing, consider creating a model or representation record that can deserialize the JSON string we have sent in the test. Take that as a parameter from the body of the request in your API endpoint.
   1. Replace the hard-coded JSON in the test with an instance of your new model type.
   2. We will implement this like we did in the class. The POST endpoint should return a new representation that has all the data from the request model, but adds an ID, and a `createdAt` property with the time the show was added. (consider making the property a `DateTimeOffset` type, and use `DateTimeOffSet.UtcNow` to provide a value).
   3. Add to the same `AddShow()` test *another* scenario that does an HTTP `GET` request to `/api/shows/{id}`, using the server generated ID of the added show.
   4. The result of that GET request should equal the body returned from the POST request.

> **Note** This is to practice, on your own, what we did in class. In a real application you might do it differently. For example, the next test might be enough *coverage* for us.

1. We need an endpoint that we can GET that returns all the shows. This should be a `/api/shows`.
2. In the test project's `Shows` directory, create another test class that uses the fixture we used in the first test.
3. Write a test that:
   1. POSTs another (different) show to the `/api/shows` endpoint.
   2. Then in the same test, another scenario that does a `GET /api/shows`. 
      1. This should return a 200 OK.
   3. Check to see if the body of the response returned:
      1. Has more than zero shows listed.
      2. That the show you added in step 1 above is one of the shows in the list.

```http
GET http://localhost:1338/api/shows
Accept: application/json
```

> **Extra Credit** The list of shows should be returned in reverse chronological order from the API endpoint. (the newest show should be the first in the list). Can you modify the test to require this? Can you make that pass?

1. Validation
   1. The Validation Rules for the Request are:
      1. `Name`: Required, Minimum Length is 3, Maximum Length is 100.
      2. `Description`: Required, Minimum Length is 10, Maximum Length is 500
      3. `StreamingService`: Required
   2. Use either validation attributes or FluentValidation to validate the model.
   3. If the model is not valid, return a `400 Bad Request`. 
   4. Create another test class in the test project that POSTs just *one* example of a bad request to verify your validation is working correctly.
      1. We will look at other techniques to test validation after the lab.