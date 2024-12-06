using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class TcComposerContext
    {
        private readonly string _prefix;

        public string UserFullName { get; set; }
        public Identifier User { get; set; }

        public string StartTime { get; set; }

        public CodedText Setting { get; set; }

        public CodedText Category { get; set; }
        public CodedText Language { get; set; }
        public CodedText Territory { get; set; }

        public TcComposerContext(string prefix)
        {
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"
                ""{_prefix}/composer|name"": ""{UserFullName}"",
                ""{_prefix}/composer/_identifier:0|id"": ""{User.Value}"",
                ""{_prefix}/composer/_identifier:0|type"": ""{User.Type}"",
                ""{_prefix}/composer/_identifier:0|issuer"": ""{User.Issuer}"",
                ""{_prefix}/context/start_time"": ""{StartTime}"",
                ""{_prefix}/context/setting|code"": ""{Setting.Code}"",
                ""{_prefix}/context/setting|value"": ""{Setting.Value}"",
                ""{_prefix}/context/setting|terminology"": ""{Setting.Terminology}"",
                ""{_prefix}/category|code"": ""{Category.Code}"",
                ""{_prefix}/category|value"": ""{Category.Value}"",
                ""{_prefix}/category|terminology"": ""{Category.Terminology}"",
                ""{_prefix}/language|code"": ""{Language.Code}"",
                ""{_prefix}/language|terminology"": ""{Language.Terminology}"",
                ""{_prefix}/territory|code"": ""{Territory.Code}"",
                ""{_prefix}/territory|terminology"": ""{Territory.Terminology}"",";

            return result;
        }
    }
}
