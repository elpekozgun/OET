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
    public partial class frmStatus : Form
    {
        public frmStatus(GlobalDocument GD)
        {
            InitializeComponent();
            CenterToParent();

            _gd = GD;

            prevX   = 0;
            PrevY   = 0;
            curX    = 0;
            curY    = 0;

            SubscribeEvents();

        }

        private GlobalDocument _gd;
        private PointF  mousePos;
        private float   prevX;
        private float   PrevY;
        private float   curX;
        private float   curY;

        private void SubscribeEvents()
        {
            this.MouseDown += FrmStatus_MouseDown;
            this.MouseMove += FrmStatus_MouseMove;
            btnOk.Click += BtnOk_Click;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void FrmStatus_MouseMove(object sender, MouseEventArgs e)
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

        private void FrmStatus_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }
    }
}
