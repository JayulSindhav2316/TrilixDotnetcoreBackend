using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntityDisplayModel
    {
        public int EntityId { get; set; }
        public int? PersonId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Age
        {
            get
            {
                return DateOfBirth.CalculateAge();
            }
        }
    }
}
