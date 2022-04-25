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

                    /*var query = from co in db.Courses
                                join c in db.Classes on co.CatalogId equals c.Listing
                                join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                join a in db.Assignments on ac.CategoryId equals a.Category
                                join s in db.Submissions on a.AssignmentId equals s.Assignment into sub
                                from x in sub.DefaultIfEmpty()
                                where co.Number == num && co.Department == subject && ac.Name==category && c.Season == season && c.Year == year*/

                    var query = from i in db.Courses 
                                join c in db.Classes on i.CatalogId equals c.Listing
                                join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                join a in db.Assignments on ac.CategoryId equals a.Category into bulk
                                from j in bulk
                                join s in db.Submissions on j.AssignmentId equals s.Assignment into right
                                from k in right.DefaultIfEmpty()
                                where i.Number == num && i.Department == subject && ac.Name==category && c.Season == season && c.Year == year

                                select new
                                {
                                    aname = j.Name,
                                    cname = ac.Name,

                                    /*due = a.Due,
                                    submissions =x==null?0: (uint?)x.Student.Count()*/
                                   

                                    due = j.Due,
                                    submissions = k == null ? 0 : (from i in db.Submissions
                                                                  
                                                                   where i.Assignment == j.AssignmentId

                                                                 // && ac.Name == category
                                                                   select i.Student).Count()                          

                                };

                        return Json(query.ToArray());

                }
            }
            else
            {
                using (db)
                {

                    /*var query = from co in db.Courses
                                join c in db.Classes on co.CatalogId equals c.Listing
                                join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                join a in db.Assignments on ac.CategoryId equals a.Category 
                                join s in db.Submissions on a.AssignmentId equals s.Assignment into sub
                                from x in sub.DefaultIfEmpty()
                                where co.Number == num && co.Department == subject && c.Season == season && c.Year == year
                                && ac.Name.Any()*/

                    var query = from i in db.Courses
                                join c in db.Classes on i.CatalogId equals c.Listing
                                join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                join a in db.Assignments on ac.CategoryId equals a.Category into bulk
                                from j in bulk
                                join s in db.Submissions on j.AssignmentId equals s.Assignment into right
                                from k in right.DefaultIfEmpty()
                                where i.Number == num && i.Department == subject && c.Season == season && c.Year == year

                                select new
                                {
                                    aname = j.Name,
                                    cname = ac.Name,

                                    /*due = a.Due,
                                    submissions =x==null?0: (uint?)x.Student.Count()*/


                                    due = j.Due,
                                    submissions = k == null ? 0 : (from i in db.Submissions

                                                                   where i.Assignment == j.AssignmentId

                                                                   // && ac.Name == category
                                                                   select i.Student).Count()

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
                if (db.Assignments.Where(a => a.Name == asgname && a.CategoryNavigation.InClassNavigation.Year==year && a.CategoryNavigation.InClassNavigation.Season==season
                && a.CategoryNavigation.InClassNavigation.ListingNavigation.Department==subject && a.CategoryNavigation.InClassNavigation.ListingNavigation.Number==num
                && a.CategoryNavigation.Name == category).Any()) //&& a.CategoryNavigation.Name==category
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
                    /*var uids = from c in db.Classes
                               join co in db.Courses on c.Listing equals co.CatalogId
                               join e in db.Enrolled on c.ClassId equals e.Class
                               where co.Department == subject && co.Number == num && c.Season == season && c.Year == year
                               select e.Student;//get uids of all students in the class 
                    foreach (string uid in uids)
                    {
                       
                    }*/
                    this.CalcGrade(subject, num, season, year, null);
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

                Submissions s = submission.SingleOrDefault();//set score
                if (s != null)
                {
                    s.Score = (uint)score;
                }
                else
                {
                    s.Score = 0;
                }
                
                try
                {
                    db.SaveChanges();                 
                }               
                catch (Exception)
                {
                    return Json(new { success = false });
                }
                this.CalcGrade(subject, num, season, year, uid);
                return Json(new { success = true });
            }
           

        }

        /// <summary>
        /// auto grading
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="num"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <param name="uid"></param> if null calc grades for all students in class
        private void CalcGrade(string subject, int num, string season, int year, string uid)
        {
            using (db)
            {
                if (uid == null)
                {
                    var uids = from c in db.Classes
                               join co in db.Courses on c.Listing equals co.CatalogId
                               join e in db.Enrolled on c.ClassId equals e.Class
                               where co.Department == subject && co.Number == num && c.Season == season && c.Year == year
                               select e.Student;//get uids of all students in the class 
                    foreach (string uid2 in uids)
                    {

                        var categories = (
                                 from i in db.Assignments
                                 join ac in db.AssignmentCategories on i.Category equals ac.CategoryId into asgc
                                 from x in asgc.DefaultIfEmpty()
                                 join co in db.Classes on x.InClass equals co.ClassId
                                 join cour in db.Courses on co.Listing equals cour.CatalogId
                                 where cour.Department == subject && cour.Number == num && co.Season == season && co.Year == year
                                 select x).Distinct();


                float scalingFactor = 0;
                float totalScore = 0;
                foreach (AssignmentCategories asgCat in categories)
                {
                    if (asgCat == null)
                    {
                        continue;
                    }
                    float maxP = 0;
                    float Tscore = 0;
                    var submissions = 
                                    from i in db.Courses
                                    join c in db.Classes on i.CatalogId equals c.Listing
                                    join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                    join a in db.Assignments on ac.CategoryId equals a.Category into asg
                                    from y in asg.DefaultIfEmpty()
                                    join s in db.Submissions on y.AssignmentId equals s.Assignment into sub
                                    from x in sub.DefaultIfEmpty()
                                    join e in db.Enrolled on c.ClassId equals e.Class
                                    where i.Department == subject && i.Number == num && c.Season == season && c.Year == year && ac.Name == asgCat.Name
                                     && e.Student == uid2 //student's submissions
                                    select x;

                   foreach (Submissions s in submissions)
                    {
                        if (s == null)
                        {
                            Tscore += 0;
                        }
                        else {
                            Tscore += (float)s.Score; //gets scores of all assignments in category
                        }

                    }
                    var assign = from ac in db.AssignmentCategories //gets assignments in categories in class
                                 join c in db.Classes on ac.InClass equals c.ClassId into cl
                                 from z in cl.DefaultIfEmpty()
                                 join co in db.Courses on z.Listing equals co.CatalogId into cour
                                 from b in cour.DefaultIfEmpty()
                                 join a in db.Assignments on ac.CategoryId equals a.Category into asg
                                 from x in asg.DefaultIfEmpty()
                                 join s in db.Submissions on x.AssignmentId equals s.Assignment into sub
                                 from y in sub.DefaultIfEmpty()
                                 where b.Department == subject && b.Number == num && z.Season == season && z.Year == year && ac.Name==asgCat.Name
                                 select x;
                    foreach(Assignments asgn in assign)
                    {
                        maxP += (float)asgn.MaxPoints;
                    }
                    totalScore += (float)((Tscore / maxP)*asgCat.Weight);
                    scalingFactor+= asgCat.Weight;
                }
                float scalingFactorTotal = 100 / scalingFactor;
                float totalPerc = totalScore * scalingFactorTotal;
                string grade="--";
                switch (totalPerc)
                {
                    case float n when n >=93:
                        grade = "A";
                        break;
                    case float n when n >= 90:
                        grade = "A-";
                        break;
                    case float n when n >= 87:
                        grade = "B+";
                        break;
                    case float n when n >= 83:
                        grade = "B";
                        break;
                    case float n when n >= 80:
                        grade = "B-";
                        break;
                    case float n when n >= 77:
                        grade = "C+";
                        break;
                    case float n when n >= 73:
                        grade = "C";
                        break;
                    case float n when n >= 70:
                        grade = "C-";
                        break;
                    case float n when n >= 67:
                        grade = "D+";
                        break;
                    case float n when n >= 63:
                        grade = "D";
                        break;
                    case float n when n >= 60:
                        grade = "D-";
                        break;
                    case float n when n < 60:
                        grade = "E";
                        break;

                }
           
                    var query = from e in db.Enrolled
                                join c in db.Classes on e.Class equals c.ClassId
                                join co in db.Courses on c.Listing equals co.CatalogId
                                where e.Student == uid2 && c.Year==year && c.Season==season && co.Number==num && co.Department==subject
                                select e;
                    Enrolled en = query.SingleOrDefault();
                    en.Grade = grade;
                    db.SaveChanges();
                

            }
                }
                else
                {
                    var categories = (
                                 from i in db.Assignments
                                 join ac in db.AssignmentCategories on i.Category equals ac.CategoryId into asgc
                                 from x in asgc.DefaultIfEmpty()
                                 join co in db.Classes on x.InClass equals co.ClassId
                                 join cour in db.Courses on co.Listing equals cour.CatalogId
                                 where cour.Department == subject && cour.Number == num && co.Season == season && co.Year == year
                                 select x).Distinct();


                    float scalingFactor = 0;
                    float totalScore = 0;
                    foreach (AssignmentCategories asgCat in categories)
                    {
                        if (asgCat == null)
                        {
                            continue;
                        }
                        float maxP = 0;
                        float Tscore = 0;
                        var submissions =
                                        from i in db.Courses
                                        join c in db.Classes on i.CatalogId equals c.Listing
                                        join ac in db.AssignmentCategories on c.ClassId equals ac.InClass
                                        join a in db.Assignments on ac.CategoryId equals a.Category into asg
                                        from y in asg.DefaultIfEmpty()
                                        join s in db.Submissions on y.AssignmentId equals s.Assignment into sub
                                        from x in sub.DefaultIfEmpty()
                                        join e in db.Enrolled on c.ClassId equals e.Class
                                        where i.Department == subject && i.Number == num && c.Season == season && c.Year == year && ac.Name == asgCat.Name
                                         && e.Student == uid //student's submissions
                                    select x;

                        foreach (Submissions s in submissions)
                        {
                            if (s == null)
                            {
                                Tscore += 0;
                            }
                            else
                            {
                                Tscore += (float)s.Score; //gets scores of all assignments in category
                            }

                        }
                        var assign = from ac in db.AssignmentCategories //gets assignments in categories in class
                                     join c in db.Classes on ac.InClass equals c.ClassId into cl
                                     from z in cl.DefaultIfEmpty()
                                     join co in db.Courses on z.Listing equals co.CatalogId into cour
                                     from b in cour.DefaultIfEmpty()
                                     join a in db.Assignments on ac.CategoryId equals a.Category into asg
                                     from x in asg.DefaultIfEmpty()
                                     join s in db.Submissions on x.AssignmentId equals s.Assignment into sub
                                     from y in sub.DefaultIfEmpty()
                                     where b.Department == subject && b.Number == num && z.Season == season && z.Year == year && ac.Name == asgCat.Name
                                     select x;
                        foreach (Assignments asgn in assign)
                        {
                            maxP += (float)asgn.MaxPoints;
                        }
                        totalScore += (float)((Tscore / maxP) * asgCat.Weight);
                        scalingFactor += asgCat.Weight;
                    }
                    float scalingFactorTotal = 100 / scalingFactor;
                    float totalPerc = totalScore * scalingFactorTotal;
                    string grade = "--";
                    switch (totalPerc)
                    {
                        case float n when n >= 93:
                            grade = "A";
                            break;
                        case float n when n >= 90:
                            grade = "A-";
                            break;
                        case float n when n >= 87:
                            grade = "B+";
                            break;
                        case float n when n >= 83:
                            grade = "B";
                            break;
                        case float n when n >= 80:
                            grade = "B-";
                            break;
                        case float n when n >= 77:
                            grade = "C+";
                            break;
                        case float n when n >= 73:
                            grade = "C";
                            break;
                        case float n when n >= 70:
                            grade = "C-";
                            break;
                        case float n when n >= 67:
                            grade = "D+";
                            break;
                        case float n when n >= 63:
                            grade = "D";
                            break;
                        case float n when n >= 60:
                            grade = "D-";
                            break;
                        case float n when n < 60:
                            grade = "E";
                            break;

                    }

                    var query = from e in db.Enrolled
                                join c in db.Classes on e.Class equals c.ClassId
                                join co in db.Courses on c.Listing equals co.CatalogId
                                where e.Student == uid && c.Year == year && c.Season == season && co.Number == num && co.Department == subject
                                select e;
                    Enrolled en = query.SingleOrDefault();
                    en.Grade = grade;
                    db.SaveChanges();
                }
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