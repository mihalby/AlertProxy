using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertProxy.Classes
{
    public class Target
    {
        public string UrlTemplate { get; set; }
        public string Bodytemplate { get; set; }
        public Dictionary<string,string> headers { get; set; }
        public string firingEmoji { get; set; }
        public string resolvingEmoji { get; set; }


    }
}
