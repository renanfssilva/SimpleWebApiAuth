using FluentValidation;
using SimpleWebApiAuth.API.APIs;
using SimpleWebApiAuth.API.Configuration;
using SimpleWebApiAuth.Application.DTOs.Validators;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

builder.Services.AddValidatorsFromAssemblyContaining(typeof(BookValidator));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(RegisterRequestValidator));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(LoginRequestValidator));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureAuthenticationApi();
app.ConfigureUsersApi();
app.ConfigureBooksApi();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    builder.ConfigureServices();

    builder.ConfigureAuthentication();
    builder.ConfigureAuthorization();

    builder.ConfigureData();
}

// add partial class so it could be called from tests
public partial class Program { }