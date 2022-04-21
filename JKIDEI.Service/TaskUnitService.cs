using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKIDEI.Service.TaskUnits;
using JKIDEI.Service.Models;

namespace JKIDEI.Service
{
    /// <summary>
    /// TaskUnitService
    /// ToDo : 
    /// 1. Installation or Setup status check Stage
    ///     1. COM+ DLL 리스트 설치 확인
    ///     2. Sources 디렉토리 존재 여부 확인
    ///     3. IIS에 설정하려는 웹사이트 존재 여부 확인
    /// 2. Start Install / Setup Stage
    ///     1. 필수 COM+ DLL 설치
    ///     2. Download and Deployment to each directories from PC/Mobile, Admin Git Repositories
    ///     3. IIS WebSite Setup
    ///     4. Host file modify for dev websites
    ///     5. Build Up All sources
    /// </summary>
    public class TaskUnitService
    {
        private static TaskUnitService _service = null;

        private List<ITaskUnit> _taskList = null;
        
        private TaskUnitService()
        {
            _taskList = new List<ITaskUnit>();
            _taskList.Add(new InstallationStatusCheckTaskUnit());
        }

        public static TaskUnitService GetService()
        {
            if (_service == null)
                _service = new TaskUnitService();
            return _service;
        }

        public List<ITaskUnit> TaskList
        {
            get { return _taskList; }
        }

        //public void BeginService()
        //{
        //    if (_taskList != null)
        //    {
        //        foreach (ITaskUnit tu in _taskList)
        //        {
        //            ErrorInfo err = tu.Execute();
        //            if (!err.IsNormal)
        //            {
        //            }
        //        }
        //    }
        //}

    }
}
