# Meter Readings

Meter Readings is web-based API written in C# ASP .NET Core that enables a user to upload (currently through an API only) a CSV file of meter readings that have corresponding account numbers, and for that data to be persisted in a database. Further to this, the user can request (currently through an API only) details of existing meter readings that have been previously submitted and accepted. There is the option for expansion of other CRUD related operations.

All API responses are standardised, so an indication of success or failure will be present.

## Getting started

Please follow the information below carefully to setup Meter Readings in a development environment.

### License

Meter Readings is licensed under the GNU General Public License (version 3).

### System Requirements

You will need the following base system requirements in order to get Meter Readings running in a development environment:

- Microsoft Visual Studio 2022 (older versions of Microsoft Visual Studio may be suitable, but these have *not* been tested). Alternatively, although not tested, it is anticipated that JetBrains Rider would also be suitable.
- .NET 6 or above (older versions of .NET may be suitable, but these have *not* been tested and the projects will require downgrading).
- The project brief did not specify a particular DBMS to use, so PostgreSQL will be used for the purposes of this task. You can use your own PostgreSQL server if you prefer, or you can use the one provided **(the authentication details for this will have been emailed to you - they are not in this repository for security reasons)**.
- Database authentication details will need setting up as a system environment variable (see instructions below).
- The project has been tested in a Windows 10 Pro development environment. It is anticipated that assorted Linux distributions will also be suitable, but these have *not* been tested.
- An HTTP client to initiate the requests. This guide documents Postman, but other HTTP clients should work.

### External Libraries Used

Meter Readings makes use of the following external libraries in addition to the libraries made available in .NET 6:

- AutoMapper - this is used to automatically map between database entity types and Data Transfer Objects (DTOs).
- Autofac - this provides an enhanced experience for Inversion of Control (IOC) and dependency injection.
- CsvHelper - this helps parsing CSV files.
- Dapper - an Object Relational Mapping (ORM) libary to help automatically map SQL query results into class objects.
- ExpressionExtensionSQL - a library to convert expressions into SQL.
- FluentValidation - a utility to provide an abstracted validation for various types.
- Npgsql - the official .NET PostgreSQL database connector.
- xUnit - testing framework.

These are used in accordance with their license.

### Configuring the Database

 The project brief did not specify a particular DBMS to use, so PostgreSQL will be used for the purposes of this task. You can use your own PostgreSQL server if you prefer, or you can use the one provided **(the authentication details for this will have been emailed to you - they are not in this repository for security reasons)**.
 
 In any case, the debugging process and testing process in the projects **will automatically *delete* the entire contents of the database specified** on each build or test of each collection. Please bare this in mind if you use your own database server. Migrations are then ran followed by the configured seeding the specified environment.
 
 You need to provide a PostgreSQL connection string as a system environment variable named `MeterReadingsConnectionString` with a value similar to below (you should change out the relevant parts as appropriate):
 
 `Server=dbms.domain.com;Port=5678;Database=database;User Id=username;Password=password;`
 
 You should restart Microsoft Visual Studio or any other command-line utility after setting the environment variable for it to take effect.
 
 ### Running the Project
 
 To run the project, you can either:
 
 - Run the project within Microsoft Visual Studio (or another IDE).
 - In a command-line utility, from the `MeterReadings` directory within `src`, run `dotnet build`.

The project will be available at `https://localhost:5001`. You should ignore any certificate errors.
 
 ### Testing
 
 Unit and integration tests are provided in the `MeterReadings.Tests` project. Specific information around testing is documented below, but to run the tests, you can either:
 
 - Run the tests from the `Test` menu in Microsoft Visual Studio in the open project (a similar process will apply for other IDEs).
 - In a command-line utility, in either the solution directory or `MeterReadings.Tests` project directory, run `dotnet test`.
 
If any of the tests fail, please check your database configuration.

## Application Structure

Whilst the project brief represented a very small project, the application structure has been architected assuming a much larger project, or one that will be built on overtime. To that effect, the application structure might seem over-engineered or large compared to the project brief, but this is simply assuming a larger, wider project. The following outlines the general structure of Meter Readings, and the purpose of each of its constituent projects:

