using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2.Data;

namespace shome.fulfillment.azure.function
{
    public interface IWebHookHandler
    {
        Task<GoogleCloudDialogflowV2WebhookResponse> HandleAsync(string json);
    }
}