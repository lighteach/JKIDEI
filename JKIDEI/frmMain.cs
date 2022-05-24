using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using JKIDEI.Common;
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

        #region InitControls
        private void InitControls()
        {
            SetMainNotice();
            lblVersion.Text = $"v {Application.ProductVersion}";

            panel1.Paint += (sender, e) =>
            {
                Panel panel = (Panel)sender;
                Color[] shadow = { Color.FromArgb(181, 181, 181), Color.FromArgb(195, 195, 195), Color.FromArgb(211, 211, 211) };
                using (Pen pen = new Pen(shadow[0]))
                {
                    Point pt = this.Location;
                    pt.Y += this.Height;
                    foreach(int sp in Enumerable.Range(0, 3))
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X + this.Width - 1, pt.Y);
                        pt.Y++;
                    }
                }

            };
        }
        #endregion

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
            btnStart.BackColor = Color.Transparent;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 2;
            btnStart.FlatAppearance.BorderColor = Color.Gray;
            btnStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(237)))));
            btnStart.Location = new Point(620, 300);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(320, 50);
            btnStart.TabIndex = 1;
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            pnlReport.Controls.Add(btnStart);

            lblPrgRatio.Text = "Ready";
            lblPrgRatio.Update();
        }
        #endregion

        #region GetNoticeLabel : 메인 안내 문구 용 Label 생성
        private Label GetNoticeLabel(string text, bool IsTitle, int width, bool smaller = false)
        {
            float fontSize = IsTitle ? 16F : 12F;
            if (smaller) fontSize = 9F;

            FontStyle fontStyle = IsTitle ? FontStyle.Bold : FontStyle.Regular;
            Color foreColor = IsTitle ? Color.FromArgb(55, 60, 66) : Color.White;
            int labelHeight = IsTitle ? 36 : 24;
            int labelLeft = IsTitle ? 20 : 60;
            if (smaller) labelLeft = 80;

            int labelTop = 10;
            if (pnlReport.Controls.Count > 0)
            {
                Control lastCtrl = pnlReport.Controls.Cast<Control>().Last();
                int locY = lastCtrl.Location.Y;
                int ctrlHeight = lastCtrl.Height;
                int totLocY = (ctrlHeight + locY) + (IsTitle ? 5 : 3);  // 이전 컨트롤의 위치와 높이를 더해 하단 위치 값을 구하고 5(타이틀이 아닌 경우 3)픽셀의 간격을 설정
                if (smaller) totLocY = (ctrlHeight + locY);

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

            Label lblStartMsg = GetNoticeLabel("설치 및 셋팅을 진행합니다.", true, 340);
            lblStartMsg.ForeColor = Color.FromArgb(0, 0, 128);
            pnlReport.Controls.Add(lblStartMsg);

            // 프로그레스바 관련 셋팅
            lblPrgRatio.Text = "진행중...";

            Progress<ProgressReport> prog = new Progress<ProgressReport>();
            prog.ProgressChanged += (o, report) =>
            {
                lblPrgRatio.Text = $"{report.PercentComplete}%";
                progBar.Value = report.PercentComplete;
                progBar.Update();
            };

            await TaskUnitProgressUpdate(prog);

            lblPrgRatio.Text = "All Proceed Complete!";
        }
        #endregion

        #region TaskUnitProgressUpdate : 각각의 작업들을 차례대로 실행시킨다.
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
                        this.Invoke(new Action(delegate ()
                        {
                            pnlReport.Controls.Add(GetNoticeLabel(tu.TaskDescription, false, 360));
                        }));
                        report.PercentComplete = index++ * 100 / totalProcess;
                        prog.Report(report);
                        if (tu.Verify())    // 현재 작업을 검증한다.
                        {
                            ErrorInfo error = tu.Execute();
                            if (!error.IsNormal)
                            {
                                //MessageBox.Show(error.Description, error.Title);
                                this.Invoke(new Action(delegate ()
                                {
                                    //SetMainNotice();

                                    Label lblErrorInfo = GetNoticeLabel("설치되지 않은 항목이 있습니다.", false, 870);
                                    pnlReport.Controls.Add(lblErrorInfo);

                                    string[] arrError = error.Description.Split(new char[] { '|', '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string strErr in arrError)
                                    {
                                        Label lblErr = GetNoticeLabel(strErr, false, 870, true);
                                        pnlReport.Controls.Add(lblErr);
                                    }
                                }));
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show(tu.TaskDescription, "작업 검증이 실패하였습니다.");
                            this.Invoke(new Action(delegate ()
                            {
                                SetMainNotice();
                                progBar.Value = 0;
                                progBar.Update();
                            }));
                            break;
                        }
                    }

                    this.Invoke(new Action(delegate ()
                    {
                        Label lblComplete = GetNoticeLabel("작업이 모두 완료되었습니다.", false, 320);
                        lblComplete.ForeColor = Color.FromArgb(73, 73, 73);
                        lblComplete.Font = new Font(lblComplete.Font, FontStyle.Bold);
                        pnlReport.Controls.Add(lblComplete);
                    }));
                });
            }
        }
        #endregion

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        const int CS_DROPSHADOW = 0x2000;
        //        CreateParams cp = new CreateParams();
        //        cp.ClassStyle |= CS_DROPSHADOW;
        //        return cp;
        //    }
        //}

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
