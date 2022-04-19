using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        protected Team19LMSContext db;

        public CommonController()
        {
            db = new Team19LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */

        public void UseLMSContext(Team19LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            using(db)
            {
                var query = from i in db.Departments select new { name= i.Name, subject= i.Subject };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            using (db)
            {
                var query = from i in db.Departments
                            select new
                            {
                                subject = i.Subject,
                                dname = i.Name,
                                courses = from j in i.Courses 
                                select new
                                {
                                    number = j.Number,
                                    cname = j.Name
                                }
                            };
                return Json(query.ToArray());
            }
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            using (db)
            {
                var query = from i in db.Classes
                            join p in db.Professors on i.TaughtBy equals p.UId
                            join c in db.Courses on i.Listing equals c.CatalogId
                            where c.Department == subject && c.Number == number
                            select new
                            {
                                season = i.Season,
                                year = i.Year,
                                location = i.Location,
                                start = i.StartTime,
                                end = i.EndTime,
                                fname = p.FName,
                                lname = p.LName
                            };

                return Json(query.ToArray());
            }
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            using (db)
            {
                var classes = db.Courses.Where(c => c.Department == subject && c.Number == num).First().Classes;
                var assignmentCategories = classes.Where(c => c.Season == season && c.Year == year).First().AssignmentCategories;
                var assignments = assignmentCategories.Where(a => a.Name == category).First().Assignments;
                var assignment = assignments.Where(a => a.Name == asgname).First();
                return Content(assignment.Contents);
            }               
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {

            return Content("");
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            using (db)
            {
                if(db.Students.Where(s => s.UId == uid).Any())
                {
                    var query = from i in db.Students
                                where i.UId == uid
                                select new
                                {
                                    fname = i.FName,
                                    lname = i.LName,
                                    uid = i.UId,
                                    department = i.Major
                                };
                    return Json(query.ToArray());
                }
                if(db.Professors.Where(p => p.UId == uid).Any())
                {
                    var query = from i in db.Professors
                                where i.UId == uid
                                select new
                                {
                                    fname = i.FName,
                                    lname = i.LName,
                                    uid = i.UId,
                                    department = i.WorksIn
                                };
                    return Json(query.ToArray());
                }
                if(db.Administrators.Where(a => a.UId == uid).Any())
                {
                    var query = from i in db.Administrators
                                where i.UId == uid
                                select new
                                {
                                    fname = i.FName,
                                    lname = i.LName,
                                    uid = i.UId,
                                };
                    return Json(query.ToArray());
                }
                else
                {
                    return Json(new { success = false });
                }
            }
        }


        /*******End code to modify********/

    }
}