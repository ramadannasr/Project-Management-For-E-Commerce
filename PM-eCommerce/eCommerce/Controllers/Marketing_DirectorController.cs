using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using eCommerce.Models;
using System.Configuration;
using System.Diagnostics;

namespace eCommerce.Controllers
{
    public class Marketing_DirectorController : Controller
    {
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();


        // GET: Marketing_Director
        public ActionResult Index(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id != null)
            {
                Employee MD_Object = db.Employee.Find(id);
                Session["userName"] = MD_Object.UserName;
                Session["userType"] = MD_Object.UserType.ToString();
                Session["id"] = id;
                Session["exper"] = int.Parse(MD_Object.Experience.ToString());

                return View(MD_Object);
            }
            else if (Session["id"] != null)
            {
                int ID = Convert.ToInt32(Session["id"]);
                Employee MD_Object = db.Employee.Find(ID);

                return View(MD_Object);
            }

            Debug.WriteLine("ELSE");

            return RedirectToAction("Index", "home");
        }

        //______________________________________________________________________________________________________________________________________________________________________________________________
        public PartialViewResult ExperinceChart()
        {
            int EID = Convert.ToInt32(Session["id"]);
            int NumOfProject = db.Database.SqlQuery<int>("select COUNT(distinct Project_ID) from ProjectModule where MD_ID in (select ID from Employee where ID=@id)", new SqlParameter("@id", EID)).FirstOrDefault();
            ViewData["NumProjects"] = NumOfProject;
            return PartialView();
        }
        //____________________________________________________________________________________________________________________________________________________________________________________________



        // GET: Marketing_Director/Detaieaves/5
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

        

