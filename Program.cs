// Create a new WebApplication builder instance
var builder = WebApplication.CreateBuilder(args);

// Register custom file logger provider based on environment
if (builder.Environment.IsDevelopment()) {
    // In development: use a debug log file, lower log level, and small max file size
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/dev.log",         // Log file path for development
        LogLevel.Debug,         // Minimum log level: Debug
        10 * 1024 * 1024        // Max file size: 10 MB
    ));
} else {
    // In production: use a production log file, higher log level, and larger max file size
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/prd.log",         // Log file path for production
        LogLevel.Information,   // Minimum log level: Information
        10 * 1024 * 1024        // Max file size: 10 MB
    ));
}

// Add controller services to the dependency injection container
builder.Services.AddControllers();

// Register Swagger/OpenAPI services for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => {
    c.DocInclusionPredicate((docName, apiDesc) => {
        return apiDesc.ActionDescriptor?.DisplayName?.Contains("BaseAuthenticatedController") != true;
    });
});

// Register the DB worker service
builder.Services.AddSingleton<PersonalWebApi.Utilities.PostgresDbWorker>();

// Register CORS policy
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Build the WebApplication instance
var app = builder.Build();

// Set the application to listen on specific URLs
app.Urls.Add("https://localhost:5001");
app.Urls.Add("http://localhost:5000");

// Enable CORS
app.UseCors("AllowAll");

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
