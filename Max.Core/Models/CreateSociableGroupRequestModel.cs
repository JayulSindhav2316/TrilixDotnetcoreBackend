using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CreateSociableGroupRequestModel
    {
        public List<CreateSociableGroupRequestLabel> label { get; set; }
        public CreateSociableGroupRequestLinks _links { get; set; }

        public CreateSociableGroupRequestModel()
        {
            label = new List<CreateSociableGroupRequestLabel>();
            _links = new CreateSociableGroupRequestLinks();
        }
    }

    public class CreateSociableGroupRequestLabel
    {
        public string value { get; set; }
    }

    public class CreateSociableGroupRequestLinks
    {
        public CreateSociableGroupRequestType type { get; set; }

        public CreateSociableGroupRequestLinks()
        {
            type = new CreateSociableGroupRequestType();
        }
    }

    public class CreateSociableGroupRequestType
    {
        public string href { get; set; }
    }
}
