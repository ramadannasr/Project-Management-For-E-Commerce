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
using System.Configuration;
using System.Diagnostics;

namespace eCommerce.Controllers
{
    public class AdminController : Controller
    {
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();
        

        // GET: Admin
        public ActionResult Index(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
            Admin ad = db.Admin.Find(id);
            Session["userName"] = ad.UserName;
            Session["userType"] = ad.UserType.ToString();
            Session["id"] = id;
                return View(ad);
            }
           else if(Session["id"] != null)
            {
               int ID = Convert.ToInt32(Session["id"]);
                Admin ad = db.Admin.Find(ID);
                return View(ad);
            }


            return RedirectToAction("Index", "home");
        }

        // GET: Admin/Details/5
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
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }


        // GET: home/Create
        public ActionResult AddNewUser(int? type)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (type != null)
            {
                Session["type"] = "cust";
                ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 5"), "ID", "UserType");
                return View();
            }

               

            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID not in (4,5)"), "ID", "UserType");

            return View();
        }

        // POST: home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewUser([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,Experience,UserType,UserName,Password")] Employee employee, HttpPostedFileBase image)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

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
                        Response.Write("<script>alert('This UserName Is Not Available.');</script>");
                        //TempData["msg10"] = "<script>alert('This UserName Is Not Available.');</script>";
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
                Response.Write("<script>alert('"+employee.FirstName + " " + employee.LastName + " Added Successfully.');</script>");
                //Session["Message"] = employee.FirstName + " " + employee.LastName + " Added Successfully.";
                return RedirectToAction("Index");
            }
            else {
                if (image == null)
                    Response.Write("<script>alert('Please Select Photo.');</script>");

            if(Session["type"] != null)
            {
                ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID =5"), "ID", "UserType", employee.UserType);
                return View(employee);
            }
            
            else

                ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID not in (4,5)"), "ID", "UserType", employee.UserType);
            return View(employee);
            }
        }

        // GET: Admin/Edit/5
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
            Admin admin = db.Admin.Find(id);
            Session["photo"] = admin.Photo;
            if (admin == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 4"), "ID", "UserType", admin.UserType);
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,UserName,Password,UserType")] Admin admin, HttpPostedFileBase image)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (ModelState.IsValid )
            {
                if (image != null)
                {
                    admin.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(admin.Photo, 0, image.ContentLength);
                }
                else
                {

                    admin.Photo = (byte[])Session["photo"];
                }
              

                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<script>alert('Data Edited Successfully .');</script>");
                
                return RedirectToAction("Index");
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 4"), "ID", "UserType", admin.UserType);
            return View(admin);
        }

      
        //-------------------------------POSTS----------------------------------------------------
        public PartialViewResult ListAllPosts()
        {
            var Projects = db.Project.SqlQuery("select * from Project where Approval_ID = 1");

            if(Projects.ToList().Count() !=0)
                 return PartialView(Projects.ToList());


            return PartialView();
        }

        public ActionResult ApproveProject(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            db.Database.ExecuteSqlCommand("update Project set Approval_ID = 2 where Project.ID = @id ", new SqlParameter("@id", id));
            db.SaveChanges();
            Response.Write("<script>alert('Project Approved Successfully .');</script>");
            TempData["msg10"] = "<script>alert('Project Approved Successfully !');</script>";
            return RedirectToAction("Index");
        }


        public ActionResult EditPost(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project post = db.Project.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.Approval_ID = new SelectList(db.RequestStatus, "ID", "Status", post.Approval_ID);

            return View(post);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost([Bind(Include = "ID,Description,Approval_ID,Customer_ID,ProjectTitle")] Project project)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<script>alert('Project Edited Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Project Edited Successfully !');</script>";
                return RedirectToAction("Index");
            }
           
            ViewBag.Approval_ID = new SelectList(db.RequestStatus, "ID", "Status", project.Approval_ID);
            return View(project);
        }
       
        // POST: Admin/Delete/5
        [HttpPost, ActionName("DeletePost")]
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
            return RedirectToAction("Index");
        }

        //------------------------CURRENT PROJECTS---------------------------------
        public PartialViewResult ListCurrentProjects()
        {
            var Projects = db.MD_Cust_Request.SqlQuery("select * from MD_Cust_Request where  MD_Cust_Request.Status_ID=2 and MD_Cust_Request.Project_ID not in(select ProjectModule.Project_ID from ProjectModule where ProjectModule.Status = 2)");
            if (Projects.ToList().Count != 0)
                return PartialView(Projects.ToList());
            return PartialView();
        }
      
        //------------------------Delivered PROJECTS---------------------------------
        public PartialViewResult ListDeliveredProjects()
        {
            var Projects = db.ProjectModule.SqlQuery("select * from ProjectModule where ProjectModule.Status = 2");
            if(Projects.ToList().Count !=0)
                return PartialView(Projects.ToList());
            return PartialView();
        }
   
        // GET: ProjectModules/Details/5
        public ActionResult ProjectDetails(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectModule projectModule = db.ProjectModule.FirstOrDefault(u => u.Project_ID == id);

           
            if (projectModule == null)
            {
                return View();
            }
            return View(projectModule);
        }
       
        //--------------------USERS---------------------------------------------
        public PartialViewResult ListAllUsers()
        {
            var employees = db.Employee.Include(e => e.EmployeeType);
            if (employees.ToList().Count() != 0)

                return PartialView(employees.ToList());

            else return PartialView();

        }

        public PartialViewResult ListAllCustomers()
        {
            var customers = db.Customer.Include(c => c.EmployeeType);
            if (customers.ToList().Count() != 0)

                return PartialView(customers.ToList());
            else return PartialView();
        }
     
        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            Employee emp = db.Employee.Find(id);
            if(emp != null)
            {
                if(emp.UserType == 1)
                {
                    db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where MD_ID = @mid", new SqlParameter("@mid", emp.ID));
                    db.Database.ExecuteSqlCommand("delete from Employee_Request where Sender_ID = @mid or Reciever_ID = @mid", new SqlParameter("@mid", emp.ID));
                    
                   // var projects = db.ProjectModule.SqlQuery("select * from ProjectModule where MD_ID = @mid",new SqlParameter("@mid",emp.ID)).ToList();
                   // if (projects.Count() != 0)
                   // {
                      //  foreach (var pro in projects)
                       // {
                           // db.Database.ExecuteSqlCommand("delete from WorksOn where ProjectID = @pid", new SqlParameter("@pid", pro.Project_ID));
                       // }
                        db.Database.ExecuteSqlCommand("delete from ProjectModule where MD_ID = @mid", new SqlParameter("@mid", emp.ID));

                        
                        
                   // }
                   
                    
                    
                }
                else if(emp.UserType == 2) 
                    {

                    db.Database.ExecuteSqlCommand("delete from Employee_Request where Sender_ID = @mid or Reciever_ID = @mid", new SqlParameter("@mid", emp.ID));

                    db.Database.ExecuteSqlCommand("delete from WorksOn where Emp_ID = @mid", new SqlParameter("@mid", emp.ID));
                   
                    }

                else if(emp.UserType == 3)
                {
                    db.Database.ExecuteSqlCommand("delete from WorksOn where Emp_ID = @mid", new SqlParameter("@mid", emp.ID));
                    db.Database.ExecuteSqlCommand("delete from MT_Evaluation where MT_ID = @mid", new SqlParameter("@mid", emp.ID));
                    db.Database.ExecuteSqlCommand("delete from Employee_Request where Sender_ID = @mid or Reciever_ID = @mid", new SqlParameter("@mid", emp.ID));
                }
                    

                db.Employee.Remove(emp);
                db.SaveChanges();
                Response.Write("<script>alert('User Removed Successfully .');</script>");
                TempData["msg10"] = "<script>alert('User Removed Successfully !');</script>";
            }
           else
            {
               Customer cu = db.Customer.Find(id);
                if(cu != null)
                {
                    var projects = db.Project.SqlQuery("select * from Project where Customer_ID = @mid", new SqlParameter("@mid", cu.ID)).ToList();
                    if (projects.Count() != 0)
                    {
                        
                        db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where Customer_ID = @mid", new SqlParameter("@mid", cu.ID));
                        
                        foreach (var pro in projects)
                        {
                            db.Database.ExecuteSqlCommand("delete from Employee_Request where Project_ID = @mid", new SqlParameter("@mid", pro.ID));
                            db.Database.ExecuteSqlCommand("delete from WorksOn where ProjectID = @pid", new SqlParameter("@pid", pro.ID));
                              
                            db.Database.ExecuteSqlCommand("delete from ProjectModule where ProjectModule.Project_ID = @mid", new SqlParameter("@mid", pro.ID));

                            db.Project.Remove(pro);
                        }

                        
                        
                    }


                    
                    db.Customer.Remove(cu);
                    db.SaveChanges();
                    Response.Write("<script>alert('User Removed Successfully .');</script>");
                    TempData["msg10"] = "<script>alert('User Removed Successfully !');</script>";
                }

            }
            
            return RedirectToAction("Index");
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
