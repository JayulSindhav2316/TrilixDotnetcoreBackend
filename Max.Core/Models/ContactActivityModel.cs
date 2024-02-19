using System;
using System.Collections.Generic;

namespace Max.Core.Models
{
    public class ContactActivityModel
    {
        public int ContactActivityId { get; set; }
        public int EntityId { get; set; }
        public int AccountId { get; set; }
        public DateTime? ActivityDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int InteractionType { get; set; }
        public int ActivityConnection { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int StaffUserId { get; set; }
        public int? Status { get; set; }
        public bool IsRoleActivity { get; set; }
        public bool IsAccountActivity { get; set; }
        public bool IsPersonActivity { get; set; }
        public int? IsDeleted { get; set; }
        public CompanyModel Account { get; set; }
        public EntityModel Entity { get; set; }
        public ICollection<ContactActivityInteractionModel> ContactActivityInteractions { get; set; }
        public StaffUserModel StaffUser { get; set; }

    }
    public class ContactActivityOutputModel
    {
        public int ContactActivityId { get; set; }
        public int EntityId { get; set; }
        public int AccountId { get; set; }
        public DateTime? ActivityDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int InteractionType { get; set; }
        public int ActivityConnection { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int StaffUserId { get; set; }
        public int? Status { get; set; }
        public bool IsRoleActivity { get; set; }
        public bool IsAccountActivity { get; set; }
        public bool IsPersonActivity { get; set; }
        public int? IsDeleted { get; set; }
        public CompanyModel Account { get; set; }
        public EntityModel Entity { get; set; }
        public ICollection<ContactActivityInteractionOutputModel> ContactActivityInteractions { get; set; }
        public StaffUserModel StaffUser { get; set; }

    }

    public class ContactActivityInputModel
    {
        public int ContactActivityId { get; set; }
        public int EntityId { get; set; }
        public int AccountId { get; set; }
        public List<InteractionContactDetails> InteractionContactDetails { get; set; }
        public DateTime? ActivityDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int InteractionType { get; set; }
        public int ActivityConnection { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int StaffUserId { get; set; }
        public int? Status { get; set; }
        public int? IsDeleted { get; set; }
        public CompanyModel Account { get; set; }
        public EntityModel Entity { get; set; }
        public ICollection<ContactActivityInteractionModel> ContactActivityInteractions { get; set; }
        public StaffUserModel StaffUser { get; set; }

    }

    public class InteractionContactDetails
    {
        public int AccountId { get; set; }
        public int EntityId { get; set; }
        public List<int> ContactRoleList { get; set; }
    }

}
