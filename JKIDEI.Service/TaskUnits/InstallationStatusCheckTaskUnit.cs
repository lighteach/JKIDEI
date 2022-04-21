using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMAdmin;
using JKIDEI.Service;
using JKIDEI.Service.Models;

namespace JKIDEI.Service.TaskUnits
{
    public class InstallationStatusCheckTaskUnit : ITaskUnit
    {
        private COMAdminCatalog catalog = null;
        private COMAdminCatalogCollection applications = null;

        private List<string> _installedDllList = null;

        public InstallationStatusCheckTaskUnit()
        {
            _installedDllList = new List<string>();
            string[] assetDllFiles = AssetDllFiles;

            catalog = new COMAdminCatalog();
            applications = (COMAdminCatalogCollection)catalog.GetCollection("Applications");
            applications.Populate();

            foreach (COMAdminCatalogObject app in applications)
            {
                COMAdminCatalogCollection components = (COMAdminCatalogCollection)applications.GetCollection("Components", app.Key);
                components.Populate();
                foreach (COMAdminCatalogObject comp in components)
                {
                    string compDll = Path.GetFileName(comp.Value["DLL"]);
                    if (assetDllFiles.Any(s => Path.GetFileName(s).Equals(compDll)))
                    {
                        _installedDllList.Add(compDll);
                    }
                }
            }
        }

        #region AssetDllFiles : \Assets\InstallationStatusCheckTaskUnit 디렉토리의 DLL 파일 목록을 가져온다
        private string[] AssetDllFiles
        {
            get
            {
                string assetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Assets", this.GetType().Name);
                string[] dllFiles = Directory.GetFiles(assetPath, "*.dll");
                //dllFiles = dllFiles.Select(s => Path.GetFileName(s)).ToArray();
                return dllFiles;
            }
        } 
        #endregion


        public bool Verify() => true;

        public string TaskDescription => "1. COM+ 구성요소 DLL 설치";

        public ErrorInfo Execute()
        {
            ErrorInfo errorInfo = new ErrorInfo(true);
            string[] dllFiles = AssetDllFiles;

            StringBuilder sb = new StringBuilder();

            // 1. COM+ DLL 리스트 설치 확인
            #region 실행 테스트 용 코드
            //COMAdminCatalog catalog = new COMAdminCatalog();
            //COMAdminCatalogCollection applications = (COMAdminCatalogCollection)catalog.GetCollection("Applications");
            //applications.Populate();

            //foreach (COMAdminCatalogObject app in applications)
            //{
            //    sb.AppendLine("".PadLeft(120, '-'));
            //    sb.AppendLine($"{app.Name}, {app.Key}");
            //    sb.AppendLine("".PadLeft(120, '-'));

            //    COMAdminCatalogCollection components = (COMAdminCatalogCollection)applications.GetCollection("Components", app.Key);
            //    components.Populate();
            //    foreach (COMAdminCatalogObject comp in components)
            //    {
            //        sb.AppendLine($"\tName : {comp.Name}");
            //        sb.AppendLine($"\tConstructorString : {comp.Value["ConstructorString"]}");
            //        sb.AppendLine($"\tDLL : {comp.Value["DLL"]}");
            //    }
            //}
            //MessageBox.Show(sb.ToString());
            #endregion

            try
            {
                foreach (COMAdminCatalogObject app in applications)
                {
                    COMAdminCatalogCollection components = (COMAdminCatalogCollection)applications.GetCollection("Components", app.Key);
                    components.Populate();
                    foreach (COMAdminCatalogObject comp in components)
                    {
                        string compName = comp.Name;
                        string installedDll = Path.GetFileName(comp.Value["DLL"]);
                        // 설치되지 않은 경우에만 설치한다.
                        if (!dllFiles.Any(s => Path.GetFileName(s).Equals(installedDll)))
                        {
                            string dllPath = dllFiles.First(s => Path.GetFileName(s).Equals(installedDll));
                            COMAdminCatalogObject appNew = (COMAdminCatalogObject)applications.Add();
                            appNew.Value["Name"] = "ATL_aes";
                            appNew.Value["Description"] = "이것은 COM+ 등록 테스트 입니다.";
                            applications.SaveChanges();
                            catalog.InstallComponent(appNew.Key, dllPath, string.Empty, string.Empty);


                            //throw new Exception(appNew.Key);

                        }
                    }
                }



            }
            catch (Exception ex)
            {
                errorInfo = new ErrorInfo(false, "에러발생", ex.ToString());
            }

            //if (sb.Length > 0)
            //{
            //    MessageBox.Show(sb.ToString(), "설치 된 COM+ 발견!!");
            //    errorInfo = new ErrorInfo(false, "다음 구성요소(COM+)가 설치되어있습니다.\n해당 구성요소를 삭제한 후에 다시 시도하여 주십시오.", sb.ToString());
            //}

            // 2. Sources 디렉토리 존재 여부 확인
            // 3. IIS에 설정하려는 웹사이트 존재 여부 확인

            return errorInfo;
        }
    }
}
