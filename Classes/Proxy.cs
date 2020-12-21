using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Stubble;
using Stubble.Core.Builders;
using Stubble.Extensions.JsonNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlertProxy.Classes
{
    public interface IProxy
    {
        Task<List<string>> Process(string target,object alert);
        Task<HttpResponseMessage> ProcessFree(string target, object obj);
    }
    public class Proxy:IProxy
    {
        private readonly ILogger<Proxy> _logger;
    
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptionsSnapshot<Dictionary<string, Target>> _options;

        private readonly HttpClient _client;
       

        public Proxy(ILogger<Proxy> logger,  IHttpClientFactory clientFactory, IOptionsSnapshot<Dictionary<string,Target>> optionsSnapshot)
        {
         
            _logger = logger;
            _clientFactory = clientFactory;
            _options = optionsSnapshot;
        

           _client = _clientFactory.CreateClient();
        }


        public async Task<List<string>> Process(string target, object alert)
        {
          
                List<string> ret = new List<string>();
                try
                {
                    

                    var alerts = JObject.Parse(alert.ToString())["alerts"];

                    //process all alerts in hook from alertmanager
                    foreach (var a in alerts.ToList())
                    {
                        var resp = await ProcessAlert(target, a);
                        ret.Add(string.Format("Status code {0} reason {1}", resp.StatusCode, resp.ReasonPhrase));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
               
                return ret;
          
        }

        private async Task<HttpResponseMessage> ProcessAlert(string target,object alert)
        {
            //var settings = _config.GetSection("targets").GetSection(target);
            var settings = _options.Value[target];

            var st = new StubbleBuilder().Configure(settings => settings.AddJsonNet()).Build();


            

            var Url = st.Render(settings.UrlTemplate, alert);
            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            //Add headers
            foreach (var h in settings.headers)
            {
                request.Headers.Add(h.Key, h.Value);

            }


            var jalert = JObject.Parse(alert.ToString());

            var state = jalert["status"];

            jalert.Add("emoji", "");
            if (state.ToString().ToUpper() == "FIRING") jalert["emoji"] = settings.firingEmoji != null ? settings.firingEmoji  : "";
            if (state.ToString().ToUpper() == "RESOLVED") jalert["emoji"] = settings.resolvingEmoji != null ? settings.resolvingEmoji : "";



            var Body = st.Render(settings.Bodytemplate, jalert);
            
            
            request.Content = new StringContent(
                     Body, Encoding.UTF8, "application/json");

            return await _client.SendAsync(request);
                

            



        }

        public async Task<HttpResponseMessage> ProcessFree(string target, object obj)
        {
            var settings = _options.Value[target];

            var st = new StubbleBuilder().Configure(settings => settings.AddJsonNet()).Build();
            var Url = st.Render(settings.UrlTemplate, obj);
            

            
            var Body = st.Render(settings.Bodytemplate, obj);
                        
            var body = new StringContent(
                     Body, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Content = body;

            return await _client.SendAsync(request);
        }




    }


}
