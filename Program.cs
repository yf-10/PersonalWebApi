// Create a new WebApplication builder instance
var builder = WebApplication.CreateBuilder(args);

// Register custom file logger provider based on environment
if (builder.Environment.IsDevelopment()) {
    // In development: use a debug log file, lower log level, and small max file size
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/dev.log",         // Log file path for development
        LogLevel.Debug,         // Minimum log level: Debug
        10 * 1024               // Max file size: 10 KB
    ));
} else {
    // In production: use a production log file, higher log level, and larger max file size
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/prd.log",         // Log file path for production
        LogLevel.Information,   // Minimum log level: Information
        20 * 1024 * 1024        // Max file size: 20 MB
    ));
}

// Add controller services to the dependency injection container
builder.Services.AddControllers();

// Register Swagger/OpenAPI services for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the DB worker service
builder.Services.AddSingleton<PersonalWebApi.Utilities.PostgresqlWorker>();
builder.Services.AddSingleton<PersonalWebApi.Utilities.MySqlWorker>();

// Build the WebApplication instance
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    // Enable Swagger UI only in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection middleware
app.UseHttpsRedirection();

// Enable authorization middleware
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Run the application
app.Run();
