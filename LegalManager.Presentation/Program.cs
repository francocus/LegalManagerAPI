using System.Text.Json.Serialization;
using LegalManager.Application.Interfaces;
using LegalManager.Application.Services;
using LegalManager.Domain.Interfaces;
using LegalManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure: la implementacion concreta se elige aca, en el arranque.
// Singleton porque el repositorio en memoria guarda el estado del proceso.
builder.Services.AddSingleton<IUserRepository, UsersRepository>();
builder.Services.AddSingleton<ICaseRepository, CasesRepository>();
builder.Services.AddSingleton<IAppointmentRepository, AppointmentsRepository>();

// Application: casos de uso.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();