using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKIDEI.Service.Models
{
    public class ErrorInfo
    {
        public ErrorInfo() { }
        
        public ErrorInfo(bool isNormal) => IsNormal = isNormal;

        public ErrorInfo(bool isNormal, string title, string desc)
        {
            IsNormal = isNormal;
            Title = title;
            Description = desc;
        }

        /// <summary>
        /// 프로그램 종료가 정상적이었는지 여부를 나타냅니다.
        /// </summary>
        public bool IsNormal { get; set; } = false;
        /// <summary>
        /// 에러의 성격을 나타내는 제목을 기록하거나 확인합니다.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 에러의 상세내용을 기록하거나 확인합니다.
        /// </summary>
        public string Description { get; set; }
    }
}
