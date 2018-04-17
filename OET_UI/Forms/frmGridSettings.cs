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
    public partial class frmGridSettings : Form
    {
        public frmGridSettings(GlobalDocument GD)
        {
            InitializeComponent();
            CenterToParent();

            _height = GD.GridHeight;
            _width  = GD.GridWidth;
            _size   = GD.GridSize;
            prevX   = 0;
            PrevY   = 0;
            curX    = 0;
            curY    = 0;
            a       = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
            SubscribeEvents();

        }

        private char    a;
        private float   _height;
        private float   _width;
        private int     _size;
        private PointF  mousePos;
        private float   prevX;
        private float   PrevY;
        private float   curX;
        private float   curY;

        public float    GridHeight
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }
        public float    GridWidth
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
            }
        }
        public int      GridSize
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
            }
        }

        private void SubscribeEvents()
        {
            this.MouseDown += FrmGridSettings_MouseDown;
            this.MouseMove += FrmGridSettings_MouseMove;
            btnOk.Click += BtnOk_Click;

            txtHeight.Text = _height.ToString(); ;
            txtWidth.Text = _width.ToString();
            txtSize.Text = _size.ToString();

            txtHeight.LostFocus += TxtHeight_TextChanged;
            txtWidth.LostFocus += TxtWidth_TextChanged;
            txtSize.LostFocus += TxtSize_TextChanged;

            txtHeight.KeyPress += TxtHeight_KeyPress;
            txtWidth.KeyPress += TxtWidth_KeyPress;
            txtSize.KeyPress += TxtSize_KeyPress;

            txtHeight.Click += TxtHeight_Click;
            txtWidth.Click += TxtWidth_Click;
            txtSize.Click += TxtSize_Click;
        }

        #region events

        private void TxtSize_Click(object sender, EventArgs e)
        {
            txtSize.SelectAll();
        }

        private void TxtWidth_Click(object sender, EventArgs e)
        {
            txtWidth.SelectAll();
        }

        private void TxtHeight_Click(object sender, EventArgs e)
        {
            txtHeight.SelectAll();
        }

        private void FrmGridSettings_MouseMove(object sender, MouseEventArgs e)
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

        private void FrmGridSettings_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        private void TxtHeight_TextChanged(object sender, EventArgs e)
        {
            if (txtHeight.Text == "" || txtHeight.Text == a.ToString())
                _height = 0;
            else
                _height = Convert.ToSingle(txtHeight.Text);
            txtHeight.Text = _height == 0 ? a + "0" : txtHeight.Text;
        }
        private void TxtWidth_TextChanged(object sender, EventArgs e)
        {
            if (txtWidth.Text == "" || txtWidth.Text == a.ToString())
                _width = 0;
            else
                _width = Convert.ToSingle(txtWidth.Text);
            txtWidth.Text = _width == 0 ? a + "0" : txtWidth.Text;
        }
        private void TxtSize_TextChanged(object sender, EventArgs e)
        {
            if (txtSize.Text == "")
                _size = 0;
            else
                _size = Convert.ToInt32(txtSize.Text);
            txtSize.Text = _size == 0 ? "0" : txtSize.Text;
        }

        private void TxtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtHeight.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtWidth.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        #endregion

    }
}
