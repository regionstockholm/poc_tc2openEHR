﻿using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class MedicationConfig
    {
        public EhrConfig Ehr { get; set; }

        public Template Template { get; set; }

        public FormConfig Form { get; set; }
        public Language Language { get; set; }

    }
}
