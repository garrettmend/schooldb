using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategories
    {
        public AssignmentCategories()
        {
            Assignments = new HashSet<Assignments>();
        }

        public uint CategoryId { get; set; }
        public string Name { get; set; }
        public uint Weight { get; set; }
        public uint InClass { get; set; }

        public virtual Classes InClassNavigation { get; set; }
        public virtual ICollection<Assignments> Assignments { get; set; }
    }
}
