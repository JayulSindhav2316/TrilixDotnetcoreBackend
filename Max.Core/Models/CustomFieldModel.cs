using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CustomFieldModel
    {
        public CustomFieldModel()
        {
            Customfieldoptions = new HashSet<CustomfieldoptionModel>();
            Customfielddata = new HashSet<CustomfielddataModel>();
            // Customfielddata = new HashSet<CustomfielddataModel>();
        }
        public int BlockId { get; set; }
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
        public List<string> Options { get; set; }
        public int? Status { get; set; }
        public int? Editable { get; set; }
        public int? CustomFieldFor { get; set; }
        public virtual FieldTypeModel FieldType { get; set; }
        public virtual ICollection<CustomfieldoptionModel> Customfieldoptions { get; set; }
        public virtual ICollection<CustomfielddataModel> Customfielddata { get; set; }
        //public virtual Fieldtype FieldType { get; set; }
        //  public virtual ICollection<CustomfielddataModel> Customfielddata { get; set; }
        //public CustomfielddataModel Customfielddata  { get; set; }
    }
}
