using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ContainerTreeModel
    {
        public ContainerTreeModel()
        {
            Children = new List<ContainerTreeModel>();
        }
        public string Label { get; set; }
        public string Data { get; set; }
        public string Key { get; set; }
        public string ExpandedIcon { get; set; }
        public string CollapsedIcon { get; set; }
        public string Icon { get; set; }
        public bool Selectable { get; set; }
        public bool Expanded { get; set; }
        public List<ContainerTreeModel> Children { get; set; }
        public ContainerTreeModel Parent { get; set; }
    }
}
