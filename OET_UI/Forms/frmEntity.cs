using OET_Types.Entities;
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
    public partial class frmEntity : Form
    {
        public frmEntity(IEntity entity)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.TopMost = true;

            switch (entity.EntityType)
            {
                case eEntityType.area:
                    {
                        tabControl.SelectedTab = tabPage1;
                        _area = (Area)entity;
                        btnColorArea.BackColor = _area.Color;
                        btnColorArea.ForeColor = Color.FromArgb(255, 255 - _area.Color.R, 255 - _area.Color.G, 255 - _area.Color.B);
                        txtThickness.Text = _area.Thickness.ToString();
                        txtAID.Text = _area.ID.ToString();
                    }
                    break;
                case eEntityType.segment:
                    {
                        tabControl.SelectedTab = tabPage2;
                        _segment = (Segment)entity;
                        btnColorSegment.BackColor = _segment.Color;
                        btnColorSegment.ForeColor = Color.FromArgb(255, 255 - _segment.Color.R, 255 - _segment.Color.G, 255 - _segment.Color.B);
                        txtRebarCount.Text = _segment.Count.ToString();
                        txtRebarSize.Text = _segment.Size.ToString();
                        txtSID.Text = _segment.ID.ToString();
                    }
                    break;
                case eEntityType.dot:
                    {
                        tabControl.SelectedTab = tabPage3;
                        _dot = (Dot)entity;
                        btnColorDot.BackColor = _dot.Color;
                        btnColorDot.ForeColor = Color.FromArgb(255, 255 - _dot.Color.R, 255 - _dot.Color.G, 255 - _dot.Color.B);
                        txtLoadX.Text = _dot.LoadX.ToString();
                        txtLoadY.Text = _dot.LoadY.ToString();
                        txtDID.Text = _dot.ID.ToString();
                        chkX.Checked = _dot.XRestrained;
                        chkY.Checked = _dot.YRestrained;
                    }
                    break;
                case eEntityType.invalid:
                    break;
            }

            a = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

            SubscribeEvents();
        }

        private Area        _area;
        private Segment     _segment;
        private Dot         _dot;
        private PointF      mousePos;
        private float       prevX;
        private float       PrevY;
        private float       curX;
        private float       curY;
        private char        a;

        public Dot Dot
        {
            get
            {
                return _dot;
            }

            set
            {
                _dot = value;
            }
        }
        public Segment Segment
        {
            get
            {
                return _segment;
            }

            set
            {
                _segment = value;
            }
        }
        public Area Area
        {
            get
            {
                return _area;
            }

            set
            {
                _area = value;
            }
        }

        private void SubscribeEvents()
        {
            tabControl.TabPages[0].MouseMove += FrmEntity_MouseMove;
            tabControl.TabPages[0].MouseDown += FrmEntity_MouseDown;
            tabControl.TabPages[1].MouseMove += FrmEntity_MouseMove;
            tabControl.TabPages[1].MouseDown += FrmEntity_MouseDown;
            tabControl.TabPages[2].MouseMove += FrmEntity_MouseMove;
            tabControl.TabPages[2].MouseDown += FrmEntity_MouseDown;

            btnColorArea.Click      += BtnColorArea_Click;
            btnColorSegment.Click   += BtnColorSegment_Click;
            btnColorDot.Click       += BtnColorDot_Click;

            btnOkArea.Click         += BtnOk_Click;
            btnOkSegment.Click      += BtnOkSegment_Click;
            btnOkDot.Click          += BtnOkDot_Click;

            txtThickness.LostFocus  += TxtThickness_LostFocus;
            txtRebarCount.LostFocus += TxtRebarCount_LostFocus;
            txtRebarSize.LostFocus  += TxtRebarSize_LostFocus;
            txtLoadY.LostFocus      += TxtLoadY_LostFocus;
            txtLoadX.LostFocus      += TxtLoadX_LostFocus;

            txtThickness.KeyPress   += TxtThickness_KeyPress;
            txtRebarCount.KeyPress  += TxtRebarCount_KeyPress;
            txtRebarSize.KeyPress   += TxtRebarSize_KeyPress;
            txtLoadY.KeyPress       += TxtLoadY_KeyPress;
            txtLoadX.KeyPress       += TxtLoadX_KeyPress;

            txtAID.KeyPress += TxtAID_KeyPress;
            txtDID.KeyPress += TxtDID_KeyPress;
            txtSID.KeyPress += TxtSID_KeyPress;

            txtAID.LostFocus += TxtAID_LostFocus;
            txtDID.LostFocus += TxtDID_LostFocus;
            txtSID.LostFocus += TxtSID_LostFocus;

            chkX.CheckedChanged += ChkX_CheckedChanged;
            chkY.CheckedChanged += ChkY_CheckedChanged;
            this.KeyDown += FrmEntity_KeyDown;

        }

        private void TxtSID_LostFocus(object sender, EventArgs e)
        {
            if (txtSID.Text == "" || txtSID.Text == a.ToString())
                _segment.ID = 0;
            else
                _segment.ID = Convert.ToInt32(txtSID.Text);
            txtSID.Text = _segment.ID == 0 ? "0" : txtSID.Text;
        }

        private void TxtDID_LostFocus(object sender, EventArgs e)
        {
            if (txtDID.Text == "" || txtDID.Text == a.ToString())
                _dot.ID = 0;
            else
                _dot.ID = Convert.ToInt32(txtDID.Text);
            txtDID.Text = _dot.ID == 0 ? "0" : txtDID.Text;
        }

        private void TxtAID_LostFocus(object sender, EventArgs e)
        {
            if (txtAID.Text == "" || txtAID.Text == a.ToString())
                _area.ID = 0;
            else
                _area.ID = Convert.ToInt32(txtAID.Text);
            txtAID.Text = _area.ID == 0 ? "0" : txtAID.Text;
        }

        private void TxtSID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtDID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }

        private void TxtAID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }

        private void FrmEntity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void FrmEntity_MouseMove(object sender, MouseEventArgs e)
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

        private void FrmEntity_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        private void TxtLoadX_LostFocus(object sender, EventArgs e)
        {
            if (txtLoadX.Text == "" || txtLoadX.Text == a.ToString())
                _dot.LoadX = 0;
            else
                _dot.LoadX = Convert.ToSingle(txtLoadX.Text);
            txtLoadX.Text = _dot.LoadX == 0 ? a + "0" : txtLoadX.Text;
        }
        private void TxtLoadY_LostFocus(object sender, EventArgs e)
        {
            if (txtLoadY.Text == "" || txtLoadY.Text == a.ToString())
                _dot.LoadY = 0;
            else
                _dot.LoadY = Convert.ToSingle(txtLoadY.Text);
            txtLoadY.Text = _dot.LoadY == 0 ? a + "0" : txtLoadY.Text;
        }
        private void TxtRebarSize_LostFocus(object sender, EventArgs e)
        {
            if (txtRebarSize.Text == "" || txtRebarSize.Text == a.ToString())
                _segment.Size = 0;
            else
                _segment.Size = Convert.ToInt32(txtRebarSize.Text);
            txtRebarSize.Text = _segment.Size == 0 ? a + "0" : txtRebarSize.Text;
        }
        private void TxtRebarCount_LostFocus(object sender, EventArgs e)
        {
            if (txtRebarCount.Text == "" || txtRebarCount.Text == a.ToString())
                _segment.Count = 0;
            else
                _segment.Count = Convert.ToByte(txtRebarCount.Text);
            txtRebarCount.Text = _segment.Count == 0 ? a + "0" : txtRebarCount.Text;
        }
        private void TxtThickness_LostFocus(object sender, EventArgs e)
        {
            if (txtThickness.Text == "" || txtThickness.Text == "0")
                MessageBox.Show("thickness cant be 0", "", MessageBoxButtons.OK);
            else
                _area.Thickness = Convert.ToSingle(txtThickness.Text);
        }

        private void TxtThickness_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtThickness.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtRebarCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtRebarSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtLoadY_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtLoadY.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtLoadY.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void TxtLoadX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtLoadX.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else if (e.KeyChar == '-' && txtLoadX.Text.Contains("-") == false)
            {
                e.Handled = false;
                e.KeyChar = '-';
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void BtnColorArea_Click(object sender, EventArgs e)
        {
            DialogResult colorResult = colorDialog1.ShowDialog();
            if (colorResult == DialogResult.OK)
            {
                var color = Color.FromArgb(160, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                btnColorArea.BackColor = color;
                _area.Color = color;
            }
        }
        private void BtnColorSegment_Click(object sender, EventArgs e)
        {
            DialogResult colorResult = colorDialog1.ShowDialog();
            if (colorResult == DialogResult.OK)
            {
                btnColorSegment.BackColor = colorDialog1.Color;
                _segment.Color = colorDialog1.Color;
            }
        }
        private void BtnColorDot_Click(object sender, EventArgs e)
        {
            DialogResult colorResult = colorDialog1.ShowDialog();
            if (colorResult == DialogResult.OK)
            {
                btnColorDot.BackColor = colorDialog1.Color;
                btnColorDot.ForeColor = Color.FromArgb(255, 255 - colorDialog1.Color.R, 255 - colorDialog1.Color.G, 255 - colorDialog1.Color.B);
                _dot.Color = colorDialog1.Color;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        private void BtnOkDot_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }
        private void BtnOkSegment_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }

        private void ChkX_CheckedChanged(object sender, EventArgs e)
        {
            if (chkX.Checked)
                _dot.XRestrained = true;
            else
                _dot.XRestrained = false;
        }
        private void ChkY_CheckedChanged(object sender, EventArgs e)
        {
            if (chkY.Checked)
                _dot.YRestrained = true;
            else
                _dot.YRestrained = false;
        }
    }
}
