using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public string Student { get; set; }
        public uint Class { get; set; }
        public string Grade { get; set; }

        public virtual Classes ClassNavigation { get; set; }
        public virtual Students StudentNavigation { get; set; }
    }
}
