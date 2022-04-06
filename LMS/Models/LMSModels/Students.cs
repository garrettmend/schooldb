using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enrolled = new HashSet<Enrolled>();
            Submissions = new HashSet<Submissions>();
        }

        public string UId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime Dob { get; set; }
        public string Major { get; set; }

        public virtual Departments MajorNavigation { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
