using Sharpcaster.Messages;
using System.Runtime.Serialization;

namespace ChromecastIframe.Dtos;

[DataContract]
public class WebMessage : MessageWithSession
{
    [DataMember(Name = "url")]
    public string Url { get; set; }
}