        // GET: Marketing_Director/Edit/5
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
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 1"), "ID", "UserType", employee.UserType);
            return View(employee);
        }

        // POST: Marketing_Director/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
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
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 1"), "ID", "UserType", employee.UserType);
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

        //___________________________________________________________________________________________
                                                  //projects                                                
        public PartialViewResult Show_All_Projects()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            var Projects_List = db.Project.SqlQuery(" select * from Project where Approval_ID = 2 AND Project.ID not in(select MD_Cust_Request.Project_ID from MD_Cust_Request where MD_Cust_Request.Project_ID = Project.ID  And MD_Cust_Request.Status_ID=2 Or MD_Cust_Request.MD_ID=@mdid)",new SqlParameter("@mdid",mdid)).ToList();
            if (Projects_List.Count() != 0)
                return PartialView(Projects_List);
            else return PartialView();
        }
        //___________________________________________________________________________________________


        public PartialViewResult Show_All_accepted_Projects()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            //var accepted_Projects = db.MD_Cust_Request.SqlQuery("select * from MD_Cust_Request where MD_Cust_Request.MD_ID=@mtid And MD_Cust_Request.Status_ID=2 AND MD_Cust_Request.Project_ID IN(select Project_ID from ProjectModule where ProjectModule.MD_ID=NULL And ProjectModule.MTL_ID=NULL)" new SqlParameter("@pm_id",id)).ToList();
            var accepted_Projects = db.Project.SqlQuery("SELECT *  FROM Project where Project.ID in ( select MD_Cust_Request.Project_ID from MD_Cust_Request where MD_Cust_Request.MD_ID  = @md_id AND MD_Cust_Request.Status_ID = 2)", new SqlParameter("@md_id", mdid)).ToList();
            if (accepted_Projects.Count() != 0)
                return PartialView(accepted_Projects);
            else return PartialView();
        }
        //___________________________________________________________________________________________ 


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send_Cu_MD_Request(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            if (id != null) { 
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
                Session["msg10"] = "<script>alert('Request Has Been Sent Successfully !');</script>";
                //  TempData["msg"] = "<script>alert('Request Has Been Sent Successfully .');</script>";
                return RedirectToAction("Index");
            }
            else { return RedirectToAction("Index"); }
        }
        

        //________________________________________________________________________________
        public ActionResult Create_Team_Module(int? id)
  
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project pro = db.Project.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }
           
          //Debug.WriteLine(pro.ID+"ana fe Create_Team_Module");
            Session["project_Id"] = pro.ID;

            return View();
        }



        //___________________________________________________________________________


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Send_EMP_MD_Request(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            int proid = Convert.ToInt32(Session["project_Id"]);
            if (id != null)
            {
                //Debug.WriteLine(pro.ID + pro.ProjectTitle + pro.Description + "ana project fe Send_EMP_MD_Request");
                //Debug.WriteLine(emp.FirstName + emp.ID+ "ana emp fe Send_EMP_MD_Request");
                //Debug.WriteLine(Convert.ToInt32(Session["id"])+"ana MD ID");
                Employee_Request emp_Request = new Employee_Request();
                emp_Request.Sender_ID = Convert.ToInt32(Session["id"]);
                emp_Request.Reciever_ID = (int)id;
                emp_Request.Status_ID = 1;
                emp_Request.Project_ID = proid;
                db.Employee_Request.Add(emp_Request);
                db.SaveChanges();
                Response.Write("<script>alert('Request Has Been Sent Successfully .');</script>");
                Session["msg10"] = "<script>alert('Request Has Been Sent Successfully !');</script>";
                return RedirectToAction("Create_Team_Module", new { id = proid });
            }
            else return RedirectToAction("Index");
        }
        //____________________________________________________________________________________________________________

        public PartialViewResult Show_Delivered_Projects()
        {
            int mdid =  Convert.ToInt32(Session["id"]);

            var Delivered_Projects = db.ProjectModule.SqlQuery("select * from ProjectModule where ProjectModule.Status = 2 AND ProjectModule.MD_ID= @mid", new SqlParameter("@mid", mdid)).ToList();
            if (Delivered_Projects.Count() != 0)
                return PartialView(Delivered_Projects);
            else return PartialView();
        }
        //_____________________________________________________________________________________________________________

        public PartialViewResult Show_maneged_Projects()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            var managed_Projects = db.ProjectModule.SqlQuery("select * from ProjectModule where ProjectModule.Status = 1 AND ProjectModule.MD_ID= @mid", new SqlParameter("@mid", mdid)).ToList();
            if (managed_Projects.Count() != 0)
                return PartialView(managed_Projects);
            else return PartialView();
        }
        //______________________________________________________________________________________________________________

        // GET: Marketing_Director
        public ActionResult Set_project_Schedule(int? id)
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            //ViewBag.MTL_ID = new SelectList(db.Employees, "ID", "FirstName");
            //ViewBag.MD_ID = new SelectList(db.Employees, "ID", "FirstName");
            var project = db.ProjectModule.FirstOrDefault(u => u.Project_ID == id);
            var project2 = db.Project.Find(id);
            Session["ProId"] = id;
            if (project2 != null)
                ViewData["ProTitle"] = project2.ProjectTitle;


            if (project != null)
            {
                Session["flag"] = id;

                ViewBag.Status = new SelectList(db.ProjectStatus, "ID", "Status");
                return View(project);
            }

            ViewBag.Status = new SelectList(db.ProjectStatus, "ID", "Status");
            return View();
        }
      
        
        // POST: Marketing_Director
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Set_project_Schedule([Bind(Include = "ID,Price,Status,StartDate,EndDate,NoOfHoursPerDay")] ProjectModule projectModule)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            if (ModelState.IsValid)
            {

                if (Session["flag"] != null && Convert.ToInt32(Session["flag"]) == Convert.ToInt32(Session["ProId"]))

                {

                    //db.Entry(projectModule).State = EntityState.Modified;
                    int pid = Convert.ToInt32(Session["flag"]);
                    ProjectModule pro = new ProjectModule();
                    pro.MD_ID = Convert.ToInt32(Session["id"]);
                    pro.NoOfHoursPerDay = projectModule.NoOfHoursPerDay;
                    pro.Price = projectModule.Price;
                    pro.Status = projectModule.Status;
                    pro.StartDate = projectModule.StartDate;
                    pro.EndDate = projectModule.EndDate;
                    pro.Project_ID = pid;

                    db.Database.ExecuteSqlCommand("UPDATE  ProjectModule set Status = @s ,Price = @p ,StartDate = @st ,EndDate = @et ,NoOfHoursPerDay = @noh     where Project_ID =  @pid", new SqlParameter("@s", projectModule.Status), new SqlParameter("@pid", pid), new SqlParameter("@p", projectModule.Price), new SqlParameter("@st", projectModule.StartDate), new SqlParameter("@et", projectModule.EndDate), new SqlParameter("@noh", projectModule.NoOfHoursPerDay));
                    Response.Write("<script>alert('Data Updated Successfully .');</script>");
                    Session["msg10"] = "<script>alert('Data Updated Successfully .');</script>";


                    return RedirectToAction("Index");
                }


                projectModule.MD_ID = Convert.ToInt32(Session["id"]);
                projectModule.Project_ID = Convert.ToInt32(Session["ProId"]);
                db.ProjectModule.Add(projectModule);
                db.SaveChanges();
                Response.Write("<script>alert('Project Schedule Setted Successfully');</script>");
                Session["msg10"] = "<script>alert('Data Updated Successfully .');</script>";
                return RedirectToAction("Index");
            }

            //ViewBag.MTL_ID = new SelectList(db.Employees, "ID", "FirstName", projectModule.MTL_ID);
            //ViewBag.MD_ID = new SelectList(db.Employees, "ID", "FirstName", projectModule.MD_ID);

            projectModule.Project_ID = Convert.ToInt32(Session["ProId"]);
            var project2 = db.Project.Find(projectModule.Project_ID);
            if (project2 != null)
                ViewData["ProTitle"] = project2.ProjectTitle;

            ViewBag.Status = new SelectList(db.ProjectStatus, "ID", "Status", projectModule.Status);
            return View(projectModule);
        }
        //______________________________________________________________________________________________________________________

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
            ProjectModule projectModule = db.ProjectModule.Find(id);
            if (projectModule == null)
            {
                return HttpNotFound();
            }
            return View(projectModule);
        }
        //_______________________________________________________________________________________________________________________________
        //-------------------------------------------------------------------------------------------------------------------------------
                                           //users
                                           //------

        public PartialViewResult Show_All_MTLs()/////////////////////////////////////
        {
            // Debug.WriteLine(prooo.ID+ " ana fe  Show_All_MTLs ");
            int pid = Convert.ToInt32(Session["project_Id"]);
            var MTLs = db.Employee.SqlQuery(" select * from Employee where UserType=2 And ID not In (select Reciever_ID from Employee_Request where Employee_Request.Project_ID = @pid) and ID not in (select WorksOn.Emp_ID from WorksOn where WorksOn.ProjectID =@pid)", new SqlParameter("@pid",pid)).ToList();
            if (MTLs.Count() != 0)
                return PartialView(MTLs);
            else return PartialView();
        }

        public PartialViewResult Show_All_MTs()///////////////////////////////////////
        {
            int pid = Convert.ToInt32(Session["project_Id"]);
            var MTs = db.Employee.SqlQuery("select * from Employee where UserType=3 And ID not In (select Reciever_ID from Employee_Request where Employee_Request.Project_ID = @pid) and ID not in (select WorksOn.Emp_ID from WorksOn where WorksOn.ProjectID =@pid)", new SqlParameter("@pid", pid)).ToList();
            if (MTs.Count() != 0)
                return PartialView(MTs);
            else return PartialView();
        }
        //_______________________________________________________________
        public PartialViewResult MTs_want_to_leave()////////////////////////Updated
        {
            int mdid = Convert.ToInt32(Session["id"]);
            //for every member 
            //var anyMember = db.Employee.SqlQuery(" select * from Employee_Request where  Reciever_ID = @md_id And Status_ID =1 ", new SqlParameter("@md_id", mdid)).ToList();
            // MTs Only
            var MTs = db.Employee_Request.SqlQuery("select * from Employee_Request where  Reciever_ID =@md_id  or Employee_Request.Project_ID in (select MD_Cust_Request.Project_ID from  MD_Cust_Request where MD_Cust_Request.MD_ID = @md_id ) And Status_ID =1 and Employee_Request.Reciever_ID in (select Employee.ID from Employee where Employee.UserType = 2)", new SqlParameter("@md_id", mdid)).ToList();
         
            if (MTs.Count() != 0)
            return PartialView(MTs);

            else return PartialView();

        }
  
        //______________________________________________________________\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public ActionResult MTL_foreachproModule(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            Session["pid"] = id;
            var MTL = db.Employee.SqlQuery("SELECT * FROM Employee  where Employee.UserType = 2 and Employee.ID in (select WorksOn.Emp_ID from WorksOn where WorksOn.ProjectID = @pid)", new SqlParameter("@pid",id)).ToList();
            // Debug.WriteLine(MTL.First().ID+ "ana fe MTL_foreachproModule");
        if(MTL.Count()!=0)
            return View(MTL);
        else
                return View();
        }
        //______________________________________________________________\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        public ActionResult List_MTs_foreachproModule(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            Session["pid"] = id;
            var MTs = db.Employee.SqlQuery("SELECT * FROM Employee  where Employee.UserType = 3 and Employee.ID in (select WorksOn.Emp_ID from WorksOn where WorksOn.ProjectID = @pid)", new SqlParameter("@pid", id)).ToList();
            if (MTs.Count() != 0)
                return PartialView(MTs);
            else
                return View();
        }
        //_______________________________________________________________\\\\\\\\\\UPDATED
        public PartialViewResult Show_All_accepted_Request_MTL_and_MT()
        {
            int mdid = Convert.ToInt32(Session["id"]);
            var MTL_Requests = db.Employee_Request.SqlQuery("select * from Employee_Request where Status_ID=2 AND Employee_Request.Project_ID in (select MD_Cust_Request.Project_ID from MD_Cust_Request where MD_Cust_Request.MD_ID =@md_id and MD_Cust_Request.Status_ID = 2 )", new SqlParameter("@md_id",mdid)).ToList();

            if (MTL_Requests.Count() != 0)
                return PartialView(MTL_Requests);
            else return PartialView();
        }


        //_____________________________________________________________________//////////////////////////////////
                                           //Confirmed

        [HttpPost, ActionName("Leave_Project")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteprojectModuleConfirmed(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            ProjectModule projectModule = db.ProjectModule.Find(id);
            if(projectModule != null)
            { 
            int pid = projectModule.Project_ID;
            db.Database.ExecuteSqlCommand("delete from Employee_Request where Project_ID = @pid",new SqlParameter("@pid",pid));
            db.Database.ExecuteSqlCommand("delete from WorksOn where ProjectID = @pid", new SqlParameter("@pid", pid));
            db.Database.ExecuteSqlCommand("delete from MT_Evaluation where Project_ID = @pid", new SqlParameter("@pid", pid));
            db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where Project_ID = @pid", new SqlParameter("@pid", pid));
            db.ProjectModule.Remove(projectModule);
            db.SaveChanges();
                Response.Write("<script>alert('You Leaved Project Successfully .');</script>");
                Session["msg10"] = "<script>alert('You Leaved Project Successfully !');</script>";
                return RedirectToAction("Index");
            }
            else
            {
                int pid = id;
                db.Database.ExecuteSqlCommand("delete from Employee_Request where Project_ID = @pid", new SqlParameter("@pid", pid));
                db.Database.ExecuteSqlCommand("delete from WorksOn where ProjectID = @pid", new SqlParameter("@pid", pid));
                db.Database.ExecuteSqlCommand("delete from MT_Evaluation where Project_ID = @pid", new SqlParameter("@pid", pid));
                db.Database.ExecuteSqlCommand("delete from MD_Cust_Request where Project_ID = @pid", new SqlParameter("@pid", pid));
                Response.Write("<script>alert('You Leaved Project Successfully .');</script>");
                Session["msg10"] = "<script>alert('You Leaved Project Successfully !');</script>";
                return RedirectToAction("Index");
            }
        }
    

        //______________________________________________
        [HttpPost, ActionName("remove_project_member")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletemtFAPConfirmed(int id)////////////////////////////////////////////
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            int pid = Convert.ToInt32(Session["pid"]);
            db.Database.ExecuteSqlCommand("delete from WorksOn where WorksOn.Emp_ID =@id   AND WorksOn.ProjectID =@pid ", new SqlParameter("@pid", pid), new SqlParameter("@id", id));
            db.Database.ExecuteSqlCommand("delete from Employee_Request where Reciever_ID = @memid and Project_ID = @pid ", new SqlParameter("@pid", pid), new SqlParameter("@memid", id));
            Response.Write("<script>alert('Member Removed Successfully .');</script>");
            Session["msg10"] = "<script>alert('Member Removed Successfully !');</script>";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult approveMemberLeaving(int id)////////////////////////////////////////////
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            Employee_Request req = db.Employee_Request.Find(id);
         
            db.Database.ExecuteSqlCommand("delete from WorksOn where WorksOn.Emp_ID =@id   AND WorksOn.ProjectID =@pid ", new SqlParameter("@pid", req.Project_ID), new SqlParameter("@id", req.Sender_ID));
            db.Database.ExecuteSqlCommand("delete from Employee_Request where Reciever_ID = @memid and Project_ID = @pid ", new SqlParameter("@pid", req.Project_ID), new SqlParameter("@memid", req.Sender_ID));
            db.Database.ExecuteSqlCommand("delete from Employee_Request where Sender_ID = @memid and Project_ID = @pid ", new SqlParameter("@pid", req.Project_ID), new SqlParameter("@memid", req.Sender_ID));
            Response.Write("<script>alert('Member Removed Successfully .');</script>");
            Session["msg10"] = "<script>alert('Member Removed Successfully !');</script>";
            return RedirectToAction("Index");
        }

        //________________________________________________________________
        [HttpPost, ActionName("Assign_Emp_To_Project")]
        [ValidateAntiForgeryToken]
        public ActionResult Assign_Pro_Emp_Confirmed(int id)///\\\\\\\\\\UPDATED--------------------------------
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            Employee_Request req = db.Employee_Request.Find(id);
            WorksOn pro = new WorksOn();
            pro.Emp_ID = req.Reciever_ID;
            pro.ProjectID = req.Project_ID;
            db.WorksOn.Add(pro);
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("update Employee_Request set Employee_Request.Status_ID = 3 Where ID = @rid", new SqlParameter("@rid", id));
            Response.Write("<script>alert('Member Added Successfully .');</script>");
            Session["msg10"] = "<script>alert('Member Added Successfully !');</script>";
            //db.Employee_Request.Remove(req);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }
        //--------------------------------------


        public ActionResult Show_All_MT_Evaluation_For_Each_Project(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            //Debug.WriteLine(id + "ana fe Show_All_MT_Evaluation_For_Each_Project");
            var MTs_Eva = db.MT_Evaluation.SqlQuery("Select * From MT_Evaluation Where Project_ID=@p_id", new SqlParameter("@p_id", id)).ToList();
            if (MTs_Eva.Count() != 0)
                return View(MTs_Eva);
            else
                return View();
        }

    }
}
