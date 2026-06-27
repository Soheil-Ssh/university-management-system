try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddIdentityServices();
    var app = builder.Build();
    app.UseIdentityPipeline();
    app.Run();
}
catch (Exception ex)
{
    throw;
}