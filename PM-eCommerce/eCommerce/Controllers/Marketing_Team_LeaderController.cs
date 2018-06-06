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
    public class Marketing_Team_LeaderController : Controller
    {
        
        private ECOMMERCEEntities2 db = new ECOMMERCEEntities2();
      

        // GET: Marketing_Team_Leader
        public ActionResult Index(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }
           
            
            //if (Session["LogOut"] != null)
            //{
            //    return RedirectToAction("Index", "home");
            //}

            

            if (id != null)
            {
                Employee ad = db.Employee.Find(id);
                Session["userName"] = ad.UserName;
                Session["userType"] = ad.UserType.ToString();
                Session["id"] = id;
                Session["exper"] = int.Parse(ad.Experience.ToString());
                return View(ad);
            }
            else if (Session["id"] != null)
            {
                int ID = Convert.ToInt32(Session["id"]);
                Employee ad = db.Employee.Find(ID);
                return View(ad);
            }
           

            return RedirectToAction("Index","home");
        }

    
        
        //______________________________________________________________________________________________________________

        public PartialViewResult ExperinceChart()
        {
            int EID = Convert.ToInt32(Session["id"]);
            int NumOfProject = db.Database.SqlQuery<int>("select count(distinct ProjectID) from WorksOn where Emp_ID=@pid", new SqlParameter("@pid", EID)).FirstOrDefault();

            Session["NumProjects"] = NumOfProject;
            return PartialView();
        }
        //_______________________________________________________________________________________________________________

        
        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            //if (Session["userName"] == null)
            //{
            //    return RedirectToAction("Index", "home");
            //}


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee mtl = db.Employee.Find(id);
            Session["photo"] = mtl.Photo;
            if (mtl == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 2"), "ID", "UserType", mtl.UserType);
            return View(mtl);
        }

     
        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,JobDescription,Mobile,Email,Photo,Experience,UserName,Password,UserType")] Employee mtl, HttpPostedFileBase image)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    mtl.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(mtl.Photo, 0, image.ContentLength);
                }
                else
                {

                    mtl.Photo = (byte[])Session["photo"];
                }


                db.Entry(mtl).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<script>alert('Data Edited Successfully .');</script>");
                return RedirectToAction("Index");
            }
            ViewBag.UserType = new SelectList(db.EmployeeType.SqlQuery("select * from EmployeeType where ID = 2"), "ID", "UserType", mtl.UserType);
            return View(mtl);
        }

      
        
        //*******************************************************************************************************
        //------------------------------------TRAINEE----------------------------------------
        public ActionResult LISTALLTRAINEES(int pmID)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "home");
            }


            int ID = Convert.ToInt32(Session["id"]);
            Session["pmid"] = pmID;
            var employee = db.Employee.SqlQuery("select * from Employee where Employee.ID not in (select Employee_Request.Reciever_ID from Employee_Request where  Employee_Request.Project_ID  = @pid) and Employee.UserType = 3", new SqlParameter("@pid", pmID)).ToList();
            if (employee.Count() != 0)
                return View(employee);
            else
                ViewData["mess"] = "There Is no Trainees Found";

                return View();
        }



        public PartialViewResult LISTALLCURRENTTRAINEES()
        {
            int ID = Convert.ToInt32(Session["id"]);

            var employee = db.WorksOn.SqlQuery("select * from WorksOn where  WorksOn.ProjectID in (select WorksOn.ProjectID from WorksOn where WorksOn.Emp_ID = @tlid ) and WorksOn.Emp_ID in (select Employee.ID from Employee where Employee.UserType = 3)",new SqlParameter("@tlid",ID)).ToList();
            if (employee.Count() != 0)
                return PartialView(employee);
            else return PartialView();
        }




        ////GET marketing team laeder evaluate
        //public ActionResult EVALUATETRAINEE(int tid,int pid)
        //{
        //    Session["tid"] = tid;
        //    Session["pid"] = pid;
        //    Debug.WriteLine("From EVALUATETRAINEE : " + Session["tid"].ToString()+ " =>"+Session["pid"].ToString());
        //    MT_Evaluation evaluation = db.MT_Evaluation.FirstOrDefault(e => e.MT_ID == tid && e.Project_ID == pid);
        //    if (evaluation != null)
        //        return View(evaluation);
        //    //  ViewBag.UserType = new SelectList(db.EmployeeTypes.SqlQuery("select distinct(MTL_ID) from EmployeeType where ID = 3"), "ID", "UserType");
        //    return View();
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EVALUATETRAINEE(int id, String textbox)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            //if (Session["userName"] == null)
            //{
            //    return RedirectToAction("Index", "home");
            //}

            Debug.WriteLine("string : "+textbox+" ID : "+id);
            WorksOn mt = db.WorksOn.Find(id);
            Debug.WriteLine("pro : " + mt.ProjectID + " ID : " + mt.Emp_ID);
            var IsEvaluated = db.MT_Evaluation.FirstOrDefault(u => u.MT_ID == mt.Emp_ID && u.Project_ID == mt.ProjectID);
            if (IsEvaluated != null)
            {
                db.Database.ExecuteSqlCommand("update MT_Evaluation set FeedBack = '"+textbox+"'  where Project_ID = @pid and MT_ID = @mtid", new SqlParameter("@pid", mt.ProjectID),new SqlParameter("@mtid",mt.Emp_ID));
                Response.Write("<script>alert('Evaluated Successfully .');</script>");
                return RedirectToAction("Index", "Marketing_Team_Leader");

            }
            MT_Evaluation feedback = new MT_Evaluation();
            feedback.MT_ID = mt.Emp_ID;
                feedback.Project_ID = mt.ProjectID;
            feedback.FeedBack = textbox;
                db.MT_Evaluation.Add(feedback);
                db.SaveChanges();
            Response.Write("<script>alert('Evaluated Successfully .');</script>");
                return RedirectToAction("Index", "Marketing_Team_Leader");
            

            // ViewBag.UserType = new SelectList(db.EmployeeTypes.SqlQuery("select * from EmployeeType where ID = 4"), "ID", "UserType");
       
        }



        [HttpPost ]
        [ValidateAntiForgeryToken]
        public ActionResult confirmDeletTrainee(int id)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }


            //if (Session["userName"] == null)
            //{
            //    return RedirectToAction("Index", "home");
            //}


            WorksOn employee = db.WorksOn.Find(id);

            db.Database.ExecuteSqlCommand("delete from Employee_Request where Employee_Request.Reciever_ID = @tid and Employee_Request.Project_ID = @pid", new SqlParameter("@tid",employee.Emp_ID),new SqlParameter("@pid",employee.ProjectID));
            db.WorksOn.Remove(employee);
            
            db.SaveChanges();
            TempData["msg10"] = "<script>alert('Trainee Removed Successfully .');</script>";
            Response.Write("<script>alert('Trainee Removed Successfully .');</script>");
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sendRequest(int tid)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            int ID = Convert.ToInt32(Session["id"]);
               Employee_Request Request = new Employee_Request();
                Request.Sender_ID = ID;
                Request.Reciever_ID = tid;
                Request.Status_ID = 1;
                Request.Project_ID = Convert.ToInt32(Session["pmid"]);
            
                db.Employee_Request.Add(Request);
                db.SaveChanges();
            Response.Write("<script>alert('Request sent Successfully .');</script>");
            TempData["msg10"] = "<script>alert('Request sent  Successfully !');</script>";
            return RedirectToAction("LISTALLTRAINEES",new { pmID = Request.Project_ID });
          
        }



        // public PartialViewResult  
        //*************************************************************
        //***********************************projects******************
        public PartialViewResult LISTALLCURRENTPROJECTS(int? id)
        {

            int ID = Convert.ToInt32(Session["id"]);
            var projects = db.WorksOn.SqlQuery("select * from WorksOn where WorksOn.Emp_ID = @tlid and WorksOn.ProjectID not in (select ProjectModule.Project_ID from ProjectModule where Status =2)", new SqlParameter("@tlid", ID)).ToList();
            if (projects.Count() != 0)
                return PartialView(projects);

            else return PartialView();
        }


        public PartialViewResult LISTALLDELEVIREDPROJECTS(int? id)
        {
            int ID = Convert.ToInt32(Session["id"]);
            var projects = db.ProjectModule.SqlQuery("select * from ProjectModule where ProjectModule.Status =2 and ProjectModule.Project_ID in (select WorksOn.ProjectID from WorksOn  where WorksOn.Emp_ID = @tlid)", new SqlParameter("@tlid",ID)).ToList();
            if (projects.Count() != 0)
                return PartialView(projects);
            else return PartialView();
        }


        //****************************************MD**************************
       public PartialViewResult listRequestMD()
        {
            int ID = Convert.ToInt32(Session["id"]);
            var requests = db.Employee_Request.SqlQuery("select * from Employee_Request where Reciever_ID = @tlid and Status_ID = 1", new SqlParameter("@tlid", ID)).ToList();
            if (requests.Count() != 0)
                return PartialView(requests);

            else return PartialView();
        }

        

        public ActionResult acceptRequest(int? id)//\\\\\\\\\\UPDATED
        {

            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (Session["id"] != null)
            {
                db.Database.ExecuteSqlCommand("update Employee_Request set Status_ID = 2 where ID = @id", new SqlParameter("@id", id));
                Response.Write("<script>alert('Request accepted Successfully .');</script>");
                TempData["msg10"] = "<script>alert('Request accepted  Successfully !');</script>";
                //Employee_Request req = db.Employee_Request.Find(id);
                //db.Database.ExecuteSqlCommand("delete from Employee_Request where Reciever_ID = @mtid and Project_ID = @pid and Status_ID !=2", new SqlParameter("@mtid", req.Reciever_ID), new SqlParameter("@pid", req.Project_ID));
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "home");
        }



        public ActionResult rejectRequest( int? id)
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

            return RedirectToAction("Index", "Marketing_Team_Leader");
        }


        //********************************************************************

        // GET: Marketing_Team_Leader/Details/5
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leaveProject(int id)//\\\\\\\\\\UPDATED
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            if (Session["id"] != null )
            {

                int mtlid = Convert.ToInt32(Session["id"]);
                Employee_Request req = db.Employee_Request.FirstOrDefault(u => u.Project_ID == id && u.Reciever_ID == mtlid);
                db.Database.ExecuteSqlCommand("delete from WorksOn Where WorksOn.Emp_ID = @mtl_id and WorksOn.ProjectID = @pid", new SqlParameter("@mtl_id", mtlid), new SqlParameter("@pid", req.Project_ID));
                //Employee_Request newreq = new Employee_Request();
                //newreq.Sender_ID = mtlid;
                //newreq.Reciever_ID = req.Sender_ID;
                //newreq.Project_ID = id;
                //newreq.Status_ID = 1;

                //db.Employee_Request.Add(newreq);
                //db.SaveChanges();

                db.Employee_Request.Remove(req);

                db.SaveChanges();


                Response.Write("<script>alert('You Leaved Project Successfully .');</script>");
                TempData["msg10"] = "<script>alert('You Leaved Project Successfully !');</script>";
            }


            return RedirectToAction("Index");
        }



        public ActionResult Message(String mess)
        {
            if (!homeController.Authenticated)
            {

                return RedirectToAction("Index", "home");
            }

            ViewData["mess"] = mess;
            return View();
          
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
