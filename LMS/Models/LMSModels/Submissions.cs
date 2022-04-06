using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public uint Assignment { get; set; }
        public string Student { get; set; }
        public uint Score { get; set; }
        public string SubmissionContents { get; set; }
        public DateTime Time { get; set; }

        public virtual Assignments AssignmentNavigation { get; set; }
        public virtual Students StudentNavigation { get; set; }
    }
}
