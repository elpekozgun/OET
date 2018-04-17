using OET_Types.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OET_UI
{
    public partial class frmGnuPlotInput : Form
    {
        public frmGnuPlotInput()
        {
            InitializeComponent();
            CenterToParent();

            prevX       = 0;
            PrevY       = 0;
            curX        = 0;
            curY        = 0;

            txtGnuInput.Text = "w l lw 3 lc var";
            command = txtGnuInput.Text;
            SubscribeEvents();
        }

        private void TxtGnuInput_LostFocus(object sender, EventArgs e)
        {
            command = txtGnuInput.Text;
        }

        private PointF mousePos;
        private float prevX;
        private float PrevY;
        private float curX;
        private float curY;
        private string command;

        public string Command
        {
            get
            {
                return command;
            }

            set
            {
                command = value;
            }
        }

        private void SubscribeEvents()
        {
            this.MouseDown          += FrmMesh_MouseDown;
            this.MouseMove          += FrmMesh_MouseMove;
            btnOk.Click             += BtnOk_Click;
            txtGnuInput.LostFocus   += TxtGnuInput_LostFocus;
        }

        #region events

        private void FrmMesh_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                prevX = curX;
                PrevY = curY;
                Point mousePosNow = e.Location;
                float deltaX = mousePosNow.X - mousePos.X;
                float deltaY = mousePosNow.Y - mousePos.Y;

                curX = (prevX + deltaX);
                curY = (PrevY + deltaY);
                this.Location = new Point((int)curX, (int)curY);
            }
        }

        private void FrmMesh_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        #endregion

    }
}
