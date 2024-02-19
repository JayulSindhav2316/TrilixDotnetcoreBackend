using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class ContactActivityInteractionModel
    {
        public int ContactActivityInteractionId { get; set; }
        public int ContactActivityId { get; set; }
        public int InteractionAccountId { get; set; }
        public int InteractionEntityId { get; set; }
        public int InteractionRoleId { get; set; }
        public int IsDeleted { get; set; }
        public CompanyModel InteractionAccount { get; set; }
        public EntityModel InteractionEntity { get; set; }
        public ContactActivityModel ContactActivity { get; set; }
    }

    public class ContactActivityInteractionOutputModel
    {
        public int ContactActivityInteractionId { get; set; }
        public int ContactActivityId { get; set; }
        public int InteractionAccountId { get; set; }
        public int InteractionEntityId { get; set; }
        public int IsDeleted { get; set; }
        public List<int> InteractionRoles { get; set; }
        public CompanyModel InteractionAccount { get; set; }
        public EntityModel InteractionEntity { get; set; }
        public ContactActivityModel ContactActivity { get; set; }
    }
}
