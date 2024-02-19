using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Customfield
    {
        public Customfield()
        {
            Customfielddata = new HashSet<Customfielddatum>();
            Customfieldlookups = new HashSet<Customfieldlookup>();
            Customfieldoptions = new HashSet<Customfieldoption>();
        }

        public int CustomFieldId { get; set; }
        public int? ModuleId { get; set; }
        public int? OrganizationId { get; set; }
        public int? FieldTypeId { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string Validations { get; set; }
        public int? CharacterLimit { get; set; }
        public int? Required { get; set; }
        public int? CountryCode { get; set; }
        public string DateFormat { get; set; }
        public string DefaultDate { get; set; }
        public string TimeFormat { get; set; }
        public string DefaultTime { get; set; }
        public int? MultipleSelection { get; set; }
        public int? Status { get; set; }
        public int? Editable { get; set; }
        public int? CustomFieldFor { get; set; }

        public virtual Fieldtype FieldType { get; set; }
        public virtual ICollection<Reportfield> Reportfields { get; set; }
        public virtual ICollection<Customfielddatum> Customfielddata { get; set; }
        public virtual ICollection<Customfieldlookup> Customfieldlookups { get; set; }
        public virtual ICollection<Customfieldoption> Customfieldoptions { get; set; }
    }
}
