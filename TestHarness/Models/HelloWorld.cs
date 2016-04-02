using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using esscWPFShell;

namespace TestHarness.Models
{
    public class HelloWorld
    {
        #region Creation

        public static HelloWorld Create() {return new HelloWorld();}
        public static HelloWorld Create(String hithere)
        {
            return new HelloWorld
            {
                HiThere = hithere
            };
        }

        #endregion

        #region Private Properties

        private String _hithere;

        #endregion

        #region Public Properties

        public String HiThere
        {
            get { return _hithere; }
            set { _hithere = value; }
        }

        #endregion
    }
}
