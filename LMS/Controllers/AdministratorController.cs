using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        private readonly Microsoft.Extensions.Logging.ILogger<AdministratorController> _logger;

        public AdministratorController(Microsoft.Extensions.Logging.ILogger<AdministratorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            using (db)
            {
                var query = from i in db.Courses
                            where i.Department == subject
                            select new
                            {
                                number = i.Number,
                                name = i.Name
                            };

                return Json(query.ToArray());
            }
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            using (db)
            {
                var query = from i in db.Professors
                            join c in db.Departments on i.WorksIn equals c.Subject
                            where i.WorksIn == subject 
                            select new
                            {
                                lname = i.LName,
                                fname = i.FName,
                                uid = i.UId
                            };

                return Json(query.ToArray());
            }
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            using (db)
            {
                if (db.Courses.Where(c => c.Department == subject && c.Number == number).Any())
                {
                    return Json(new { success = false });
                }
                else // Insert new Course
                {
                    // Largest Catalog num plus 1
                    uint catalogNum = db.Courses.OrderByDescending(c => c.CatalogId).First().CatalogId + 1;
                    Courses course = new Courses
                    {
                        CatalogId = catalogNum,
                        Department = subject,
                        Number = (uint)number,
                        Name = name
                    };
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            using (db)
            {
                // Resolve course catalog id first
                var course = db.Courses.Where(c => c.Department == subject && c.Number == number).FirstOrDefault();
                if (course == null)
                {
                    _logger.LogWarning("CreateClass failed: course not found: {subject} {number}", subject, number);
                    return Json(new { success = false });
                }
                uint catalogID = course.CatalogId;

                // Check for duplicate offering of the same course in the same semester
                var duplicate = db.Classes.Where(c => c.Listing == catalogID && c.Season == season && c.Year == year).FirstOrDefault();
                if (duplicate != null)
                {
                    _logger.LogWarning("CreateClass failed: duplicate offering exists for listing {listing} season {season} year {year}", catalogID, season, year);
                    return Json(new { success = false });
                }

                // Check location conflict only for the same semester (season/year)
                var conflict = db.Classes.Where(c => c.Location == location && c.Season == season && c.Year == (uint)year && (c.StartTime <= end.TimeOfDay) && (c.EndTime >= start.TimeOfDay)).FirstOrDefault();
                if (conflict != null)
                {
                    _logger.LogWarning("CreateClass failed: location conflict with class id {classId} in {season} {year} at {location}", conflict.ClassId, season, year, location);
                    return Json(new { success = false });
                }

                // Verify instructor exists
                var prof = db.Professors.FirstOrDefault(p => p.UId == instructor);
                if (prof == null)
                {
                    _logger.LogWarning("CreateClass failed: instructor not found: {instructor}", instructor);
                    return Json(new { success = false });
                }

                try
                {
                    Classes c = new Classes
                    {
                        Listing = catalogID,
                        Season = season,
                        Year = (uint) year,
                        StartTime = start.TimeOfDay,
                        EndTime = end.TimeOfDay,
                        Location = location,
                        TaughtBy = instructor
                    };
                    db.Classes.Add(c);
                    db.SaveChanges();
                    _logger.LogInformation("CreateClass succeeded: listing {listing} season {season} year {year} location {location} instructor {inst}", catalogID, season, year, location, instructor);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Save failed; log and return false without exposing details
                    _logger.LogError(ex, "CreateClass failed: exception saving class for listing {listing} season {season} year {year}", catalogID, season, year);
                    return Json(new { success = false });
                }
            }
        }


        /*******End code to modify********/

    }
}