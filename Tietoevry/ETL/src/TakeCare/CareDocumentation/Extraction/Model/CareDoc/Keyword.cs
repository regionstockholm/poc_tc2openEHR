using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Extension;

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
        public string? TermId { get; set; }

        [XmlElement("Comment")]
        public string? Comment { get; set; }

        [XmlElement("Value")]
        public KeywordValue? Value { get; set; }

        [XmlElement("Keyword")]
        public List<Keyword>? Keywords { get; set; }
    }

    public class KeywordValue
    {
        private string? textVal;

        [XmlElement("TextVal")]
        public string? TextVal
        {
            get => textVal;
            set => textVal = value?.Replace("\n", "\\n").Replace("\r", "\\n");
        }

        [XmlElement("NumVal")]
        public NumVal? NumVal { get; set; }

        [XmlElement("TermId")]
        public string? TermId { get; set; }
    }

    public class NumVal
    {
        private decimal? val;

        [XmlElement("Val")]
        public string Val
        {
            get
            {
                return val?.ToString() ?? string.Empty; 
            }
            set
            {
                val = value?.GetDecimalNumberValue();
            }
        }

        public decimal? GetDecimalValue()
        {
            return val;
        }

        [XmlElement("Unit")]
        public string? Unit { get; set; }
    }
}
