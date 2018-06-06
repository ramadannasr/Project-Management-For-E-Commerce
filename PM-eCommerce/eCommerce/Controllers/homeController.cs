using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eCommerce.Models;
using System.Diagnostics;
using System.Data.SqlClient;

namespace eCommerce.Controllers
{
    public class homeController : Controller
    {
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();
        public static Boolean Authenticated = false;

        // GET: home
        public ActionResult Index()
        {

            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType");

            return View();
            //var employees = db.Employees.Include(e => e.EmployeeType);
            //return View(employees.ToList());
        }


        [HttpPost]
        public ActionResult LogIn([Bind(Include = "UserName,Password")] Employee employee)
        {

            Authenticated = true;

            String name = employee.UserName;
            String pass = employee.Password;
            var myUser = db.Employee.FirstOrDefault(u => u.UserName == name && u.Password == pass);
            if (myUser != null)
            {
                if (myUser.UserType == 1)
                    return RedirectToAction("Index", "Marketing_Director", new { id = myUser.ID });

                else if (myUser.UserType == 2)
                    return RedirectToAction("Index", "Marketing_Team_Leader", new { id = myUser.ID });

                else if (myUser.UserType == 3)
                    return RedirectToAction("Index", "Marketing_Trainee", new { id = myUser.ID });

            }

            else //check whether the user is admin or customer
            {
                var myUser2 = db.Customer.FirstOrDefault(u => u.UserName == name && u.Password == pass);
                if (myUser2 != null)
                    return RedirectToAction("Index", "Customer", new { id = myUser2.ID });
                else
                {
                    var myUser3 = db.Admin.FirstOrDefault(u => u.UserName == name && u.Password == pass);

                    if (myUser3 != null)
                    {

                        return RedirectToAction("Index", "Admin", new { id = myUser3.ID });
                    }
                }//end else


            }//end else
            Response.Write("<script>alert('UserName Or Password  Incorrect.');</script>");
            TempData["msg10"] = "<script>alert('UserName Or Password  Incorrect !');</script>";
            //ModelState.AddModelError("", "UserName or Password Is Wrong!!");

            return RedirectToAction("Index");
        }



        //Log out 
        public ActionResult LogOut()
        {
            Authenticated = false;

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            
            return RedirectToAction("Index", "home");
        }


