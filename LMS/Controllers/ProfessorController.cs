using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            using (db)
            {
                var query = from i in db.Enrolled
                            join s in db.Students on i.Student equals s.UId
                            join cl in db.Classes on i.Class equals cl.ClassId
                           // where cl.Year == year && cl.Season == season
                            join co in db.Courses on cl.Listing equals co.CatalogId 
                            where co.Number == num && co.Department == subject && cl.Year == year && cl.Season == season
                            select new
                            {
                                fname = s.FName,
                                lname = s.LName,
                                uid = s.UId,
                                dob = s.Dob,
                                grade = i.Grade
                            };
                            


                return Json(query.ToArray());
            }
    }



    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
            if (category != null)
            {
                using (db) {
                    var query = from i in db.Submissions
                                join a in db.Assignments on i.Assignment equals a.AssignmentId
                                join ac in db.AssignmentCategories on a.Category equals ac.CategoryId
                              //  where ac.Name == category
                                join c in db.Classes on ac.InClass equals c.ClassId
                              //  where c.Season == season && c.Year == year
                                join co in db.Courses on c.Listing equals co.CatalogId
                                where co.Number == num && co.Department == subject && ac.Name==category && c.Season == season && c.Year == year
                                select new
                                {
                                    aname = a.Name,
                                    cname = ac.Name,
                                    due = a.Due,
                                    submissions = i.Student.Count()
                                   

                                };
                    return Json(query.ToArray());
                }
            }
            else
            {
                using (db)
                {
                    var query = from i in db.Submissions
                                join a in db.Assignments on i.Assignment equals a.AssignmentId
                                join ac in db.AssignmentCategories on a.Category equals ac.CategoryId
                               // where ac.Name == category
                                join c in db.Classes on ac.InClass equals c.ClassId
                               // where c.Season == season && c.Year == year
                                join co in db.Courses on c.Listing equals co.CatalogId
                                where co.Number == num && co.Department == subject && c.Season == season && c.Year == year
                                select new
                                {
                                    aname = a.Name,
                                    cname = ac.Name,
                                    due = a.Due,
                                    submissions = i.Student.Count()


                                };
                    return Json(query.ToArray());
                }

            }
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            using (db)
            {
                var query = from ac in db.AssignmentCategories
                            join c in db.Classes on ac.InClass equals c.ClassId                          
                            //where c.Season == season && c.Year == year
                            join co in db.Courses on c.Listing equals co.CatalogId
                            where co.Number == num && co.Department == subject && c.Season == season && c.Year == year
                            select new
                            {
                                name = ac.Name,
                                weight = ac.Weight                              

                            };
                return Json(query.ToArray());
            }
    }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false},
    ///	false if an assignment category with the same name already exists in the same class.</returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
            using (db)
            {
                if (db.AssignmentCategories.Where(ac => ac.Name == category && ac.InClassNavigation.ListingNavigation.Number == num &&
                ac.InClassNavigation.ListingNavigation.Department == subject && ac.InClassNavigation.Season==season && ac.InClassNavigation.Year==year).Any())
                {
                    return Json(new { success = false });
                }
                else
                {
                    uint classId = db.Classes.Where(c => c.ListingNavigation.Department == subject && c.ListingNavigation.Number == num
                    && c.Season == season && c.Year == year).First().ClassId;
                    uint CategoryID = db.AssignmentCategories.OrderByDescending(c => c.CategoryId).First().CategoryId + 1;
                    AssignmentCategories ac = new AssignmentCategories
                    {
                        CategoryId = CategoryID,
                        Name = category,
                        Weight = (uint)catweight,
                        InClass = classId
                    };
                    db.AssignmentCategories.Add(ac);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
    }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false,
	/// false if an assignment with the same name already exists in the same assignment category.</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
            using (db)
            {
                if (db.Assignments.Where(a => a.Name == asgname && a.CategoryNavigation.Name==category).Any())
                {
                    return Json(new { success = false });
                }
                else
                {
                    uint cat = db.AssignmentCategories.Where(ac => ac.InClassNavigation.Season==season && ac.InClassNavigation.Year==year
                    && ac.Name==category && ac.InClassNavigation.ListingNavigation.Number==num && ac.InClassNavigation.ListingNavigation
                    .Department==subject).First().CategoryId;

                    uint AsgnID = db.Assignments.OrderByDescending(c => c.AssignmentId).First().AssignmentId + 1;
                    Assignments a = new Assignments
                    {
                        AssignmentId = AsgnID,
                        Name = asgname,
                        Contents = asgcontents,
                        Due = asgdue,
                        MaxPoints=(uint)asgpoints,
                        Category = cat
                    };
                    db.Assignments.Add(a);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }

    }


    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {

            using (db)
            {
                var query = from i in db.Submissions
                            join a in db.Assignments on i.Assignment equals a.AssignmentId
                           // where a.Name == asgname
                            join s in db.Students on i.Student equals s.UId
                            join ac in db.AssignmentCategories on a.Category equals ac.CategoryId
                          //  where ac.Name == category
                            join c in db.Classes on ac.InClass equals c.ClassId
                          //  where c.Season == season && c.Year == year
                            join co in db.Courses on c.Listing equals co.CatalogId
                            where co.Number == num && co.Department == subject && a.Name == asgname && ac.Name == category && c.Season == season && c.Year == year
                            select new
                            {
                                fname = s.FName,
                                lname = s.LName,
                                uid = s.UId,
                                time = i.Time,
                                score = i.Score


                            };
                return Json(query.ToArray());
            }
        }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {
            using (db)
            {
                var submission = from i in db.Submissions//gets submission to set score of
                                 join a in db.Assignments on i.Assignment equals a.AssignmentId
                                 join ac in db.AssignmentCategories on a.Category equals ac.CategoryId
                                 join c in db.Classes on ac.InClass equals c.ClassId
                                 join co in db.Courses on c.Listing equals co.CatalogId
                                 where co.Department == subject && co.Number == num && c.Season == season && c.Year == year && ac.Name == category
                                 && a.Name == asgname && i.Student == uid
                                 select i;
               
                    foreach(Submissions s in submission)//sets score of submission
                    {
                        s.Score=(uint)score;

                    }
                
                try
                {
                    db.SaveChanges();                 
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
                return Json(new { success = true });
            }
           

        }


    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            using (db)
            {
                var query = from i in db.Classes
                            join p in db.Professors on i.TaughtBy equals p.UId
                            join co in db.Courses on i.Listing equals co.CatalogId
                            where p.UId == uid
                            select new
                            {
                                subject = p.WorksIn,
                                number = co.Number,
                                name = co.Name,
                                season = i.Season,
                                year = i.Year
                            };

                return Json(query.ToArray());
            }
    }


    /*******End code to modify********/

  }
}