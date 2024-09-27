using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc
{
    public class Keyword
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        public Guid Guid { get; set; }
        /// <summary>
        /// ParentId is the ID of the - reverse link
        /// </summary>
        public Guid ParentId { get; set; }

        public int ParentCount { get; set; }

        [XmlElement("TermId")]
        public string TermId { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }

        [XmlElement("Value")]
        public KeywordValue Value { get; set; }

        [XmlElement("Keyword")]
        public List<Keyword> Keywords { get; set; }
    }

    public class KeywordValue
    {
        [XmlElement("TextVal")]
        public string TextVal { get; set; }

        [XmlElement("NumVal")]
        public NumVal NumVal { get; set; }
    }

    public class NumVal
    {
        [XmlElement("Val")]
        public string Val { get; set; }

        [XmlElement("Unit")]
        public string Unit { get; set; }
    }
}
