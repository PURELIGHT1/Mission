using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PMB.DAO;
using PMB.Models;
using System.Security.Claims;

namespace PMB.Controllers
{
    public class AccountController : Controller
    {
        AccountDAO dao;

        [TempData]
        public string Message { get; set; }

        public AccountController()
        {
            dao = new AccountDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ActLogin(string username, string password)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            bool isAuthenticated = false;
            var data = dao.getKaryawan(username);

            if (data != null)
            {
                // data ada masuk ke pengecekan password
                if (data.password == password)
                {
                    // data password sama
                    isAuthenticated = true;
                    identity = new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, data.nama),
                                    new Claim(ClaimTypes.Role, data.deskripsi),
                                    new Claim("username", data.npp),
                                    new Claim("nama", data.nama),
                                    new Claim("role", data.deskripsi),
                                    new Claim("menu", GenerateMenu(username))
                                }, CookieAuthenticationDefaults.AuthenticationScheme);
                }
                else
                {
                    // password salah
                    Message = "Password salah!";
                }
            }
            else
            {
                // data karyawan tidak ditemukan
                Message = "Data tidak ditemukan!";
            }

            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult LogOut()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        protected string GenerateMenu(string npp)
        {
            string menu = "";
            List<MDLMENU> menus = new List<MDLMENU>();
            List<MDLSUBMENU> submenus = new List<MDLSUBMENU>();

            menus = dao.GetMenuKaryawan(npp);
            submenus = dao.GetSubMenuKaryawan(npp);

            if (menu != null)
            {
                foreach (var row in menus)
                {
                    string deskripsiMenu = "";
                    string iconMenu = "";

                    if (row.DESKRIPSI != null && row.DESKRIPSI.Contains(','))
                    {
                        deskripsiMenu = row.DESKRIPSI?.Split(',')[0];
                        iconMenu = row.DESKRIPSI?.Split(',')[1];
                    }
                    else
                    {
                        deskripsiMenu = row.DESKRIPSI;
                        iconMenu = "far fa-circle";
                    }

                    menu += $"<li class='nav-item'><a href = '#' class='nav-link'><i class='nav-icon {iconMenu}'></i><p>{deskripsiMenu}<i class='fas fa-angle-left right'></i></p></a><ul class='nav nav-treeview'>";
                    var filtersub = submenus.Where(x => x.ID_SI_MENU == row.ID_SI_MENU).ToList();

                    foreach (var submenu in filtersub)
                    {

                        string deskripsiSubMenu = "";
                        string iconSubMenu = "";
                        if (submenu.DESKRIPSI != null && submenu.DESKRIPSI.Contains(','))
                        {
                            deskripsiSubMenu = submenu.DESKRIPSI?.Split(',')[0];
                            iconSubMenu = submenu.DESKRIPSI?.Split(',')[1];
                        }
                        else
                        {
                            deskripsiSubMenu = submenu.DESKRIPSI;
                            iconSubMenu = "far fa-circle";
                        }

                        menu += $"<li class='nav-item'><a href='{submenu.LINK}' class='nav-link'><i class='nav-icon {iconSubMenu}' style='color: #f3ad1b;'></i><p>{deskripsiSubMenu}</p></a></li>";
                    }

                    menu += "</ul></li> ";
                }
            }

            return menu;
        }
    }
}
