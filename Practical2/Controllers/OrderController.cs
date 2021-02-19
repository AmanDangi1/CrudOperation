using Newtonsoft.Json;
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
    [Authorize]
    public class OrderController : Controller
    {
        // GET: Order

       
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CustomerList()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Manage(int? mid)
        {
            CreateOrUpdateOrder model = new CreateOrUpdateOrder();
            model.mid = Convert.ToInt32(mid);

            GetAutoOrderNo(model);
            GetCustomerData(model);
            GetProductData(model);
            GetCustomerDataById(model);
            GetOrderDetailsById(model);
          
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(CreateOrUpdateOrder model)
        {
            if (ModelState.IsValid)
            {
                if (model.mid == 0)
                {

                    GetAutoOrderNo(model);
                    GetCustomerData(model);
                    GetProductData(model);
                    InsertCustomerDetails(model);
                    InsertProductDetails(model);
                }
                else
                {
                    ManageUpdateData(model);
                    GetAutoOrderNo(model);
                    GetCustomerData(model);
                    GetProductData(model);
                }
            }
            //return View("OrderList",model);
            return RedirectToRoute("MyRoute");

        }
        public ActionResult OrderList()
        {
            DataTable dborderdetails = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlDataAdapter sqld = new SqlDataAdapter("GetOrderDetails", con);
                sqld.Fill(dborderdetails);

            }
            return View(dborderdetails);
        }

        public void GetAutoOrderNo(CreateOrUpdateOrder model)
        {
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("SELECT Max(Id) as Id,COUNT(Id) as OrdeNo from [dbo].[Order]", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int a = Convert.ToInt32(rdr["Id"]);
                    if (a != 0)
                    {
                        int b = a + 1;
                        model.Id = Convert.ToInt32(b);
                    }
                    int i = Convert.ToInt32(rdr["OrdeNo"]);
                    if (i != 0)
                    {
                        int j = i + 1;
                        model.OrderNo = Convert.ToString("000000000" + j.ToString());
                        model.OrderDate = DateTime.Now;
                    }
                    else
                    {
                        model.OrderNo = "0000000001";
                    }
                    //order.OrderNo = Convert.ToString(rdr["OrdeNo"]);
                }
                con.Close();
            }
        }
        public void GetCustomerData(CreateOrUpdateOrder model)
        {
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

            model.Customers = new List<DropDownViewModel>();
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = " SELECT Id,CustomerName FROM Customers";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                model.Customers.Add(new DropDownViewModel()
                                {
                                    Name = sdr["CustomerName"].ToString(),
                                    Id = Convert.ToInt32(sdr["Id"]),
                                });
                            }
                        }
                        con.Close();
                    }

                }

            }

        }
        public void GetProductData(CreateOrUpdateOrder model)
        {
            model.OrderDetailsFormModel = new OrderDetailViewModel();
            model.OrderDetailsFormModel.Items = new List<DropDownViewModel>();
            {
                string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = " SELECT Id,ItemName,Amount FROM Products";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                var item = new DropDownViewModel()
                                {
                                    Name = sdr["ItemName"].ToString(),
                                    Id = Convert.ToInt32(sdr["Id"]),
                                };
                                model.OrderDetailsFormModel.Items.Add(item);
                            }
                        }
                        con.Close();
                    }

                }

            }
        }

        public void InsertCustomerDetails(CreateOrUpdateOrder model)
        {
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("Insert_Order", con);

                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", model.CustomerId);
                cmd.Parameters.AddWithValue("@OrderNo", model.OrderNo);
                cmd.Parameters.AddWithValue("@Date", model.OrderDate);
                cmd.ExecuteNonQuery();
                con.Close();
            }

           
        }
        public void InsertProductDetails(CreateOrUpdateOrder model)
        {
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("Insert_ProductDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                try
                {
                    foreach (var item in model.OrderDetails)
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@ItemId", Convert.ToString(item.ItemId));
                            cmd.Parameters.AddWithValue("@Amount", Convert.ToString(item.Amount));
                            cmd.Parameters.AddWithValue("@Quantity", Convert.ToString(item.Quantity));
                            cmd.Parameters.AddWithValue("@Price", Convert.ToString(item.TotalAmount));
                            cmd.Parameters.AddWithValue("@Name", Convert.ToString(model.Id));

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        catch (Exception e)
                        { }
                    }
                    con.Close();
                }
                catch (Exception)
                {

                    throw;
                }


            }

        }
        private void ManageUpdateData(CreateOrUpdateOrder model)
        {
            #region Update Order
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {


                SqlCommand cmd = new SqlCommand("updatecustomerdetails", con);
                con.Open();

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.mid);

                cmd.Parameters.AddWithValue("@Name", Convert.ToString(model.CustomerId));
                cmd.Parameters.AddWithValue("@Date", model.OrderDate);

                cmd.ExecuteNonQuery();
                con.Close();


            }
            #endregion


            #region Update OrderDetails

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("update_Productrecords", con);


                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                foreach (var item in model.OrderDetails)
                {

                    try
                    {
                        cmd.Parameters.AddWithValue("@ItemId", Convert.ToString(item.ItemId));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToString(item.Amount));
                        cmd.Parameters.AddWithValue("@Quantity", Convert.ToString(item.Quantity));
                        cmd.Parameters.AddWithValue("@Price", Convert.ToString(item.TotalAmount));
                        cmd.Parameters.AddWithValue("@Id", Convert.ToString(model.mid));

                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                    }
                    catch (Exception e)
                    {

                    }

                }
                con.Close();

            }
            #endregion



        }

        public JsonResult PrepareProductData(int itemid)
        {
            var Amount = string.Empty;
            string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetProductDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ItemId", itemid);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {

                    while (rdr.Read())
                    {

                        Amount = rdr["Amount"].ToString();

                    }

                    con.Close();
                }
            }
            return Json(new { Amount });
        }
        #region Delete
        public ActionResult Delete(int id)
        {
            string CS = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Delete From [dbo].[Order] where Id = @id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("OrderList");
        }

        #endregion
        
        public ActionResult GetCustomerDataById(CreateOrUpdateOrder model) 
        {
            string CS = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            try
            {
                #region Get OrderData For edit
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetCustomerDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.mid);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            model.OrderNo = Convert.ToString(rdr["OrdeNo"]);
                            model.OrderDate = Convert.ToDateTime(rdr["Date"]);
                            model.CustomerId = Convert.ToInt32(rdr["Name"]);
                            model.mid = Convert.ToInt32(rdr["Id"]);
                        }
                        con.Close();
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Manage",model);

        }

        public ActionResult GetOrderDetailsById(CreateOrUpdateOrder model)
        {
            string CS = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            #region Get Orderdetails Edit Data
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetAllProductDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.mid);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        var childTable = rdr;
                        model.OrderDetails = new List<OrderDetailViewModel>();
                        while (rdr.Read())
                        {
                            model.OrderDetails.Add(new OrderDetailViewModel
                            {
                                ItemId = Convert.ToInt32(rdr["ItemId"]),
                                Amount = Convert.ToInt32(rdr["Amount"]),
                                Quantity = Convert.ToInt32(rdr["Quantity"]),
                                ItemName = Convert.ToString(rdr["ItemName"]),
                            });
                        }
                        con.Close();
                        con.Close();
                        if (model.OrderDetails.Count > 0)
                        {
                            model.OrderDetails_update = JsonConvert.SerializeObject(model.OrderDetails);
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            #endregion
            return RedirectToAction("Manage",model);
           
        }
    }
}