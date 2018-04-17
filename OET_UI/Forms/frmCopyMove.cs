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
    public partial class frmCopyMove : Form
    {
        public frmCopyMove(eAction action)
        {
            InitializeComponent();
            CenterToParent();

            _X      = 0;
            _Y      = 0;
            _count  = 1;
            _action = action;
            prevX   = 0;
            PrevY   = 0;
            curX    = 0;
            curY    = 0;
            a       = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

            PrepareUI();
            SubscribeEvents();

        }

        private PointF  mousePos;
        private float   prevX;
        private float   PrevY;
        private float   curX;
        private float   curY;
        private eAction _action;
        private float   _X;
        private float   _Y;
        private int     _count;
        private char    a;

        private void PrepareUI()
        {
            txtCopyX.Text = _X.ToString();
            txtCopyY.Text = _Y.ToString();
            txtMoveX.Text = _X.ToString();
            txtMoveY.Text = _Y.ToString();
            txtCopyCount.Text = _count.ToString();

            if (_action == eAction.copy)
            {
                tabControl1.SelectedTab = tabPage1;
                txtCopyX.SelectAll();
            }
            else if (_action == eAction.move)
            {
                tabControl1.SelectedTab = tabPage2;
                txtMoveX.SelectAll();
            }
        }

        private void SubscribeEvents()
        {
            tabControl1.DrawItem    += tabControl1_DrawItem;
            this.MouseMove          += FrmCopy_MouseMove;
            this.MouseDown          += FrmCopy_MouseDown;
            btnOk.Click             += BtnOk_Click;
                
            txtCopyX.LostFocus      += TxtCopyX_LostFocus;
            txtCopyY.LostFocus      += TxtCopyY_LostFocus;
            txtCopyCount.LostFocus  += TxtCopyCount_LostFocus;
            txtMoveX.LostFocus      += TxtMoveX_LostFocus;
            txtMoveY.LostFocus      += TxtMoveY_LostFocus;

            txtCopyX.KeyPress       += TxtCopyX_KeyPress;
            txtCopyY.KeyPress       += TxtCopyY_KeyPress;
            txtCopyCount.KeyPress   += TxtCopyCount_KeyPress;
            txtMoveX.KeyPress       += TxtMoveX_KeyPress;
            txtMoveY.KeyPress       += TxtMoveY_KeyPress;

            txtCopyX.Click          += TxtCopyX_Click;
            txtCopyY.Click          += TxtCopyY_Click;
            txtCopyCount.Click      += TxtCopyCount_Click;
            txtMoveX.Click          += TxtMoveX_Click;
            txtMoveY.Click          += TxtMoveY_Click;
        }

        public void GetCopyMoveValues(ref float X,ref float Y, ref int count,ref eAction act)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                act = eAction.copy;
                count = _count;
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                act = eAction.move;
                count = 1;
            }

            X = _X;
            Y = _Y;
            
        }

        #region Events

        private void TxtCopyX_Click(object sender, EventArgs e)
        {
            txtCopyX.SelectAll();
        }

        private void TxtCopyY_Click(object sender, EventArgs e)
        {
            txtCopyY.SelectAll();
        }

        private void TxtCopyCount_Click(object sender, EventArgs e)
        {
            txtCopyCount.SelectAll();
        }

        private void TxtMoveX_Click(object sender, EventArgs e)
        {
            txtMoveX.SelectAll();
        }

        private void TxtMoveY_Click(object sender, EventArgs e)
        {
            txtMoveY.SelectAll();
        }

        private void FrmCopy_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }  

        private void FrmCopy_MouseMove(object sender, MouseEventArgs e)
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

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl1.TabPages[e.Index];
            Rectangle rect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            e.Graphics.FillRectangle(new SolidBrush(page.BackColor),rect);

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -3 :0;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, page.ForeColor);
        }

        private void TxtMoveY_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtMoveY.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtMoveY.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtMoveX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtMoveX.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtMoveX.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtCopyY_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtCopyY.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtCopyY.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtCopyCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtCopyX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtCopyX.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtCopyX.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtMoveY_LostFocus(object sender, EventArgs e)
        {
            if (txtMoveY.Text == "" || txtMoveY.Text == a.ToString())
                _Y = 0;
            else
                _Y = Convert.ToSingle(txtMoveY.Text);
            txtMoveY.Text = _Y == 0 ? a + "0" : txtMoveY.Text;
        }

        private void TxtMoveX_LostFocus(object sender, EventArgs e)
        {
            if (txtMoveX.Text == "" || txtMoveX.Text == a.ToString())
                _X = 0;
            else
                _X = Convert.ToSingle(txtMoveX.Text);
            txtMoveX.Text = _X == 0 ? a + "0" : txtMoveX.Text;
        }

        private void TxtCopyCount_LostFocus(object sender, EventArgs e)
        {
            if (txtCopyCount.Text == "")
                _count = 0;
            else
                _count = Convert.ToInt32(txtCopyCount.Text);
            txtCopyCount.Text = _count == 0 ? "0" : txtCopyCount.Text;
        }

        private void TxtCopyY_LostFocus(object sender, EventArgs e)
        {
            if (txtCopyY.Text == "" || txtCopyY.Text == a.ToString())
                _Y = 0;
            else
                _Y = Convert.ToSingle(txtCopyY.Text);
            txtCopyY.Text = _Y == 0 ? a + "0" : txtCopyY.Text;
        }

        private void TxtCopyX_LostFocus(object sender, EventArgs e)
        {
            if (txtCopyX.Text == "" || txtCopyX.Text == a.ToString())
                _X = 0;
            else
                _X = Convert.ToSingle(txtCopyX.Text);
            txtCopyX.Text = _X == 0 ? a + "0" : txtCopyX.Text;
        }

        #endregion

    }
}
