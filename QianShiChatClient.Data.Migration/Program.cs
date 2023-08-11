using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using QianShiChatClient.Data;
using Microsoft.EntityFrameworkCore;

// add migration cmd: Add-Migration XXX -Context QianShiChatClient.Data.ChatDbContext -Project QianShiChatClient.Data -StartupProject QianShiChatClient.Data.Migration

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddChatDbContext(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chat.db3"));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var chatDbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    chatDbContext.Database.Migrate();
}

app.Run();