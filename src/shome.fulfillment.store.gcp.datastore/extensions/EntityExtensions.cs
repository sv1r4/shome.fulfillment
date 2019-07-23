using Google.Cloud.Datastore.V1;

namespace shome.fulfillment.store.gcp.datastore.extensions
{
    public static class EntityExtensions
    {
        public static string GetPropertyValueString(this Entity entity, string propertyName)
        {
            if (!entity.Properties.TryGetValue(propertyName, out var value) 
                || string.IsNullOrWhiteSpace(value.StringValue))
            {
                return string.Empty;
            }

            return value.StringValue;
        }
    }
}
