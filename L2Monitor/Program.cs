﻿using L2Monitor.Common;
using L2Monitor.Config;
using L2Monitor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("log.txt")
                .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
.ConfigureLogging(log =>
{
    log.ClearProviders();
    log.AddSerilog();
})
.ConfigureServices((ctx, services) =>
{
    var globalConfig = ctx.Configuration.Get<AppSettings>();
    services.AddSingleton(globalConfig);
    services.AddSingleton<ClientHandler>();
    services.AddHostedService<L2MainService>();
    services.AddHostedService<CleanupService>();
});


try
{
    await host.RunConsoleAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}



//public static void encXORPass(byte[] raw, int offset, int size, int key)
//{
//    int stop = size - 8;
//    int pos = 4 + offset;
//    int edx;
//    int ecx = key; // Initial xor key

//    while (pos < stop)
//    {
//        edx = (raw[pos] & 0xFF);
//        edx |= (raw[pos + 1] & 0xFF) << 8;
//        edx |= (raw[pos + 2] & 0xFF) << 16;
//        edx |= (raw[pos + 3] & 0xFF) << 24;

//        ecx += edx;

//        edx ^= ecx;

//        raw[pos++] = (byte)(edx & 0xFF);
//        raw[pos++] = (byte)((edx >> 8) & 0xFF);
//        raw[pos++] = (byte)((edx >> 16) & 0xFF);
//        raw[pos++] = (byte)((edx >> 24) & 0xFF);
//    }

//    raw[pos++] = (byte)(ecx & 0xFF);
//    raw[pos++] = (byte)((ecx >> 8) & 0xFF);
//    raw[pos++] = (byte)((ecx >> 16) & 0xFF);
//    raw[pos] = (byte)((ecx >> 24) & 0xFF);
//}


