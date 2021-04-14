using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;
using L2Monitor.Services;
using Serilog.Events;
using Microsoft.Extensions.Logging;

namespace L2Monitor
{
    class Program
    {


        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            
                            .Enrich.FromLogContext()
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
            try
            {
                Log.Information("Starting host");
                await CreateHostBuilder(args).RunConsoleAsync();
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
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
                    //{
                        
                    //    log.AddDebug();
                    //    log.AddConsole();
                    //}
                    log.AddSerilog();
                })
                .ConfigureServices((services) =>
                {
                    services.AddHostedService<PacketInterceptionService>();
                });
                

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


    }
}
