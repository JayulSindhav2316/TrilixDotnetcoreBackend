using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MemberLoginResponseModel
    {
        public MemberLoginResponseModel()
        {
            Groups = new List<GroupSociableModel>();
        }
        public int PersonId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int OrganizationId { get; set; }
        public int EntityId { get; set; }
        public string EmailDisplay { get; set; }
        public bool VerificationRequired { get; set; }
        public string VerificationToken { get; set; }
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
        public List<GroupSociableModel> Groups { get; set; }
    }
}