        // GET: home/Details/5
        public ActionResult Details(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: home/Create
        public ActionResult Create()
        {

            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType");

            return View();
        }

        // POST: home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,Experience,UserType,UserName,Password")] Employee employee, HttpPostedFileBase image)
        {

            if (ModelState.IsValid && image != null)
            {

                employee.Photo = new byte[image.ContentLength];
                image.InputStream.Read(employee.Photo, 0, image.ContentLength);

                if (employee.UserType == 5)
                {

                    Customer customer2 = new Customer();
                    customer2 = db.Customer.FirstOrDefault(u => u.UserName == employee.UserName);
                    if (customer2 != null)
                    {
                        //TempData["msg10"] = "<script>alert('This UserName Is Not Available.');</script>";
                        Response.Write("<script>alert('This UserName Is Not Available.');</script>");
                        ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType", employee.UserType);
                        return View(employee);
                    }


                    Customer customer = new Customer();
                    customer.FirstName = employee.FirstName;
                    customer.LastName = employee.LastName;
                    customer.Mobile = employee.Mobile;
                    customer.Description = employee.JobDescription;
                    customer.UserType = employee.UserType;
                    customer.Photo = employee.Photo;
                    customer.UserName = employee.UserName;
                    customer.Password = employee.Password;
                    customer.Email = employee.Email;
                    db.Customer.Add(customer);
                }
                else
                {
                    Employee emp2 = new Employee();
                    emp2 = db.Employee.FirstOrDefault(u => u.UserName == employee.UserName);
                    if (emp2 != null)
                    {
                        Response.Write("<script>alert('This UserName Is Not Available.');</script>");
                        //TempData["msg10"] = "<script>alert('This UserName Is Not Available.');</script>";
                        ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType", employee.UserType);
                        return View(employee);

                    }

                    db.Employee.Add(employee);
                }





                db.SaveChanges();
                Response.Write("<script>alert('" + employee.FirstName + " " + employee.LastName + " Added Successfully.');</script>");
                TempData["msg10"] = "<script>alert('" + employee.FirstName + " " + employee.LastName + " Added Successfully.');</script>";
                return RedirectToAction("Index");
            }
            else
            {
                if (image == null)
                    Response.Write("<script>alert('Select Photo Please.');</script>");
                ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType", employee.UserType);
                return View(employee);
            }
        }



        //public ActionResult doesUserNameExist(string UserName)  
        //{
        //    //if (Session.Count ==0)
        //    //{

        //        var myUser = db.Employee.FirstOrDefault(u => u.UserName == UserName);
        //        if (myUser != null)
        //        {
        //            // This show the error message of validation and stop the submit of the form
        //            return Json(false, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var myUser2 = db.Customer.FirstOrDefault(u => u.UserName == UserName);
        //            if (myUser2 != null)
        //            {
        //                return Json(false, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var myUser3 = db.Admin.FirstOrDefault(u => u.UserName == UserName);

        //                if (myUser3 != null)
        //                {
        //                    return Json(false, JsonRequestBehavior.AllowGet);

        //                }
        //            }//end else

        //        }//end else

        //    //}

        //    //if (UserName == "x value")
        //    //{
        //    //    // This show the error message of validation and stop the submit of the form
        //    //    return Json(true, JsonRequestBehavior.AllowGet);
        //    //}
        //    //else
        //    //{
        //        // This will ignore the validation and the submit of the form is gone to take place.
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    //}

        //}

        // GET: home/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserType = new SelectList(db.EmployeeTypes, "ID", "UserType", employee.UserType);
        //    return View(employee);
        //}

        // POST: home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,Experience,UserType,UserName,Password")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(employee).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserType = new SelectList(db.EmployeeTypes, "ID", "UserType", employee.UserType);
        //    return View(employee);
        //}

        //// GET: home/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(employee);
        //}

        // POST: home/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Employee employee = db.Employees.Find(id);
        //    db.Employees.Remove(employee);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public PartialViewResult emptyMessage()
        {
            ViewData["empty"] = "There Is No Data To Display!";
            return PartialView();
        }


        //__________________________________________________________________________________________
        //Marketing Director Home
        public PartialViewResult Show_All_Projects()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            var Projects_List = db.Project.SqlQuery(" select * from Project where Approval_ID = 2 AND Project.ID not in(select MD_Cust_Request.Project_ID from MD_Cust_Request where MD_Cust_Request.Project_ID = Project.ID  And MD_Cust_Request.Status_ID=2 Or MD_Cust_Request.MD_ID=@mdid)", new SqlParameter("@mdid", mdid)).ToList();
            if (Projects_List.Count() != 0)
                return PartialView(Projects_List);
            else return PartialView();
        }


        public PartialViewResult Show_All_Projects2()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            var Projects_List = db.Project.SqlQuery(" select * from Project where Approval_ID = 2 AND Project.ID not in(select MD_Cust_Request.Project_ID from MD_Cust_Request where MD_Cust_Request.Project_ID = Project.ID  And MD_Cust_Request.Status_ID=2 Or MD_Cust_Request.MD_ID=@mdid)", new SqlParameter("@mdid", mdid)).ToList();
            if (Projects_List.Count() != 0)
                return PartialView(Projects_List);
            else return PartialView();
        }

        //__________________________________________________________________________________________
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send_Cu_MD_Request(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
                Project pro = db.Project.Find(id);

                //  Debug.WriteLine(id);
                MD_Cust_Request Pro_Request = new MD_Cust_Request();

                Pro_Request.MD_ID = Convert.ToInt32(Session["id"]);
                Pro_Request.Customer_ID = pro.Customer_ID;
                Pro_Request.Status_ID = 1;
                Pro_Request.Project_ID = pro.ID;
                db.MD_Cust_Request.Add(Pro_Request);
                db.SaveChanges();
                Response.Write("<script>alert('Request Has Been Sent Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request Has Been Sent Successfully !');</script>";
                //  TempData["msg"] = "<script>alert('Request Has Been Sent Successfully .');</script>";
                return RedirectToAction("Index");
            }
            else { return RedirectToAction("Index"); }
        }

