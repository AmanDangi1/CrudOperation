using Practical2.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Practical2.Controllers
{
    
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind] Login objlog)
        {
            if (ModelState.IsValid)
            {
                string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

                SqlConnection con = new SqlConnection(constr);
                con.Open();

                SqlCommand cmd = new SqlCommand("validate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", objlog.UserName);
                cmd.Parameters.AddWithValue("@Password", objlog.Password);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            bool Isvalidator = true;
                            FormsAuthentication.SetAuthCookie(objlog.UserName, false);
                            return RedirectToAction("../Order/OrderList");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "invalid Username or Password");


                        return RedirectToAction("../Order/Login");
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Register reg)
        {
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("Insert_Register", con);

                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", reg.FirstName);
                cmd.Parameters.AddWithValue("@LastName", reg.LastName);
                cmd.Parameters.AddWithValue("@MobNumber", reg.MobNumber);
                cmd.Parameters.AddWithValue("@Email", reg.Email);
                cmd.Parameters.AddWithValue("@Password", reg.Password);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Order");
        }

    }
}