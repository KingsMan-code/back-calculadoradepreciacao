using CalculadoraDepreciacao.Models;
using CalculadoraDepreciacao.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// QUESTPDF License
// ---------------------------
QuestPDF.Settings.License = LicenseType.Community;

// ---------------------------
// CORS Configuration
// ---------------------------

const string CorsPolicy = "FrontPolicy";

// Lê as origens permitidas do appsettings.json / appsettings.Development.json
var allowedOrigins = builder.Configuration
    .GetSection("AllowedCorsOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
            // Se precisar enviar cookies ou tokens cross-origin:
            // .AllowCredentials();
        }
        else
        {
            // fallback básico para desenvolvimento
            policy
                .WithOrigins("http://localhost:5173", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

// ---------------------------
// Swagger / Services
// ---------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CalculationService>();
builder.Services.AddSingleton<ExcelExporter>();
builder.Services.AddSingleton<PdfExporter>();

var app = builder.Build();

// ---------------------------
// Middlewares
// ---------------------------

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// ⚠️ CORS precisa vir ANTES dos endpoints
app.UseCors(CorsPolicy);

// ---------------------------
// Endpoints
// ---------------------------

app.MapPost("/api/calculate", (CalculationRequest req, CalculationService calcService) =>
{
    var resultado = calcService.Calculate(req);
    return Results.Ok(resultado);
});

app.MapPost("/api/export/excel", (CalculationRequest req, ExcelExporter exporter) =>
{
    var bytes = exporter.CreateWorkbookWithFormulas(req);
    return Results.File(
        bytes,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "calculadora.xlsx"
    );
});

app.MapPost("/api/export/pdf", (CalculationRequest req, CalculationService calcService, PdfExporter pdfExporter) =>
{
    var resultado = calcService.Calculate(req);
    var pdf = pdfExporter.CreatePdf(req, resultado);
    return Results.File(pdf, "application/pdf", "calculadora.pdf");
});

// ---------------------------
// Run
// ---------------------------

app.Run();
