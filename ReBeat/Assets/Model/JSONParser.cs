using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    class JSONParser
    {
        public static void Parse(String jsonText)
        {
            var json = JSON.Parse(jsonText);
        }
    }
}
