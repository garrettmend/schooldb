using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrolled = new HashSet<Enrolled>();
        }

        public uint ClassId { get; set; }
        public string Season { get; set; }
        public uint Year { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public uint Listing { get; set; }
        public string TaughtBy { get; set; }

        public virtual Courses ListingNavigation { get; set; }
        public virtual Professors TaughtByNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
    }
}
