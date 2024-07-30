using WebApplication1.Interface;
using WebApplication1.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// �]�m�t�m���J����
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ���ҷ�e����
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddControllers();

builder.Services.AddScoped<ITransLogService, TransLogService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
