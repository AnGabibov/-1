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
    // init.sql копируется в выходную директорию (см. WTT.API.csproj),
    // но на время разработки может лежать рядом с исходниками — учитываем оба варианта.
    var scriptPath = File.Exists("init.sql")
        ? "init.sql"
        : Path.Combine(AppContext.BaseDirectory, "init.sql");

    if (!File.Exists(scriptPath))
        throw new FileNotFoundException(
            "Не найден скрипт инициализации базы данных init.sql.");

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();
    var sqlScript = File.ReadAllText(scriptPath);
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
