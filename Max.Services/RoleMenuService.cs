
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;

namespace Max.Services
{
  public class RoleMenuService : IRoleMenuService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMenuService _menuService;

    public RoleMenuService(IUnitOfWork unitOfWork, IMenuService menuService)
    {
      this._unitOfWork = unitOfWork;
      this._menuService = menuService;
    }

    public async Task<IEnumerable<RoleMenuModel>> GetMenuByRoleId(int roleId)
    {
        var menus = await _menuService.GetAllMenus();
        menus = menus.Where(x => x.Display != false);
        var roleMenu = await _unitOfWork.Rolemenus.GetMenuByRoleIdAsync(roleId);
        var roleMenuModel = new RoleMenuModel();
        List<RoleMenuModel> listRoleMenuModel = new List<RoleMenuModel>();

        foreach (var menuItem in menus)
        {
            roleMenuModel = new RoleMenuModel();

            if (roleMenu.Any(x => x.MenuId == menuItem.MenuId))
            {
                roleMenuModel.Status = 1;
            }
            else
            {
                roleMenuModel.Status = 0;
            }
            
            if(menuItem.Expanded == (int)Status.Active)
            {
                continue;
            }
            var group = await _unitOfWork.MenuGroups.GetByIdAsync(menuItem.GroupId??0);
            roleMenuModel.DisplayOrder = menuItem.DisplayOrder;
            roleMenuModel.MenuId = menuItem.MenuId;
            roleMenuModel.MenuName = $"{group.GroupName}-{menuItem.Label}";
            roleMenuModel.Group = group.MenuName;
            roleMenuModel.RoleId = roleId;
            roleMenuModel.URL = menuItem.RouterUrl;

            listRoleMenuModel.Add(roleMenuModel);
        }

        return listRoleMenuModel.OrderBy(x => x.Group).ThenBy(x => x.DisplayOrder);
    }

    public async Task<bool> UpdateRoleMenubyRoleId(dynamic requestObject)
    {
        int roleId = requestObject.roleId;
        string selectedMenus = requestObject.selectedMenuIDs;

        //Remove the current role-menu links if not in current list

        var roleMenus = await _unitOfWork.Rolemenus.GetMenuByRoleIdAsync(roleId);

        if (roleMenus.Count() > 0)
        {
            _unitOfWork.Rolemenus.RemoveRange(roleMenus);
        }

        List<Rolemenu> listRolemenu = new List<Rolemenu>();

        string[] arraySelectedMenuIDs = selectedMenus.Split(',');

        foreach (var menuId in arraySelectedMenuIDs)
        {
            if(menuId.Length >0)
            {
                Rolemenu roleMenu = new Rolemenu();
                roleMenu.MenuId = Convert.ToInt32(menuId);
                roleMenu.RoleId = roleId;
                listRolemenu.Add(roleMenu);

                //Add parent also

                var menu = await _unitOfWork.Menus.GetByIdAsync(roleMenu.MenuId);
                if(menu.ParentMenuId > 0 )
                {
                    if(!listRolemenu.Any(x => x.MenuId == menu.ParentMenuId))
                    {
                        roleMenu = new Rolemenu();
                        roleMenu.MenuId = menu.ParentMenuId??0;
                        roleMenu.RoleId = roleId;
                        listRolemenu.Add(roleMenu);
                    }
                }
            }
        }

        await _unitOfWork.Rolemenus.AddRangeAsync(listRolemenu);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> DeleteRoleMenuByRoleId(int roleId)
    {
      var roleMenus = await _unitOfWork.Rolemenus.GetMenuByRoleIdAsync(roleId);
      if (roleMenus != null)
      {
        _unitOfWork.Rolemenus.RemoveRange(roleMenus);
        await _unitOfWork.CommitAsync();
        return true;
      }
      return false;
    }

  }
}