using System.Threading.Tasks;

namespace shome.fulfillment.azure.function.wakeup
{
    public interface IWakeupHandler
    {
        Task HandleAsync();
    }
}
