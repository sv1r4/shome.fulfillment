﻿using System.Threading.Tasks;

namespace shome.fulfillment.store
{
    public interface IMqttIntentStore
    {
        Task<MqttIntent> FindAsync(string intentName);
    }
}
