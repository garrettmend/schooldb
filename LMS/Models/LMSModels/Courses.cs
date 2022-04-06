using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CatalogId { get; set; }
        public uint Number { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public virtual Departments DepartmentNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
