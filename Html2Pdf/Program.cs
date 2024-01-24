using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Wkhtmltopdf.NetCore;
using Wkhtmltopdf.NetCore.Options;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(opts => opts.AddServerHeader = false);

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddWkhtmltopdf("/");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/", (IGeneratePdf generatePdf, IFormFile file,
    [FromQuery] Orientation orientation = Orientation.Portrait,
    [FromQuery] Size size = Size.A4,
    [FromQuery] bool? lowQuality = null) =>
{
    generatePdf.SetConvertOptions(new ConvertOptions
    {
        PageOrientation = orientation,
        PageSize = size,
        IsLowQuality = lowQuality ?? false
    });
    using var source = file.OpenReadStream();
    using var reader = new StreamReader(source);
    var buffer = generatePdf.GetPDF(reader.ReadToEnd());
    var filename = string.IsNullOrEmpty(file.FileName) ? "generated.pdf" : Path.ChangeExtension(file.FileName, ".pdf");
    return Results.File(buffer, MediaTypeNames.Application.Pdf, filename);
}).WithName("ConvertHtmlToPdf")
    .Produces<FileResult>(200, MediaTypeNames.Application.Pdf)
    .Produces(500)
    .DisableAntiforgery();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok("healthy"))
    .ExcludeFromDescription();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();