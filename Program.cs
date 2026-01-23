using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseMySql
    (builder.Configuration.GetConnectionString("DefaultConnection"), new MariaDbServerVersion(new Version(10, 4, 32))

    ));
var app = builder.Build();
