try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddIdentityServices(builder.Configuration);

    var app = builder.Build();

    await app.UseIdentityPipeline();
    app.Run();
}
catch (Exception ex)
{
    throw;
}