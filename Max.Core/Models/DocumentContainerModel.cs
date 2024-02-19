using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentContainerModel
    {
        public DocumentContainerModel()
        {
            ContainerAccesss = new List<ContainerAccessModel>();
        }

        public int ContainerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AccessControlEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string BlobContinerId { get; set; }
        public string EncryptionKey { get; set; }
        public  int  UserId { get; set; }
        public  List<ContainerAccessModel> ContainerAccesss { get; set; }
    }
}
