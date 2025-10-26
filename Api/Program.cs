using Api.Manager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMatchManager, MatchManager>();
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://www.basketball-bund.net/rest/")
    };
    client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
    client.DefaultRequestHeaders.Add("accept-language", "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");
    //client.DefaultRequestHeaders.Add("origin", "https://www.basketball-bund.net");
    //client.DefaultRequestHeaders.Add("referer", "https://www.basketball-bund.net/static/");
    return client;
});

if(builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)        
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}
else if(builder.Environment.IsProduction())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy//.WithOrigins("https://matchtracker-476209.web.app")
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

