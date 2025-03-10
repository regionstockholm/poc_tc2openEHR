using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class Organization
    {
        public string Name { get; set; }

        public List<string> Address { get; set; }
        public string PersonName { get; set; }

    }

    public class OrganizationInfo : Organization
    {
        private readonly string _prefix;

        public Identifier Sid { get; set; }
        public Identifier CareUnit { get; set; }

        public OrganizationInfo(string prefix)
        {
            _prefix = prefix;
        }
        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrEmpty(Name))
            {
                result += $@"
                ""{_prefix}/laboratorium_organisationsinfo/namn"": ""{Name}"",";
            }

            if (Address != null && Address.Count > 0)
            {
                for(int i = 0; i < Address.Count; i++)
                {
                    result += $@"
                ""{_prefix}/laboratorium_organisationsinfo/adress:{i}/adressrad"": ""{Address[i]}"",";
                }
            }

            if (!string.IsNullOrEmpty(PersonName))
            {
                result += $@"
                ""{_prefix}/laboratorium_organisationsinfo/person/namn"": ""{PersonName}"",";
            }

            if (CareUnit != null)
            {
                result += $@"
                ""{_prefix}/laboratorium_organisationsinfo/careunitid"": ""{CareUnit.Id}"",
                ""{_prefix}/laboratorium_organisationsinfo/careunitid|issuer"": ""{CareUnit.Issuer}"",
                ""{_prefix}/laboratorium_organisationsinfo/careunitid|assigner"": ""{CareUnit.Assigner}"",";
                //""{_prefix}/laboratorium_organisationsinfo/careunitid|type"": ""{CareUnit.Type}"",";
            }

            if (Sid != null)
            {
                result += $@"
                ""{_prefix}/laboratorium_organisationsinfo/sid|id"": ""{Sid.Id}"",
                ""{_prefix}/laboratorium_organisationsinfo/sid|issuer"": ""{Sid.Issuer}"",
                ""{_prefix}/laboratorium_organisationsinfo/sid|assigner"": ""{Sid.Assigner}"",";
                //""{_prefix}/laboratorium_organisationsinfo/sid|type"": ""{Sid.Type}"",";
            }

            return result;
        }
    }


    public class RequestingOrganization : Organization
    {
        private readonly string _prefix;

        public Identifier InvoiceeCareUnitExternalId { get; set; }
        public Identifier CareUnitExternalId { get; set; }

        public RequestingOrganization(string prefix)
        {
            _prefix = prefix;
        }
        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrEmpty(Name))
            {
                result += $@"
                ""{_prefix}/beställningsdetaljer/begärande_organisation/namn"": ""{Name}"",";
            }

            if (Address != null && Address.Count > 0)
            {
                for (int i = 0; i < Address.Count; i++)
                {
                    result += $@"
                ""{_prefix}/beställningsdetaljer/begärande_organisation/adress:{i}/adressrad"": ""{Address[i]}"",";
                }
            }

            if (!string.IsNullOrEmpty(PersonName))
            {
                result += $@"
                ""{_prefix}/beställningsdetaljer/begärande_organisation/person/namn"": ""{PersonName}"",";
            }

            if (CareUnitExternalId != null)
            {
                result += $@"
                ""{_prefix}/beställningsdetaljer/begärande_organisation/careunitexternalid"": ""{CareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/begärande_organisation/careunitexternalid|issuer"": ""{CareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/begärande_organisation/careunitexternalid|assigner"": ""{CareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/begärande_organisation/careunitexternalid|type"": ""{CareUnitExternalId.Type}"",";
            }

            if (InvoiceeCareUnitExternalId != null)
            {
                result += $@"
                ""{_prefix}/beställningsdetaljer/begärande_organisation/invoiceecareunitexternalid"": ""{InvoiceeCareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/begärande_organisation/invoiceecareunitexternalid|issuer"": ""{InvoiceeCareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/begärande_organisation/invoiceecareunitexternalid|assigner"": ""{InvoiceeCareUnitExternalId.Id}"",
                ""{_prefix}/beställningsdetaljer/beställningsdetaljer/begärande_organisation/invoiceecareunitexternalid|type"": ""{InvoiceeCareUnitExternalId.Type}"",";
            }

            return result;
        }
    }

}
