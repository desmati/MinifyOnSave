using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinifyOnSave
{
    public class Features
    {
        public bool IgnoreJsComments { get; set; }

        /// <summary>
        /// Should we ignore the html comments and not minify?
        /// </summary>
        public bool IgnoreHtmlComments { get; set; }

        /// <summary>
        /// Property for the max character count
        /// </summary>
        public int MaxLength { get; set; }
    }
}
