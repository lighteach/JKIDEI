using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKIDEI.Service.TaskUnits;
using JKIDEI.Service.Models;

namespace JKIDEI.Service
{
    public class TaskUnitService
    {
        private static TaskUnitService _service = null;

        private List<ITaskUnit> _taskList = null;
        
        private TaskUnitService()
        {
            _taskList = new List<ITaskUnit>();
            _taskList.Add(new ComPlusInstallationTaskUnit());   // COM+ 설치 태스크
            _taskList.Add(new DbConnCopyTaskUnit());   // dbconn 파일 복사 태스크
            _taskList.Add(new IISWebSiteSetTaskUnit());   // IIS구성요소 설치 태스크
            _taskList.Add(new ExeInstallTaskUnit());   // 필수 인스톨러 설치자 실행 태스크
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
    }
}
