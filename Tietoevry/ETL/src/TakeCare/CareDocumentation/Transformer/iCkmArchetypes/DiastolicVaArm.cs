﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.iCkmArchetypes
{
    internal record DiastolicVaArm
    {
        private readonly IUnitProvider _unitService;

        public DiastolicVaArm(IUnitProvider unitService)
        {
            _unitService = unitService;
        }

        public static void AddDiastolicVaArmData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/blodtryck_vä_arm_diastoliskt");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{"/diastoliskt|magnitude"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Val : keyword.Value.TextVal) : "";
            composedObject[$"{prefix}{v}{"/diastoliskt|unit"}"] = "mm[Hg]";
            composedObject[$"{prefix}{v}{"/mätplats|code"}"] = "at0025";

            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;

            /*if ( string.IsNullOrWhiteSpace(keyword.Value))
            {
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.TermId;
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = "";
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "external_terminology";
            }
            composedObject[$"{prefix}{v}{suffix}{"datatyp|code"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|value"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|terminology"}"] = "external_terminology";

            */


            composedObject[$"{prefix}{v}{suffix}{"dv_text_en"}"] = "*DV_TEXT (en) 54";
            composedObject[$"{prefix}{v}{suffix}{"dv_boolean_en"}"] = false;


            if (keyword.Value != null && keyword.Value.NumVal != null && keyword.Value.NumVal.Unit != null)
            {
                composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
            }
            composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            composedObject[$"{prefix}{v}{suffix}{"nivå"}"] = keyword.ParentCount;
            if (keyword.Childs != null)
            {
                for (int i = 0; i < keyword.Childs.Count; i++)
                {
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}{"/ehr_uri_value"}"] = "ehr://" + keyword.Childs[i];
                }
            }
        }
    }
}
