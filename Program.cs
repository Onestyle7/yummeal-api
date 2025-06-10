var builder = WebApplication.CreateBuilder(args);
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supaBaseKey = builder.Configuration["Supabase:Key"];
builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supaBaseKey, new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true,
}));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();
