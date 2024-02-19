using System;
namespace Max.Core.Models
{
    public class AccountContactRoleModel
    {
        public int EntityRoleId { get; set; }
        public int EntityId { get; set; }
        public int ContactRoleId { get; set; }
        public int AccountId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public string Role { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PrimaryEmail { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string TimeLine
        {
            get
            {
                var endDate = EndDate.HasValue ? EndDate.Value.ToString("MMMM dd, yyyy") : "Present";
                if (EffectiveDate == Constants.EntityRole_MinDate)
                {
                    if (EndDate == Constants.EntityRole_MinDate)
                    {
                         return " - Present";
                    }
                    else
                    {
                        return $" - {endDate}";
                    }
                }
                else if (EndDate == Constants.EntityRole_MinDate)
                {
                    return $"{EffectiveDate.ToString("MMMM dd, yyyy")} - Present";
                }
                else
                {
                    return $"{EffectiveDate.ToString("MMMM dd, yyyy")} - {endDate}";
                }
            }
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