##### Project `MeterReadings`
This is the web layer of the appplication and it is therefore responsible for setting up the server. It is the entrypoint for HTTP requests and translates data from a format suitable for the HTTP paradigm into a format suitable to be processed by the service and data access layers.
##### Project `MeterReadings.Core`
This is the domain layer of the application. Typically, you wouldn't see CRUD related operations here. Instead, it encapsulates business logic that doesn't depend on external resources or isn't directly initiated by external consumers of the application. Entities and repository definitions (interfaces) are also defined in this project.
##### Project `MeterReadings.Core.Services`
This is the application services and infrastructure services layer. Services and classes found in this layer are used by external consumers to interface with your system.
##### Project `MeterReadings.DataAccess`
This is the data access layer. This includes concrete implementations of the repository interfaces defined in `MeterReadings.Core`. Database migrations, seeds and other SQL or database related code is stored here.
##### Project `MeterReadings.Shared`
This project includes other classes; typically utility classes, that don't ordinarily or naturally fit into other projects.
##### Project `MeterReadings.Tests`
This project contains all unit and integration tests.

## Using the API End Points

The web layer defines a number of end points, including the one required by the project brief, which are documented below. All API end points have a standard JSON template that is used in all cases.

### Response Structure
As is typical with REST API's, in end points geared towards the retrieval of a single piece of data by a unique identifier, in the event that such data cannot be found, an HTTP 404 error will be returned. Otherwise, a response including at a minimum: `success` (a boolean value indicating success or failure of the request) will be included.

For responses that are successful, this will most likely include `payload` (a member that includes the actual response data, whether this be a DTO, a list of DTOs, etc). In the example of retrieving a single meter reading by its identifier, the response might look like:

```
{
    "payload": {
        "id": 1,
        "account_id": 123,
        "submitted_at": "2021-12-11T22:16:15",
        "value": "12312"
    },
    "success": true
}
```

For responses that are unsuccessful, this will include `error` (a string member with the error message). This might look like:

```
{
    "error": "Missing pagination information",
    "success": true
}
```

### End Points

The following end points are defined:

##### HTTP GET [`/meter-readings/{id:int}`]
This end point retrieves a meter reading that has been previous submitted and accepted by its unique identifier which in this case is an automatically incrementing number. This returns the **unique identifier**, **corresponding account identifier**, **reading submission date and time** and **reading value**.
##### HTTP GET [`/meter-readings`]
This end point retrieves multiple meter readings that have been previously submitted and accepted. For a request to this end point to be successful, you must include `page` and `page_size` query parameters, indicating the page and page size respectively. Without this safeguard, consumers of the API could make a request to this end point and retrieve large amounts of data into system memory, potentially causing downtime.
##### HTTP POST [`/meter-reading-uploads`]
This end point is the end point defined by the project brief. You must supply a CSV file with the headings `AccountId`, `MeterReadingDateTime` and `MeterReadValue` as a file in the request body. The body type should be set to `form-data` and the key for the file should be `meterReadingsFile`. Without this, the request will be unsuccessful.
Each meter reading record must contain an account identifier *that has already been registered*, the date and time of the reading in the format `dd/MMyyyy HH:mm` and the actual meter reading value, which should be 5 numbers, including the leading zeros (e.g. 01356 or 53139). Any record that does not meet these requirements or is a record that has been previously submitted, it will be discarded. Records that were accepted will be returned to the web layer and returned to the HTTP client.

A typical response might look like:

```
{
    "payload": {
        "accepted_readings_count": 3,
        "rejected_readings_count": 32,
        "accepted_readings": [
            {
                "id": 3,
                "account_id": 2351,
                "submitted_at": "2019-04-22T12:25:00",
                "value": "57579"
            },
            {
                "id": 4,
                "account_id": 6776,
                "submitted_at": "2019-05-10T09:24:00",
                "value": "23566"
            },
            {
                "id": 5,
                "account_id": 1239,
                "submitted_at": "2019-05-17T09:24:00",
                "value": "45345"
            }
        ]
    },
    "success": true
}
```

## Testing Architecture

All tests should be placed in the `MeterReadings.Tests` project. Integration tests should derive from the `IntegrationTests` class. This is necessary to enable the migrations and necessary seeding to be run before the testing of each collection. Dependency injection is also setup at the same time. When creating a test, in order to access the *scoped service provider*, necessary to access services and repositories (which are scoped by design), you should create a new lifetime scope by raising the method `InitialiseLifetimeScope` or use a helper method, designed to mimic a particular context (such as with user authentication or without), such as `WithoutUserAsync`.

Some classes in the application are marked as `internal`, for good reasons. However, this does mean that test projects which legitimately have a reason to access those classes are unable to. To overcome this, those projects that contain `internal` members that require testing have an empty `.cs` file created containing the following code:
```
[assembly: InternalsVisibleTo("MeterReadings.Tests")]
```
This enables the testing project to access those classes whilst maintaining the intent behind an `internal` member.
