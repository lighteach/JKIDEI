using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Security.Principal;
using JKIDEI.Common;

namespace JKIDEI
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            (bool Verified, string Message) veriInfo = ExecuteVerify();
            if (!veriInfo.Verified)
            {
                MessageBox.Show(veriInfo.Message, "Execute Verify Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }

        private static (bool, string) ExecuteVerify()
        {
            bool IsExecutedAdmin = false;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                IsExecutedAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            (bool, string) rtn = (true, "");
            (bool, string)[] arrVerified =
            {
                (IsExecutedAdmin, "관리자 권한으로 실행하여 주세요.\n(우클릭 > 관리자 권한으로 실행)")
                , (Directory.Exists(JKIDEIEnv.AssetDir), "설치 항목들이있는 프로그램 디렉토리가 실행 경로내에\n존재하지 않아, 프로그램을 실행 할 수 없습니다.")
            };

            if (arrVerified.Any(tpl => !tpl.Item1))
                rtn = arrVerified.First(tpl => !tpl.Item1);

            return rtn;
        }
    }
}
