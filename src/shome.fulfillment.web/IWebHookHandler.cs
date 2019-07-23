using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2.Data;

namespace shome.fulfillment.web
{
    public interface IWebHookHandler
    {Task<GoogleCloudDialogflowV2WebhookResponse> HandleAsync(GoogleCloudDialogflowV2WebhookRequest request);
    }
}