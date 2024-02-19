using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;

namespace Max.Services.Interfaces
{
  public interface IMenuService
  {
    Task<IEnumerable<Menu>> GetAllMenus();
    Task<Tuple<List<MenuModel>, List<string>>> GetMenuByStaffId(int staffId);
    }
}