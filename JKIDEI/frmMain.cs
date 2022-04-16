using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace JKIDEI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            InitControls();
        }

        private void InitControls()
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
            // 


        }

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
