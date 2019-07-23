using System;
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Datastore.V1;
using shome.fulfillment.store.gcp.datastore.extensions;

namespace shome.fulfillment.store.gcp.datastore.mapping
{
    public class MqttIntentEntityMapperProfile:AutoMapper.Profile
    {
        public MqttIntentEntityMapperProfile()
        {
            CreateMap<Entity, MqttIntent>()
                .ForMember(dst => dst.IntentName, opt => opt.MapFrom(src => src.Key.Path.Last().Name))
                .ForMember(dst=> dst.Topic, opt=>opt.MapFrom(src=>src.GetPropertyValueString("topic")))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.GetPropertyValueString("message")))
                .ForMember(dst=>dst.ParamMapMqtt, opt=>opt.MapFrom(src=>MapParamArray("paramMapMqtt",src)))
                .ForMember(dst=>dst.ParamMapText, opt=>opt.MapFrom(src=>MapParamArray("paramMapText",src)))
                ;
        }

        

        private static IList<ParamMap> MapParamArray(string key, Entity entity)
        {
            const string paramSeparator = "@";
            const string fromToSeparator = "::";
            if (!entity.Properties.TryGetValue(key, out Value value) || value.ArrayValue?.Values?.Any() != true)
            {
                return Array.Empty<ParamMap>();
            }

            return value.ArrayValue.Values.Select(x =>
            {
                var pairStr = x.StringValue;
                if (string.IsNullOrWhiteSpace(pairStr))
                {
                    throw new InvalidOperationException($"Invalid {key} value = '{value}'. Should be array of strings");
                }
                var paramMap = pairStr.Split(paramSeparator);
                if (paramMap.Length != 2)
                {
                    throw new InvalidOperationException($"Invalid {key} value = '{pairStr}'. Param and map should be separated with '{paramSeparator}'");
                }
                var fromTo = paramMap[1].Split(fromToSeparator);
                if (fromTo.Length != 2)
                {
                    throw new InvalidOperationException($"Invalid {key} value = '{pairStr}'. From and To should be separated with '{fromToSeparator}'");
                }
                return new ParamMap
                {
                    Param = paramMap[0],
                    From = fromTo[0],
                    To = fromTo[1]
                };

            }).ToArray();
        }

    }
}
