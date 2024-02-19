using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SociableApiResponseModel
    {
        public SociableApiResponseModel()
        {
            ResponseStatus = new ResponseStatusModel();
        }
        public int EntityId { get; set; }
        public int StaffUserId { get; set; }
        public int ProfileId { get; set; }
        public int SociableUserId { get; set; }
        public int SociableProfileId { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
    }
}
