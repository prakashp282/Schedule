using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Dtos
{
    /// <summary>
    /// Job執行狀態
    /// </summary>
    public enum JobStatus : byte
    {
        [Description("Initialization")]
        Init = 0,
        [Description("Scheduled")]
        Scheduled = 1,
        [Description("Running")]
        Running = 2,
        [Description("Stopped")]
        Stopped = 3,
    }
}
