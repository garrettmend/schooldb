using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            using (db)
            {
                var query = from i in db.Students
                            join e in db.Enrolled on i.UId equals e.Student
                            join c in db.Classes on e.Class equals c.ClassId
                            join co in db.Courses on c.Listing equals co.CatalogId
                            where i.UId == uid
                            select new
                            {
                                subject = co.Department,
                                number = co.Number,
                                name = co.Name,
                                season = c.Season,
                                year = c.Year,
                                grade = e.Grade
                            };

                return Json(query.ToArray());
            }               
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            using (db)
            {
                // course -> class -> assignment Categories -> assignment -> submissions
                Courses course = db.Courses.Where(c => c.Department == subject && c.Number == num).Include(c => c.Classes).ThenInclude(c => c.AssignmentCategories).First();
                Classes clss = course.Classes.Where(c => c.Season == season && c.Year == year).First();
                var acs = clss.AssignmentCategories;
                return Json(null);
            }
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            using (db)
            {
                try
                {
                    var course = db.Courses.Where(c => c.Department == subject && c.Number == num).First();
                    var clss = course.Classes.Where(c => c.Season == season && c.Year == year).First();
                    var acategory = clss.AssignmentCategories.Where(a => a.Name == category).First();
                    Assignments assignment = acategory.Assignments.Where(a => a.Name == asgname).First();
                    Submissions submission = assignment.Submissions.Where(s => s.Student == uid).FirstOrDefault();
                    // Check if they have already submitted
                    if (submission != null)
                    {
                        submission.SubmissionContents = contents;
                        submission.Time = DateTime.Now;
                    }
                    else // No submission yet
                    {
                        Submissions newSubmit = new Submissions
                        {
                            Assignment = assignment.AssignmentId,
                            Student = uid,
                            Score = 0,
                            SubmissionContents = contents,
                            Time = DateTime.Now
                        };
                        db.Submissions.Add(newSubmit);
                    }
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    return Json(new { success = false });
                }               
            }
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            using(db)
            {
                Courses course = db.Courses.Where(c => c.Department == subject && c.Number == num).Include(c => c.Classes).ThenInclude(c => c.Enrolled).First();
                Classes clss = course.Classes.Where(c => c.Season == season && c.Year == year).First();
                var enrollments = clss.Enrolled;
                // Student already enrolled
                if (enrollments.Where(e => e.Student == uid).Any())
                {
                    return Json(new { success = false });
                }
                else
                {
                    Enrolled newEnroll = new Enrolled
                    {
                        Class = clss.ClassId,
                        Student = uid,
                        Grade = "--"
                    };
                    db.Enrolled.Add(newEnroll);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            double gradePoints = 0.0;
            int count = 0;
            int creditHour = 4;
            using (db)
            {
                var enrollments = db.Enrolled.Where(e => e.Student == uid);                
                foreach (Enrolled e in enrollments)
                {
                    string grade = e.Grade;
                    if (!grade.Equals("--"))
                    {
                        count++;
                        switch(grade)
                        {
                            case "A":
                                gradePoints += (4.0*creditHour);
                                break;
                            case "A-":
                                gradePoints += (3.7*creditHour);
                                break;
                            case "B+":
                                gradePoints += (3.3*creditHour);
                                break;
                            case "B":
                                gradePoints += (3.0*creditHour);
                                break;
                            case "B-":
                                gradePoints += (2.7*creditHour);
                                break;
                            case "C+":
                                gradePoints += (2.3*creditHour);
                                break;
                            case "C":
                                gradePoints += (2.0*creditHour);
                                break;
                            case "C-":
                                gradePoints += (1.7*creditHour);
                                break;
                            case "D+":
                                gradePoints += (1.3*creditHour);
                                break;
                            case "D":
                                gradePoints += (1.0*creditHour);
                                break;
                            case "D-":
                                gradePoints += (0.7*creditHour);
                                break;
                            case "E":
                                break;
                        }
                    }
                }               
            }
            int creditHours = count * creditHour;
            double GPA = gradePoints / creditHour;
            var json = new { gpa = GPA };
            return Json(json);
        }

        /*******End code to modify********/

    }
}