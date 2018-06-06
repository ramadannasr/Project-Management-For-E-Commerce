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
    public class Marketing_TraineeController : Controller
    {
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();

        // GET: Marketing_Trainee
        public ActionResult Index(int? id)
        {
            if(!homeController.Authenticated)
            {
           
                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
                Employee mt = db.Employee.Find(id);
                Session["userName"] = mt.UserName;
                Session["userType"] = mt.UserType.ToString();
                Session["id"] = id;
                Session["exper"] = int.Parse(mt.Experience.ToString());

                return View(mt);
            }
            else if (Session["id"] != null)
            {
                int ID = Convert.ToInt32(Session["id"]);
                Employee mt = db.Employee.Find(ID);
                return View(mt);
            }


            return RedirectToAction("Index", "home");
        }

      
        //___________________________________________________________________________________________________________________
        public PartialViewResult ExperinceChart()
        {
            int EID = Convert.ToInt32(Session["id"]);
            int NumOfProject = db.Database.SqlQuery<int>("select count(distinct ProjectID) from WorksOn where Emp_ID=@pid", new SqlParameter("@pid", EID)).FirstOrDefault();

            Session["NumProjects"] = NumOfProject;
            return PartialView();
        }

        //___________________________________________________________________________________________________________________

    
       // GET: Marketing_Trainee/Details/5
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
            Employee employee = db.Employee.Find(id);
            Session["photo"] = employee.Photo;
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 3"), "ID", "UserType", employee.UserType);
            return View(employee);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,Experience,UserType,UserName,Password")] Employee employee, HttpPostedFileBase image)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    employee.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(employee.Photo, 0, image.ContentLength);
                }
                else
                {

                    employee.Photo = (byte[])Session["photo"];
                }


                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<script>alert('Data Edited Successfully .');</script>");
                return RedirectToAction("Index");
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 3"), "ID", "UserType", employee.UserType);
            return View(employee);
        }

   
        // POST: Marketing_Trainee/leave project/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leaveProject(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (Session["id"] != null)
            {

                int mtid = Convert.ToInt32(Session["id"]); 
                Employee_Request req = db.Employee_Request.FirstOrDefault(u => u.Project_ID == id && u.Reciever_ID == mtid);
                Employee_Request newreq = new Employee_Request();
                newreq.Sender_ID = mtid;
                newreq.Reciever_ID = req.Sender_ID;
                newreq.Project_ID = id;
                newreq.Status_ID = 1;
                Debug.WriteLine("dddd"+newreq.ToString());
                db.Employee_Request.Add(newreq);
                db.SaveChanges();
                Response.Write("<script>alert('Request To Director Sent Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request To Director Sent Successfully !');</script>";
            }
            
            
            return RedirectToAction("Index");
        }
        //--------------------MD and MTL Requests-------------------


        public PartialViewResult displayRequests()
        {

            if (Session["id"] != null)
            {
                int mtid = Convert.ToInt32(Session["id"]);
                var employeeRequest = db.Employee_Request.SqlQuery("select * from Employee_Request where Reciever_ID = @mtid and Status_ID =1", new SqlParameter("@mtid", mtid)).ToList();
                if (employeeRequest.Count() != 0)
                {
                  //  Debug.WriteLine("num of req : " + employeeRequest.Count()+"ID :"+employeeRequest.ToList().ElementAt(1).Sender_ID);
                    return PartialView(employeeRequest);
                }
                    
            }


            return PartialView();
        }
        //-----------------------------acceptRequest--------------------UPDATED


        public ActionResult acceptRequest(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (Session["id"] != null)
            {
                db.Database.ExecuteSqlCommand("update Employee_Request set Status_ID = 2 where ID = @id", new SqlParameter("@id",id));
                //Employee_Request req = db.Employee_Request.Find(id);
                Response.Write("<script>alert('Request accepted Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request accepted  Successfully !');</script>";
                //db.Database.ExecuteSqlCommand("delete from Employee_Request where Reciever_ID = @mtid and Project_ID = @pid and Status_ID !=2",new SqlParameter("@mtid",req.Reciever_ID),new SqlParameter("@pid",req.Project_ID));
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index","home");
        }
    
        
        //-----------------------------rejectRequest--------------------
        public ActionResult rejectRequest(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {

                db.Database.ExecuteSqlCommand("delete from Employee_Request where ID = @rid", new SqlParameter("@rid", id));
                Response.Write("<script>alert('Request rejected Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request rejected  Successfully !');</script>";
            }

            return RedirectToAction("Index", "Marketing_Trainee");
        }



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
            Project projectModule = db.Project.FirstOrDefault(u => u.ID == id);


            if (projectModule == null)
            {
                return View();
            }
            return View(projectModule);
        }
      
        
        //---------------------All Current Projects-----------------------
        public PartialViewResult ListCurrentProjects()
        {

            if (Session["id"] != null)
            {
                int tid = Convert.ToInt32(Session["id"]);
                var Projects = db.WorksOn.SqlQuery("select * from WorksOn where WorksOn.Emp_ID =@tid ", new SqlParameter("@tid", tid)).ToList();
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
                int mtid = Convert.ToInt32(Session["id"]);
                var Projects = db.WorksOn.SqlQuery("select * from WorksOn where WorksOn.Emp_ID = @mtid", new SqlParameter("@mtid", mtid)).ToList();

                if (Projects.Count() != 0)
                {

                    foreach (var pro in Projects)
                    {

                        var projectModule = db.ProjectModule.FirstOrDefault(pm => pm.Project_ID == pro.ID && pm.Status == 2);

                        if (projectModule != null)
                            deliveredProjects.Add(projectModule);
                    }
                    if (deliveredProjects.Count() != 0)
                        return PartialView(deliveredProjects);
                }
            }

            return PartialView();

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
