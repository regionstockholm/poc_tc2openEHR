using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class TcLabResult
    {
        private readonly string _prefix;
        private readonly int v;

        public AnyEvent Events { get; set; }
        public OrganizationInfo OrganizationInfo { get; set; }
        public RequestingOrganization RequestingOrganization { get; set; }
        public string InternalLaboratoryIdentifier { get; set; }
        public TestRequestDetails TestRequestDetails { get; set; }
        public string PointOfCareTest { get; set; }
        //public List<Identifier> LisIdentifiers { get; set; }

        public TcLabResult(string prefix, int counter)
        {
            _prefix = prefix;
            v = counter;
            //LisIdentifiers = new List<Identifier>();
            RequestingOrganization = new RequestingOrganization(_prefix);
            OrganizationInfo = new OrganizationInfo(_prefix);
        }

        public override string ToString()
        {
            var result = $@"";

            if(Events != null)
            {
                result += Events.ToString();
            }

            if(RequestingOrganization != null)
            {
                result += RequestingOrganization.ToString();
            }

            if(OrganizationInfo != null)
            {
                result += OrganizationInfo.ToString();
            }

            if(!string.IsNullOrWhiteSpace(InternalLaboratoryIdentifier))
            {
                result += $@"
                ""{_prefix}:{v}/laboratoriets_interna_identifierare"": ""{InternalLaboratoryIdentifier}"",";
            }

            if (TestRequestDetails != null)
            {
                result += $@"
                ""{_prefix}:{v}/beställningsdetaljer/beställarens_beställningsidentifierare"": ""{TestRequestDetails.RequesterOrderIdentifier}"",
                ""{_prefix}:{v}/beställningsdetaljer/mottagarens_beställningsidenitifierare"": ""{TestRequestDetails.ReceiverOrderIdentifier}"",";                
            }

            if (PointOfCareTest!=null)
            {
                result += $@"
                ""{_prefix}:{v}/patientnära_analys"": {PointOfCareTest},";
            }

            /*if (LisIdentifiers != null && LisIdentifiers.Count>0)
            {
                for(int i = 0; i < LisIdentifiers.Count; i++)
                {
                    result += $@"
                ""{_prefix}/lis/identifierare:{i}"": ""{LisIdentifiers[i].Id}"",
                ""{_prefix}/lis/identifierare:{i}|issuer"": ""{LisIdentifiers[i].Issuer}"",
                ""{_prefix}/lis/identifierare:{i}|assigner"": ""{LisIdentifiers[i].Assigner}"",
                ""{_prefix}/lis/identifierare:{i}|type"": ""{LisIdentifiers[i].Type}"",";
                }
            }*/

            return result;
        }
    }

    public class TestRequestDetails
    {
        public string RequesterOrderIdentifier { get; set; }
        public string ReceiverOrderIdentifier { get; set; }
    }
}
