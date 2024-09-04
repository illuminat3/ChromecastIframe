using System.Runtime.Serialization;
using Sharpcaster.Interfaces;

namespace ChromecastIframe.Dtos;

[DataContract]
public class UrlMessage : IMessage
{
    public UrlMessage()
    {
        var type = GetType().Name;
        //Get the type name without the "Message" suffix
        type = type.Replace("Message", "");
        var firstCharacter = true;
        var result = "";
        //Convert the type name to uppercase with underscores
        //example: "ReceiverStatusMessage" -> "RECEIVER_STATUS"
        for (int i = 0; i < type.Length; i++)
        {
            var c = type[i];
            if (firstCharacter)
            {
                firstCharacter = false;
                result += char.ToUpper(c).ToString();
            }
            else if (char.IsUpper(c))
            {
                result += $"_{c}";
            }
            else
            {
                result += char.ToUpper(c).ToString();
            }
        }

        Type = result;
        Url = "";
    }

    public UrlMessage(string url) : base()
    {
        Url = url;
    }

    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "type")]
    public string? Type { get; set; }
}