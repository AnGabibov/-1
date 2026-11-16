using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ ПРИ СТАРТЕ ---
var dbPath = "TimeTracker.db";
if (!File.Exists(dbPath))
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();
    var sqlScript = File.ReadAllText("init.sql");
    using var command = new SqliteCommand(sqlScript, connection);
    command.ExecuteNonQuery();
    Console.WriteLine("База данных успешно создана!");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
