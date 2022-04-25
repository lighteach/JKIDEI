using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKIDEI.Service.Models;

namespace JKIDEI.Service
{
    public interface ITaskUnit
    {
        bool Verify();
        string TaskDescription { get; set; }
        ErrorInfo Execute();
    }
}
