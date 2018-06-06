using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eCommerce.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace eCommerce.Controllers
{
    public class CustomerController : Controller
    {
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();

        // GET: Customer
        public ActionResult Index(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
                Customer customer = db.Customer.Find(id);
                Session["userName"] = customer.UserName;
                Session["userType"] = customer.UserType.ToString();
                Session["id"] = id;
                return View(customer);
            }
            else if (Session["id"] != null)
            {
                int ID = Convert.ToInt32(Session["id"]);
                Customer customer = db.Customer.Find(ID);
                return View(customer);
            }


            return RedirectToAction("Index","home");
        }

        // GET: Customer/Details/5
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
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }


        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            Session["photo"] = customer.Photo;
            
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 5"), "ID", "UserType", customer.UserType);
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Description,Mobile,Email,Photo,UserName,Password,UserType")] Customer customer, HttpPostedFileBase image)
        {


            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (ModelState.IsValid  )
            {
                
                if (image != null)
                {
                    customer.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(customer.Photo, 0, image.ContentLength);
                }else
                {
                    
                    customer.Photo = (byte[])Session["photo"];
                }

                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
               Response.Write("<script>alert('Data Edited Successfully .');</script>");
                return RedirectToAction("Index");
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 5"), "ID", "UserType", customer.UserType);
            return View(customer);
        }

        //// GET: Customer/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Customer customer = db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customer/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Customer customer = db.Customers.Find(id);
        //    db.Customers.Remove(customer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //------------------------CURRENT PROJECTS---------------------------------
        public PartialViewResult ListCurrentProjects()
        {
            
            if (Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var Projects = db.MD_Cust_Request.SqlQuery("select * from MD_Cust_Request where  MD_Cust_Request.Customer_ID = @cid and MD_Cust_Request.Status_ID=2 and MD_Cust_Request.Project_ID not in(select ProjectModule.Project_ID from ProjectModule where ProjectModule.Status = 2)", new SqlParameter("@cid", cid)).ToList();
                if (Projects.Count() != 0)
                {
                    return PartialView(Projects);

                 
                }
            }
            
        return PartialView();
        }
        //------------------------Delivered PROJECTS---------------------------------


        public PartialViewResult ListDeliveredProjects()
        {

            var deliveredProjects = new List<ProjectModule>();
            if (Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var Projects = db.Project.SqlQuery("select * from Project where Project.Customer_ID = @cid", new SqlParameter("@cid", cid)).ToList();
                
                if (Projects.Count() != 0)
                {

                    foreach (var pro in Projects)
                    {

                        var projectModule = db.ProjectModule.FirstOrDefault(pm => pm.Project_ID == pro.ID && pm.Status == 2 );
                       
                        if (projectModule != null)
                            deliveredProjects.Add(projectModule);
                    }
                    if(deliveredProjects.Count()!=0)
                    return PartialView(deliveredProjects);
                }
            }

            return PartialView();

        }

        //-------------------ADD PROJECT--------------------------
        // GET: Projects/Create
        public ActionResult addProject()
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addProject([Bind(Include = "ID,Description,ProjectTitle")] Project project)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

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
                return RedirectToAction("Index");
            }


            return View(project);
        }

        public PartialViewResult viewProjects()
        {
            
            if (Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var Projects = db.Project.SqlQuery("select * from Project where Project.Customer_ID = @cid and Project.ID not in (select MD_Cust_Request.Project_ID from MD_Cust_Request where Project.ID = MD_Cust_Request.Project_ID and MD_Cust_Request.Status_ID = 2)", new SqlParameter("@cid", cid)).ToList();
                if (Projects.Count() != 0)
                {

                    return PartialView(Projects);

                }
            }

            return PartialView();

        }

        // GET: Projects/Edit/5
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
                return RedirectToAction("Index");
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
            Response.Write("<script>alert('Project Removed Successfully .');</script>");
            TempData["msg10"] = "<script>alert('Project Removed Successfully !');</script>";
            return RedirectToAction("Index");
        }

        // MD_Cust_Request-----------------------DISPLAY REQUESTS----------------
        public PartialViewResult displayMDrequests()
        {

            if(Session["id"] != null)
            {
                int cid = Convert.ToInt32(Session["id"]);
                var mD_Cust_Request = db.MD_Cust_Request.SqlQuery("select * from MD_Cust_Request where Customer_ID = @cid and Status_ID = 1", new SqlParameter("@cid",cid)).ToList();
                if(mD_Cust_Request.Count()!=0)
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
                db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where ID != @rid and Project_ID = @pid", new SqlParameter("@rid", mD_Cust_Request.ID),new SqlParameter("@pid", mD_Cust_Request.Project_ID));
                db.SaveChanges();
                Response.Write("<script>alert('Project Assigned To Director Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Project Assigned To Director Successfully !');</script>";

            }

            return RedirectToAction("Index","Customer");
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

            return RedirectToAction("Index", "Customer");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
