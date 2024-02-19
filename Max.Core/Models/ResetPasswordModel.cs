using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ResetPasswordModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public string ResetPasswordUrl { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
    }
}
