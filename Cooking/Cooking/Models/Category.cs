using System;
using System.Collections.Generic;

namespace Cooking.Models
{
    public partial class Category
    {
        public Category()
        {
            Measure = new HashSet<Measure>();
        }

        public string CategoryCode { get; set; }
        public string Name { get; set; }
        public string MeasureCode { get; set; }

        public virtual Measure MeasureCodeNavigation { get; set; }
        public virtual ICollection<Measure> Measure { get; set; }
    }
}
