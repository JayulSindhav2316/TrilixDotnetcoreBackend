
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
    public class MenuService : IMenuService
    {

        private readonly IUnitOfWork _unitOfWork;
        public MenuService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Menu>> GetAllMenus()
        {
            return await _unitOfWork.Menus
                .GetAllMenusAsync();
        }

        public async Task<Tuple<List<MenuModel>,List<string>>> GetMenuByStaffId(int staffId)
        {
            List<MenuModel> menuList = new List<MenuModel>();
            List<string> accessUrls = new List<string>();

            //Get all staff role
            var roles = await _unitOfWork.Staffroles.GetAllStaffRolesByStaffIdAsync(staffId);

            //Get all menus for roles
            string[] rootMenus = Constants.StaffMenus;
           
            foreach (var mainMenu in rootMenus)
            {
                MenuModel menu = new MenuModel();
                List<Rolemenu> roleMenu = new List<Rolemenu>();
                menu.Label = mainMenu;
                foreach (var role in roles)
                {
                    var menuRoleList = await _unitOfWork.Rolemenus.GetMenuByMenuNameRoleIdAsync(role.RoleId, mainMenu);

                    foreach (var menuItem in menuRoleList)
                    {
                        if (!roleMenu.Any(x => x.MenuId == menuItem.MenuId))
                        {
                            roleMenu.Add(menuItem);
                        }
                    }
                }
                foreach (var menuRoleItem in roleMenu)
                { 
                    if (menuRoleItem.Menu.ParentMenuId != 0)
                    {
                        continue;
                    }
                    MenuItemModel menuItem = new MenuItemModel();

                    menuItem.Label = menuRoleItem.Menu.Label;
                    menuItem.Icon = menuRoleItem.Menu.Icon;
                    menuItem.RouterLink.Add(menuRoleItem.Menu.RouterUrl);
                    accessUrls.Add(menuRoleItem.Menu.RouterUrl.StartsWith("/")? menuRoleItem.Menu.RouterUrl:"/"+ menuRoleItem.Menu.RouterUrl);
                    //Get All childmenus

                    var childMenus = await _unitOfWork.Menus.GetMenusByParentIdAsync(menuRoleItem.MenuId);
                    if (!childMenus.IsNullOrEmpty())
                    {
                        menuItem.MenuItems = new List<MenuItemModel>();
                        menuItem.Expanded = "true";
                    }

                    foreach (var childMenu in childMenus)
                    {
                        
                        if (roleMenu.Any(x => x.MenuId == childMenu.MenuId))
                        {
                            MenuItemModel childMenuItem = new MenuItemModel();

                            childMenuItem.Label = childMenu.Label;
                            childMenuItem.Icon = childMenu.Icon;
                            //childMenu.Display=childMenu.Display;
                            //childMenuItem.Expanded = menuRoleItem.Menu.Expanded;
                            childMenuItem.RouterLink.Add(childMenu.RouterUrl);
                            menuItem.MenuItems.Add(childMenuItem);
                            accessUrls.Add(childMenu.RouterUrl.StartsWith("/")? childMenu.RouterUrl:"/" +childMenu.RouterUrl); 
                        }
                        var child = await _unitOfWork.Menus.GetMenusByParentIdAsync(childMenu.MenuId);
                        if (child.Count() > 0)
                        {
                            if (accessUrls.Contains(childMenu.RouterUrl) || accessUrls.Contains("/" + childMenu.RouterUrl))
                            {
                                child.ToList().ForEach(x =>
                                {
                                    accessUrls.Add(x.RouterUrl.StartsWith("/") ? x.RouterUrl : "/" + x.RouterUrl);
                                });
                            }
                        }
                        if (childMenu.Display == false)
                        {
                            accessUrls.Add(childMenu.RouterUrl.StartsWith("/") ? childMenu.RouterUrl : "/" + childMenu.RouterUrl);
                        }
                    }
                    if (menuItem.MenuItems?.Count() == 0)
                    {
                        menuItem.MenuItems = null;
                    }
                    menu.MenuItems.Add(menuItem);
                }
                menuList.Add(menu);
            }
            menuList = menuList.Where(x => x.MenuItems.Count > 0).ToList();
            return Tuple.Create(menuList,accessUrls);
        }
    }
}