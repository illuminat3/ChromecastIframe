using Microsoft.Extensions.Logging;
using ChromecastIframe.Helpers;
using Sharpcaster.Models;
using Sharpcaster;
using ChromecastIframe.Dtos;

namespace ChromecastIframe;

public class Program
{
    const string ApplicationId = "F7FD2183";
    const string NamespaceUri = "urn:x-cast:com.boombatower.chromecast-dashboard";
    const string DeviceName = "";
    const string Url = "";
    private static ILogger<Program> Logger = default!;

    public static async Task Main()
    {
        Logger = LoggerHelper.SetupLogger<Program>();

        var receivers = await GetReceivers();


        var receiver = GetReceiverByName(DeviceName, receivers);

        if (receiver == null)
        {
            Logger.LogError("Receiver is null");
            return;
        }

        await SendUrlToChromecast(Url, receiver);
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
                Logger.LogInformation("Device added: {DeviceName}", receiver.Name);
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

        var sessionId = client.GetChromecastStatus().Application.SessionId;

        var message = new WebMessage
        {
            Url = url,
            Type = "load",
            SessionId = sessionId
        };

        await client.SendAsync(Logger, NamespaceUri, message, sessionId);
    }
}