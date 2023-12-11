using Services.Common.Domain.Repository;
using Services.Common.Infrastructure.Repository;
using MongoDB.Driver.Core.Configuration;
using Services.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;



// Add services to the container.
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();



services.AddDistributedMemoryCache();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//services.AddTransient(_ => new DapperContext(connectionString));
services.AddTransient<ITestRepository , TestRepository>();


//Build & Run
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();
