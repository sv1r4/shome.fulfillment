using System;
using System.Threading.Tasks;

namespace shome.fulfillment.mqtt
{
    public interface IMqttPublisher: IAsyncDisposable
    {
        Task<bool> ConnectMqttAsync();
        Task PublishAsync(string topic, string message);
    }
}
