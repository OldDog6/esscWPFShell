using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using esscWPFShell;
using TestHarness.Models;
using TestHarness.Dialogs;

namespace TestHarness.ViewModels
{
    public class HelloWorldViewModel : WorkspaceViewModel
    {

        #region Private Properties

        private HelloWorld _hi;

        #endregion

        #region Constructor

        public HelloWorldViewModel()
        {
            base.DisplayName = "Hello World";
            _hi = HelloWorld.Create("Hello");

        }

        #endregion

        #region Public Properties

        public String Hello
        {
            get { return _hi.HiThere; }
            set
            {
                if (_hi.HiThere == value)
                    return;
                _hi.HiThere = value;
                OnPropertyChanged("Hello");
            }
        }

        #endregion

    }

}
