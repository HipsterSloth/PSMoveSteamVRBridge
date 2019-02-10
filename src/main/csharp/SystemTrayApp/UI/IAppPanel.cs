using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    interface IAppPanel
    {
        void OnPanelEntered();
        void OnPanelExited();
    }
}
