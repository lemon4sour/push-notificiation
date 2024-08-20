using control.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<FirebaseService>(client =>
{
    client.BaseAddress = new Uri("https://push-notification-29e4d-default-rtdb.europe-west1.firebasedatabase.app");
});

builder.Services.AddSingleton(sp => 
    new FirebaseService(
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile("push-notification-29e4d-firebase-adminsdk-r251y-03b21aa619.json"),
            ProjectId = "push-notification-29e4d"
        }), 
        "https://push-notification-29e4d-default-rtdb.europe-west1.firebasedatabase.app"
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
