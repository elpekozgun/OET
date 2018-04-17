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
    public partial class frmMesh : Form
    {
        public frmMesh(GlobalDocument GD)
        {
            InitializeComponent();
            CenterToParent();

            _meshSize   = GD.MeshSize;
            _horizon    = GD.Horizon;
            _nodeCount  = GD.Nodes.Count;
            prevX       = 0;
            PrevY       = 0;
            curX        = 0;
            curY        = 0;
            a           = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);


            PrepareUI();
            SubscribeEvents();
        }

        private char a;
        private float _meshSize;
        private float _horizon;
        private eBoundary _boundary;
        private int _nodeCount;

        private PointF mousePos;
        private float prevX;
        private float PrevY;
        private float curX;
        private float curY;

        public float MeshSize
        {
            get
            {
                return _meshSize;
            }

            set
            {
                _meshSize = value;
            }
        }
        public float Horizon
        {
            get
            {
                return _horizon;
            }

            set
            {
                _horizon = value;
            }
        }
        public eBoundary Boundary
        {
            get
            {
                return _boundary;
            }

            set
            {
                _boundary = value;
            }
        }
        private List<eBoundary> comboList = Enum.GetValues(typeof(eBoundary)).Cast<eBoundary>().ToList();

        private void PrepareUI()
        {
            foreach (var item in comboList)
            {
                cmbBoundary.Items.Add(item);
            }
            cmbBoundary.SelectedIndex = 0;
        }

        private void SubscribeEvents()
        {
            this.MouseDown += FrmMesh_MouseDown;
            this.MouseMove += FrmMesh_MouseMove;
            btnRun.Click += BtnRun_Click;

            txtMeshSize.Text = _meshSize.ToString(); ;
            txtHorizon.Text = _horizon.ToString();

            txtMeshSize.LostFocus += TxtMeshSize_LostFocus;
            txtHorizon.LostFocus += TxtHorizon_LostFocus;

            txtMeshSize.KeyPress += TxtMeshSize_KeyPress;
            txtHorizon.KeyPress += TxtHorizon_KeyPress;

            txtMeshSize.Click += TxtMeshSize_Click;
            txtHorizon.Click += TxtHorizon_Click;

            cmbBoundary.SelectedIndexChanged += CmbBoundary_SelectedIndexChanged;
        }

        private void CmbBoundary_SelectedIndexChanged(object sender, EventArgs e)
        {
            _boundary =  comboList[cmbBoundary.SelectedIndex];
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

        private void TxtMeshSize_Click(object sender, EventArgs e)
        {
            txtMeshSize.SelectAll();
        }

        private void TxtHorizon_Click(object sender, EventArgs e)
        {
            txtHorizon.SelectAll();
        }

        private void TxtHorizon_LostFocus(object sender, EventArgs e)
        {
            if (txtHorizon.Text == "" || txtHorizon.Text == a.ToString())
                _horizon = 0;
            else
                _horizon = Convert.ToSingle(txtHorizon.Text);
            txtHorizon.Text = _horizon == 0 ? a + "0" : txtHorizon.Text;
        }

        private void TxtMeshSize_LostFocus(object sender, EventArgs e)
        {
            if (txtMeshSize.Text == "" || txtMeshSize.Text == a.ToString())
                _meshSize = 0;
            else
                _meshSize = Convert.ToSingle(txtMeshSize.Text);
            txtMeshSize.Text = _meshSize == 0 ? a + "0" : txtMeshSize.Text;

            _horizon = (float)(Math.Sqrt(_meshSize * _meshSize + _meshSize * _meshSize) + 0.1);
            txtHorizon.Text = _horizon.ToString("#.###");
        }

        private void TxtHorizon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == a && txtHorizon.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtMeshSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            char a = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
            if (e.KeyChar == a && txtMeshSize.Text.Contains(a.ToString()) == false)
            {
                e.Handled = false;
                e.KeyChar = a;
            }
            else
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (_nodeCount != 0)
            {
                DialogResult messageBox = MessageBox.Show("all previous mesh data will be deleted, proceed?", "Mesh", MessageBoxButtons.OKCancel);
                if (messageBox == DialogResult.OK)
                    this.DialogResult = DialogResult.OK;
            }
            else
                this.DialogResult = DialogResult.OK;
        }

        #endregion

    }
}
