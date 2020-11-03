using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asaq2.Model;

namespace asaq2.WebSites.Common
{
    public interface IBrochureRequest
    {
        void ProcessRequest(BrochureRequest br, BrochureSettings settings);
    }
}
