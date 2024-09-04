using Microsoft.Extensions.Logging;
using ChromecastIframe.Helpers;
using Sharpcaster.Models;
using Sharpcaster;
using ChromecastIframe.Dtos;

namespace ChromecastIframe;

public class Program
{
    const string ApplicationId = "0B083724";
    const string NamespaceUri = "urn:x-cast:xyz.illuminat3.cast";
    private static ILogger<Program> Logger = default!;

    public static async Task Main(string[] args)
    {
        Logger = LoggerHelper.SetupLogger<Program>();

        var receivers = await GetReceivers();

        string deviceName = "";

        var receiver = GetReceiverByName(deviceName, receivers);

        if (receiver == null)
        {
            Logger.LogError("Receiver is null");
            return;
        }

        string url = "https://example.com/";

        await SendUrlToChromecast(url, receiver);
    }

    private static async Task<List<ChromecastReceiver>> GetReceivers()
    {
        MdnsChromecastLocator locator = new();
        var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));

        var devices = new List<ChromecastReceiver>();

        for (int i = 0; i < 3; i++)
        {
            var tempDevices = await locator.FindReceiversAsync(source.Token);

            foreach (var dev in tempDevices)
            {
                Logger.LogInformation("Device Found: {DeviceName}", dev.Name);

                if (!devices.Contains(dev) && dev.DeviceUri != null)
                {
                    devices.Add(dev);
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return devices;
    } 

    private static ChromecastReceiver? GetReceiverByName(string name, List<ChromecastReceiver> receivers)
    {
        foreach (var receiver in receivers)
        {
            if (receiver.Name == name)
            {
                return receiver;
            }
        }

        return receivers.FirstOrDefault();
    }

    private static async Task SendUrlToChromecast(string url, ChromecastReceiver chromecast)
    {
        var client = new ChromecastClient();

        await client.ConnectChromecast(chromecast);
        await client.LaunchApplicationAsync(ApplicationId, false);    

        var message = new UrlMessage(url);

        await client.SendAsync(Logger, NamespaceUri, message, "receiver-0");
    }
}