using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    interface IControllerPanel
    {
        void Reload();
        void Save();
    }
}