        //___________________________________________________________________________________________

        
        // Customer Home
        //-------------------ADD PROJECT--------------------------
        // GET: Projects/Create
        public PartialViewResult addProject()
        {

            return PartialView();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]    
        public ViewResult adProject([Bind(Include = "ID,Description,ProjectTitle")] Project project)
        {


            if (ModelState.IsValid && Session["id"] != null)
            {

                int cust_id = Convert.ToInt32(Session["id"]);
                project.Customer_ID = cust_id;
                project.Approval_ID = 1;
                db.Project.Add(project);
                //Debug.WriteLine(project.Approval_ID+" "+project.Customer_ID+" "+project.Description+" "+project.ProjectTitle);
                db.SaveChanges();

                Response.Write("<script>alert('Project Added Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Project Added Successfully !');</script>";
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID != 4"), "ID", "UserType");

            return View("Index");

        }


        public PartialViewResult viewProjects()
        {

            if (Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var Projects = db.Project.SqlQuery("select * from Project where Approval_ID = 2 and Project.Customer_ID = @cid and Project.ID not in (select MD_Cust_Request.Project_ID from MD_Cust_Request where Project.ID = MD_Cust_Request.Project_ID and MD_Cust_Request.Status_ID = 2)", new SqlParameter("@cid", cid)).ToList();
                if (Projects.Count() != 0)
                {

                    return PartialView(Projects);

                }
            }

            return PartialView();

        } // GET: Projects/Edit/5

        public ActionResult EditProject(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // POST: Projects/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject([Bind(Include = "ID,Description,Approval_ID,Customer_ID,ProjectTitle")] Project project)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (ModelState.IsValid)
            {
                project.Approval_ID = 1;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<script>alert('Project Edited Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Project Edited Successfully !');</script>";
                return RedirectToAction("Index", "home");
            }

            return View(project);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            Project post = db.Project.Find(id);
            db.Project.Remove(post);
            db.SaveChanges();
            Response.Write("<script>alert('Post Removed Successfully .');</script>");
            TempData["msg10"] = "<script>alert('Post Removed Successfully !');</script>";
            return RedirectToAction("Index", "home");
        }

        // MD_Cust_Request-----------------------DISPLAY REQUESTS----------------
        public PartialViewResult displayMDrequests()
        {


            if (Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var mD_Cust_Request = db.MD_Cust_Request.SqlQuery("select * from MD_Cust_Request where Customer_ID = @cid and Status_ID = 1", new SqlParameter("@cid", cid)).ToList();
                if (mD_Cust_Request.Count() != 0)
                    return PartialView(mD_Cust_Request);
            }
            return PartialView();

        }

        public ActionResult AssignProjectToMD(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
                MD_Cust_Request mD_Cust_Request = db.MD_Cust_Request.Find(id);

                db.Database.ExecuteSqlCommand("update MD_Cust_Request set Status_ID = 2 where ID = @rid", new SqlParameter("@rid", mD_Cust_Request.ID));
                db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where ID != @rid and Project_ID = @pid", new SqlParameter("@rid", mD_Cust_Request.ID), new SqlParameter("@pid", mD_Cust_Request.Project_ID));
                db.SaveChanges();
                Response.Write("<script>alert('Project Assigned To Director Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Project Assigned To Director Successfully !');</script>";

            }

            return RedirectToAction("Index", "home");
        }


        public ActionResult rejectRequest(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {

                db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where ID = @rid", new SqlParameter("@rid", id));
                Response.Write("<script>alert('Request rejected Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request rejected  Successfully !');</script>";
            }

            return RedirectToAction("Index", "home");
        }

        public ActionResult MDdetails(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);


            if (employee == null)
            {
                return HttpNotFound();
            }
            //NumOfProjects
            int NumOfProject = db.Database.SqlQuery<int>("select COUNT(distinct Project_ID) from ProjectModule where MD_ID in (select ID from Employee where ID=@id)", new SqlParameter("@id", id)).FirstOrDefault();
            ViewData["NumProjects"] = NumOfProject;
            return View(employee);
        }



    }
}
