var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // going to eat some of the time to start this api, and use some memory.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// above this line is configuring services and opting in to .NET features.
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

app.Run(); // kestrel web server 

public partial class Program; // willing to explain later if you want. .NET 10 (next week) won't require this.