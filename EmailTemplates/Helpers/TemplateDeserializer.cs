using System;
using EmailTemplates.Views;
using Newtonsoft.Json;

namespace EmailTemplates.Helpers
{
    public static class TemplateDeserializer
    {
        public static BaseEmailClass GetEmailData(string data, string dataType)
        {
            switch (dataType)
            {
                case nameof(BasicEmailTemplateModel):
                    return JsonConvert.DeserializeObject<BasicEmailTemplateModel>(data);
                default:
                    throw new NullReferenceException($"The defined data type: {dataType} is not defined. " +
                                                     "Please register it to the Template Deserializer if needed.");
            }
        }
    }
}
