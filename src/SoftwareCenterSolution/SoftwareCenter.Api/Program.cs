
using Marten;
using SoftwareCenter.Api.Vendors;
using SoftwareCenter.Api.Vendors.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // going to eat some of the time to start this api, and use some memory.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// above this line is configuring services and opting in to .NET features.

// ask my environment for the connection string to my database

var connectionString = builder.Configuration.GetConnectionString("software") ?? 
    throw new Exception("No software connection string found!");


// look a lot of places - and it always looks in all the places, even if it already found it.
// 1. appsettings.json
// 2. appsettings.{ASPNETCORE_ENVIRONMENT}.json
// 3. looks in the "secrets" in visual studio. Not showing this.
// 4. look in an environment variable on the machine it is running on
//    In this example it would look for connectionstrings__software 
// 5. it will look on the comand line when you do "dotnet run" 

// set up my "service" that will connect to the database
//builder.Services.AddDbContext<MyDbContext>(c => { ... });
builder.Services.AddMarten(config =>
{
    config.Connection(connectionString);
}).UseLightweightSessions();
// It will provide an object that implements A context class.
// IDocumentSession


// AddScoped = One Per HttpRequest
//builder.Services.AddScoped<VendorCreateModelValidator>();
builder.Services.AddVendorServices();

builder.Services.AddScoped<IManageVendors, MartenPostgresVendorManager>();


var app = builder.Build();
// after this line is configuring the HTTP "middleware" - how are actual requests and responses 
// generated.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
}

app.UseAuthorization();

app.MapControllers(); // this uses .NET reflection to scan your application and read those
// routing attributes and create the "routing table" - phone book.
// Current Route Table:
// GET requests to /vendors
//  - Create an instance of the VendorsController
//  - Call the GetAllVendors method.
// POST /vendors
// - create an instance of the vendors controller
// - call the addvendor method
// - but they this is going to need an IDocumentSession 
// GET requests to /vendors/{id} where id loooks like a Guid
//  - create the VendorsController
//  - call the GetVendorById method with that id from the url.

app.Run(); // kestrel web server 

public partial class Program; // willing to explain later if you want. .NET 10 (next week) won't require this.