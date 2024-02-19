using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
  public class RoleMenuModel
  {
    public int RoleId { get; set; }
    public int MenuId { get; set; }
    public string MenuName { get; set; }
    public int Status { get; set; }
    public string Group { get; set; }
    public string URL { get; set; }
    public int DisplayOrder { get; set; }

    public bool IsSelected
    {
        get
        {
            if (Status == 1) return true;
            return false;
        }
        set
        {
            if (value)
            {
                Status = 1;
            }
            else
            {
                Status = 0;
            }
        }
    }

  }
}
