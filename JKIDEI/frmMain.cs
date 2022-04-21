using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JKIDEI.Service;
using JKIDEI.Service.Models;

namespace JKIDEI
{
    public delegate void TaskUnitServiceTerminateEventHandler();

    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            SetMainNotice();
        }

        #region SetMainNotice : 첫 화면 안내 메시지 셋팅
        public void SetMainNotice()
        {
            pnlReport.Controls.Clear();
            Label lblFirst = GetNoticeLabel("시작 전 안내사항", true, 320);
            pnlReport.Controls.Add(lblFirst);
            Label lbl2 = GetNoticeLabel("해당 프로그램은 초기 개발 소스를 포함한 웹 사이트 구동을 위한 필수 구성요소들(DLL, IIS)을 설치하거나 셋팅합니다.", false, 870);
            pnlReport.Controls.Add(lbl2);
            Label lbl3 = GetNoticeLabel("혹시라도 기존에 필수 구성요소들을 설치해두었거나 소스 파일을 리포지토리로 부터 받아 디렉토리를 구성 한 경우", false, 850);
            pnlReport.Controls.Add(lbl3);
            Label lbl4 = GetNoticeLabel("설치가 실패할 수 있습니다.", false, 300);
            pnlReport.Controls.Add(lbl4);

            Button btnStart = new Button();
            btnStart.Text = "시작하기";
            btnStart.BackColor = System.Drawing.Color.Transparent;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 2;
            btnStart.FlatAppearance.BorderColor = Color.Gray;
            btnStart.Location = new System.Drawing.Point(200, 300);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(320, 50);
            btnStart.TabIndex = 1;
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            pnlReport.Controls.Add(btnStart);
        } 
        #endregion

        #region GetNoticeLabel : 메인 안내 문구 용 Label 생성
        private Label GetNoticeLabel(string text, bool IsTitle, int width)
        {
            float fontSize = IsTitle ? 16F : 12F;
            FontStyle fontStyle = IsTitle ? FontStyle.Bold : FontStyle.Regular;
            Color foreColor = IsTitle ? Color.FromArgb(55, 60, 66) : Color.White;
            int labelHeight = IsTitle ? 36 : 24;
            int labelLeft = IsTitle ? 20 : 60;
            int labelTop = 10;
            if (pnlReport.Controls.Count > 0)
            {
                Control lastCtrl = pnlReport.Controls.Cast<Control>().Last();
                int locY = lastCtrl.Location.Y;
                int ctrlHeight = lastCtrl.Height;
                int totLocY = (ctrlHeight + locY) + (IsTitle ? 5 : 3);  // 이전 컨트롤의 위치와 높이를 더해 하단 위치 값을 구하고 5(타이틀이 아닌 경우 3)픽셀의 간격을 설정
                labelTop = totLocY;
            }

            Label lblNotice = new Label();
            lblNotice.Text = text;
            lblNotice.Font = new Font("맑은 고딕", fontSize, fontStyle, GraphicsUnit.Point, ((byte)(129)));
            lblNotice.BackColor = Color.Transparent;
            lblNotice.ForeColor = foreColor;
            lblNotice.Location = new Point(labelLeft, labelTop);
            lblNotice.Width = width;
            lblNotice.Height = labelHeight;
            return lblNotice;
        }
        #endregion

        #region btnStart_Click : 시작 버튼 클릭
        private async void btnStart_Click(object sender, EventArgs e)
        {
            pnlReport.Controls.Clear();

            Label topSubject = GetNoticeLabel("설치 및 셋팅을 진행합니다.", true, 340);
            pnlReport.Controls.Add(topSubject);

            // 프로그레스바 관련 셋팅
            Label lblPrgRatio = new Label();
            lblPrgRatio.Text = "%";
            lblPrgRatio.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            lblPrgRatio.Location = new Point(10, 460);
            lblPrgRatio.Width = 220;
            lblPrgRatio.Height = 20;
            lblPrgRatio.Text = "진행중...";
            pnlReport.Controls.Add(lblPrgRatio);

            ProgressBar progBar = new ProgressBar();
            progBar.Location = new Point(10, 480);
            progBar.Width = 400;
            progBar.Height = 30;
            pnlReport.Controls.Add(progBar);

            Progress<ProgressReport> prog = new Progress<ProgressReport>();
            prog.ProgressChanged += (o, report) =>
            {
                lblPrgRatio.Text = $"{report.PercentComplete}%";
                progBar.Value = report.PercentComplete;
                progBar.Update();
            };

            await TaskUnitProgressUpdate(prog);

            lblPrgRatio.Text = "설치 완료!";
        } 
        #endregion

        private async Task TaskUnitProgressUpdate(IProgress<ProgressReport> prog)
        {
            TaskUnitService tuService = TaskUnitService.GetService();
            List<ITaskUnit> taskUnits = tuService.TaskList;

            if (taskUnits.Count() > 0)
            {
                int index = 1;
                int totalProcess = taskUnits.Count();

                ProgressReport report = new ProgressReport();

                await Task.Run(() =>
                {
                    foreach (ITaskUnit tu in taskUnits)
                    {
                        report.PercentComplete = index++ * 100 / totalProcess;
                        prog.Report(report);
                        ErrorInfo error = tu.Execute();
                        if (!error.IsNormal)
                        {
                            MessageBox.Show(error.Description, error.Title);
                            this.Invoke(new Action(delegate()
                            {
                                SetMainNotice();
                            }));
                            break;
                        }
                    }
                });
            }
        }


        #region 메인폼 최소화, 닫기 버튼 이벤트
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 메인 바탕 패널 마우스 이동 관련 이벤트
        private bool onClick;
        private Point startPoint = new Point(0, 0);

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (onClick)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            onClick = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            onClick = false;
        }
        #endregion
    }
}
