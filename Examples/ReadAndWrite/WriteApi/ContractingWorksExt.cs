using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Text;

namespace ContractingWorks.WriteApi
{
    // We need to extend the NSWag client so that we can set the Authorization header
    partial class ContractingWorksClient
    {
        public required string CwToken { get; init; }
        public required ILogger Logger { get; init; }

        // Inject the Authorization header when the request is prepared
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            request.Headers.Authorization = new ("Bearer", CwToken);

#if DEBUG
            // Bit messy code to dump the JSON doc to the log
            //  in DEBUG.
            //  This is just to let you see the JSON doc
            //  we send to the REST Api.
            var content = request.Content;
            if (content is not null)
            {
                var bs = content.ReadAsByteArrayAsync().Result;
                var json = UTF8Encoding.UTF8.GetString(bs);
                Logger.LogInformation("Request:{json}", json);
            }
#endif
        }

        partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings)
        {
            // It's important to not include null values in JSON doc to the
            //  REST Api. A null value means that the REST API will try to
            //  set the value in the table to null. 
            //  A null value typically means "leave the value as is".
            //  `JsonIgnoreCondition.WhenWritingNull` means we don't include
            //  null values in the JSON which will leave the value as is
            settings.DefaultIgnoreCondition  = JsonIgnoreCondition.WhenWritingNull  ;
            // The REST API assumes camelCasing which is the idiom for JavaScript
            settings.PropertyNamingPolicy    = JsonNamingPolicy.CamelCase           ;
#if DEBUG
            settings.WriteIndented           = true                                 ;
#else
            settings.WriteIndented           = false                                ;
#endif
        }

    }
}
