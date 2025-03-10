using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class Identifier
    {

        public string Id { get; set; }
        public string Issuer { get; set; }
        public string Assigner { get; set; }
        public string Type { get; set; }
        /*
        public Identifier(string prefix, int counter)
        {
            _prefix = prefix;
            _counter = counter;
        }

        public override string ToString()
        {
            var result = $@"";
            if (!string.IsNullOrWhiteSpace(Id))
            {
                result += $@"
                        ""{_prefix}/identifikator{_counter}/id"": ""{Id}"",";
            }

            if (!string.IsNullOrWhiteSpace(Issuer))
            {
                result += $@"
                        ""{_prefix}/identifikator{_counter}/utfardare"": ""{Issuer}"",";
            }

            if (!string.IsNullOrWhiteSpace(Assigner))
            {
                result += $@"
                        ""{_prefix}/identifikator{_counter}/utfardare"": ""{Assigner}"",";
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                result += $@"
                        ""{_prefix}/identifikator{_counter}/typ"": ""{Type}"",";
            }

            return result;
        }
        */
    }
}
