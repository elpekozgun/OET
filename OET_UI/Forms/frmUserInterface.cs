using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using OET_2DEngine;
using System;
using OET_Types.Entities;
using OET_Types.Elements;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using GNUPlot;
using System.Threading.Tasks;
using System.ComponentModel;

namespace OET_UI
{
    public enum eDrawingObject : byte
    {
        segment = 0,
        Area = 1,
        Dot = 2,
        selectionBox = 3,
        zoombox = 4,
        deselect = 5,
        nothing = 255
    }

    public enum eObjectOver : byte
    {
        overCorner = 0,
        overDot = 1,
        overSegment = 2,
        overArea = 3,
        nothing = 4,
    }

    public enum eAction : byte
    {
        idle = 0,
        draw = 1,
        copy = 2,
        move = 3,
        pan = 4
    }

    public partial class frmUserInterface : Form
    {
        #region Private Fields
       
        private float               zoom                =       1;
        private float               maxzoom             =       20f;
        private float               minzoom             =       0.2f;
        private float               prevX               =       0;
        private float               PrevY               =       0;
        private float               curX                =       0;
        private float               curY                =       0;
        private float               dX;
        private float               dY;
        private int                 count;

        private eDrawingObject      drawingObject       =       eDrawingObject.nothing;
        private eAction             action              =       eAction.idle;
        private eObjectOver         objectOver          =       eObjectOver.nothing;
        private Area                newArea             =       new Area(new List<PointF>());
        private Segment             newSegment          =       new Segment(new List<PointF>());
        private Dot                 newDot              =       new Dot(new PointF());
        private IEntity             hitEntity;           
        private PointF              hitCoord;
        private Cursor              cursor              =       Cursors.Arrow   ;

        private PointF              mousePos;
        private Point               pos0;
        private float               width;
        private float               height;
        private Rectangle           selectionRectangle;
        private Rectangle           zoomRectangle;
        private TextBox             inputBox;
        private int                 selectedCount;
        private GlobalDocument      GD;
        private char                a = Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
        private frmEntity           frmEntityProp;
        private frmStatus           statusForm;
        private int                 time = 0;
        private System.Windows.Forms.Timer timer; 

        #endregion

        #region Ctor

        public frmUserInterface()
        {
            GD = new GlobalDocument();

            timer= new System.Windows.Forms.Timer();


            InitializeComponent();
            PrepareUI();
            SubscribeEvents();
            ZoomExtent();
        }

        private void SubscribeEvents()
        {
            canvas.Resize                   +=  Canvas_Resize;
            canvas.MouseUp                  +=  Canvas_MouseUp;
            canvas.MouseWheel               +=  Canvas_MouseWheel;
            canvas.MouseDown                +=  Canvas_MouseDown;
            canvas.MouseMove                +=  Canvas_MouseMove;
            canvas.MouseClick               +=  Canvas_MouseClick;
            canvas.MouseDoubleClick         +=  Canvas_MouseDoubleClick;
            canvas.Paint                    +=  Canvas_Paint;
            btnTsDrawArea.Click             +=  BtnTsDrawArea_Click;
            btnTsDrawDot.Click              +=  BtnTsDrawDot_Click;
            btnTsDrawLine.Click             +=  BtnTsDrawLine_Click;
            btnTsEntityProp.Click           +=  BtnTsEntityProp_Click;

            btnTsCopy.Click                 +=  BtnTsCopy_Click;
            btnTsMove.Click                 +=  BtnTsMove_Click;
            btnTsExport.Click               +=  BtnTsExport_Click;
            btnTsNew.Click                  +=  BtnTsNew_Click;
            btnTsSave.Click                 +=  BtnTsSave_Click;
            btnTsOpen.Click                 +=  BtnTsOpen_Click;
            btnTsZoomIn.Click               +=  BtnTsZoomIn_Click;
            btnTsZoomOut.Click              +=  BtnTsZoomOut_Click;
            btnTsZoomExtent.Click           +=  BtnTsZoomExtent_Click;
            btnTsZoomRectangle.Click        +=  BtnTsZoomRectangle_Click;
            btnTsPan.Click                  +=  BtnTsPan_Click;
            btnTsMesh.Click                 +=  BtnTsMesh_Click;
            btnTsDelete.Click               +=  BtnTsDelete_Click;
            btnTsAbout.Click                +=  BtnTsAbout_Click;
            btnTsGridSettings.Click         +=  BtnTsGridSettings_Click;
            btnTsOlm.Click                  +=  BtnTsOlm_Click;
            btnTsGnuPlot.Click              +=  BtnTsGnuPlot_Click;
            cmbUnits.SelectedIndexChanged   += CmbUnits_SelectedIndexChanged;

            this.KeyDown                    +=  FrmUserInterface_KeyDown;

        }

        private void PrepareUI()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            CenterToScreen();
            this.BackColor = Color.FromArgb(255, 60, 60, 60);
            canvas.BackColor = Color.FromArgb(255, 30, 30, 30);
            this.KeyPreview = true;
            chkSnapGrid.Checked = true;

            toolStrip1.BackColor = Color.FromArgb(255, 60, 60, 60);
            toolStrip1.Renderer = new MySR();

            cmbUnits.Items.Add(eUnit.kN_m.ToString().Replace('_', '.'));
            cmbUnits.Items.Add(eUnit.N_mm.ToString().Replace('_', '.'));
            cmbUnits.SelectedItem = GD.Unit.ToString().Replace('_','.');
            
        }

        #endregion

        #region Draw Methods

        private void DrawGrid(Graphics g)
        {
            int grid = GD.GridSize;
            int truncWidth = (int)(GD.GridWidth - (GD.GridWidth % grid));
            int truncHeight = (int)(GD.GridHeight - (GD.GridHeight % grid));

            Pen pen = new Pen(Color.FromArgb(50,216,216,216), 0);

            for (float i = 0 ; i <= truncWidth; i += grid)
            {
                g.DrawLine(pen, i, 0 , i, truncHeight);
            }
            for (float j = 0 ; j <= truncHeight; j += grid)
            {
                g.DrawLine(pen, 0 , j, truncWidth, j);
            }
        }

        private void DrawOrigin(Graphics g)
        {
            Brush red   =   new SolidBrush(Color.Red);
            Brush green =   new SolidBrush(Color.LimeGreen);
            Pen penX    =   new Pen(red, 0.04f * GD.GridSize);
            Pen penY    =   new Pen(green, 0.04f * GD.GridSize );

            //draw X arm of origin.
            g.DrawLine(penX, 0, 0, GD.GridSize, 0);
            g.DrawLine(penX, GD.GridSize, 0, (float)GD.GridSize - (float)GD.GridSize / 5,   (float)GD.GridSize / 7);
            g.DrawLine(penX, GD.GridSize, 0, (float)GD.GridSize - (float)GD.GridSize / 5, - (float)GD.GridSize / 7);

            //draw Y arm of origin.
            g.DrawLine(penY, 0,0, 0, GD.GridSize);
            g.DrawLine(penY, 0, GD.GridSize, (float)-GD.GridSize / 7, (float)GD.GridSize - (float)GD.GridSize / 5);
            g.DrawLine(penY, 0, GD.GridSize, (float)+GD.GridSize / 7, (float)GD.GridSize - (float)GD.GridSize / 5);

        }

        private void DrawAll(Graphics g )
        {
            if (GD.Entities != null)
            {
                SortEntities();
                foreach (Area area in GD.Entities.FindAll(x => x.EntityType == eEntityType.area))
                {
                    g.FillPolygon(new SolidBrush(area.Color), area.Points.ToArray());
                    g.DrawPolygon(new Pen(Color.FromArgb(160, 204, 50, 50), 0.2f * GD.GridSize / 10), area.Points.ToArray());
                }
                foreach (Segment segment in GD.Entities.FindAll(x => x.EntityType == eEntityType.segment))
                {
                    g.DrawLine(new Pen(segment.Color, 0.2f * GD.GridSize / 10), segment.Points[0], segment.Points[1]);
                }
                foreach (Dot dot in GD.Entities.FindAll(x => x.EntityType == eEntityType.dot))
                {
                    if (dot.XRestrained ||dot.YRestrained)
                    {
                        RectangleF rect = new RectangleF(
                        (dot.Points[0].X - Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)), (dot.Points[0].Y - Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)),
                        (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)), (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)));
                        g.FillRectangle(new SolidBrush(dot.Color), rect);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(
                        (dot.Points[0].X - Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)), (dot.Points[0].Y - Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)),
                        (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)), (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.3f)));
                        g.FillEllipse(new SolidBrush(dot.Color), rect);
                    }
                }
            }
        }

        private void DrawEndPoints(Graphics g)
        {
            foreach (IEntity entity in GD.Entities)
            {
                if (entity.EntityType != eEntityType.dot)
                {
                    foreach (PointF corner in entity.Points)
                    {
                        RectangleF rect = new RectangleF(
                            (corner.X - Math.Max(GD.Nod * GD.GridSize / 5, 0.2f)), (corner.Y - Math.Max(GD.Nod * GD.GridSize / 5,0.2f)),
                            (2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.2f)), (2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.2f)));
                        g.FillEllipse(Brushes.Yellow, rect);
                        g.DrawEllipse(new Pen(Color.OrangeRed,  0.1f * GD.GridSize / 5), rect);
                    }
                }
            }
        }

        private void DrawSnapPoint(Graphics g)
        {
            if (action == eAction.draw)
            {
                RectangleF rect = new RectangleF(
                (mousePos.X - Math.Max(GD.Nod * GD.GridSize / 4, 0.4f)), (mousePos.Y - Math.Max(GD.Nod * GD.GridSize / 4, 0.4f)),
                (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.4f))  , (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.4f)) );
                g.FillEllipse(new SolidBrush(Color.FromArgb(180,240,230,255)), rect);
                g.DrawEllipse(new Pen(Color.FromArgb(255, 153, 51, 255), 0.4f), rect);
            }
        }

        private void DrawSelected(Graphics g)
        {
            Pen select_pen = new Pen(Color.White, 0.4f * GD.GridSize / 10);
            select_pen.DashStyle = DashStyle.Dash;

            foreach (var entity in GD.Entities.FindAll(x=>x.Selected == true))
            {
                if (entity.EntityType == eEntityType.area || entity.EntityType == eEntityType.segment)
                {
                    g.DrawPolygon(select_pen, entity.Points.ToArray());
                    foreach (PointF corner in entity.Points)
                    {
                        RectangleF rect = new RectangleF(
                            (corner.X - Math.Max(GD.Nod * GD.GridSize / 5, 0.4f)), (corner.Y - Math.Max(GD.Nod * GD.GridSize / 5, 0.4f)),
                            (2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.4f)), (2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.4f)));
                        g.FillEllipse(Brushes.White, rect);
                        g.DrawEllipse(new Pen(Color.White, 0.4f), rect);
                    }
                }
                else if (entity.EntityType == eEntityType.dot)
                {
                    RectangleF rect = new RectangleF(
                        entity.Points[0].X - Math.Max(GD.Nod * GD.GridSize / 4, 0.5f), entity.Points[0].Y - Math.Max(GD.Nod * GD.GridSize / 4, 0.5f),
                        (2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.5f)), 2 * Math.Max(GD.Nod * GD.GridSize / 4, 0.5f));
                    g.FillRectangle(new SolidBrush(Color.White), rect);
                }
            }
        }

        private void DrawNextEntity(Graphics g,PointF mouse)
        {
            Pen select_pen = new Pen(Color.Green, 0.5f * GD.GridSize / 10);
            select_pen.DashStyle = DashStyle.Dash;

            if (newArea != null)
            {
                if (newArea.Points.Count > 1)
                    g.DrawLines(select_pen, newArea.Points.ToArray());
                if (newArea.Points.Count >= 1)
                    g.DrawLine(select_pen, newArea.Points[newArea.Points.Count - 1], mouse);
            }
            foreach (PointF corner in newArea.Points)
            {
                RectangleF rect = new RectangleF(
                    corner.X - Math.Max(GD.Nod * GD.GridSize / 5, 0.3f), corner.Y - Math.Max(GD.Nod * GD.GridSize / 5, 0.3f),
                    (2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.3f)), 2 * Math.Max(GD.Nod * GD.GridSize / 5, 0.3f));
                g.FillEllipse(Brushes.LightGreen, rect);
                g.DrawEllipse(new Pen(Color.Green, 0.3f), rect);
            }

            if (newSegment != null && newSegment.Points.Count == 1)
                g.DrawLine(select_pen, newSegment.Points[0], mouse);

            foreach (PointF corner in newSegment.Points )
            {
                RectangleF rect = new RectangleF(
                    corner.X - GD.Nod * GD.GridSize / 5, corner.Y - GD.Nod * GD.GridSize / 5,
                    2 * GD.Nod * GD.GridSize / 5, 2 * GD.Nod * GD.GridSize / 5);
                g.FillEllipse(Brushes.LightGreen, rect);
                g.DrawEllipse(new Pen(Color.Green, 0.3f), rect);
            }

        }

        private void DrawCurrentCoordinate(Graphics g, PointF mouse)
        {
            txtStatus.Text = $"X: {mouse.X.ToString("#,0")} ;Y: {mouse.Y.ToString("#,0")}";


            //g.TranslateTransform(-curX - width / 2 - 2, curY - height / 2 + 1);
            //g.TranslateTransform(0, height);
            //g.ScaleTransform(1 / zoom, -1 / zoom);

            //var temp = new PointF(mouse.X,mouse.Y);
            //WorldToScreen(ref temp);

            //string location = $"X: {mouse.X.ToString("#,0")} ;Y: {mouse.Y.ToString("#,0")}";
            //g.DrawString(location, DefaultFont, Brushes.Aqua, temp.X + GD.GridSize , temp.Y );
        }

        private void SnapToGrid(ref PointF pt)
        {
            pt = new Point((int)pt.X, (int)pt.Y);

            pt.X = GD.GridSize * (int)Math.Round(pt.X /  GD.GridSize);
            pt.Y = GD.GridSize * (int)Math.Round(pt.Y /  GD.GridSize);
        }

        private void SnapToPoint(ref PointF pt)
        {

            foreach (IEntity entity in GD.Entities)
            {
                for (int i = 0; i < entity.Points.Count; i++)
                {
                    
                    if (Math.Abs(pt.X - entity.Points[i].X) <= GD.Nod &&
                        Math.Abs(pt.Y - entity.Points[i].Y) <= GD.Nod)
                    {
                        pt.X = entity.Points[i].X;
                        pt.Y = entity.Points[i].Y;
                    }
                }
            }
            for (int i = 0; i < newArea.Points.Count; i++)
            {
                if (Math.Abs(pt.X - newArea.Points[i].X) <= GD.Nod &&
                    Math.Abs(pt.Y - newArea.Points[i].Y) <= GD.Nod)
                {
                    pt.X = newArea.Points[i].X;
                    pt.Y = newArea.Points[i].Y;
                }
            }
            
        }

        private void Snap(ref PointF pt, bool snapGrid, bool snapPoint)
        {
            if (snapPoint)
            {
                SnapToPoint(ref pt);
            }
            if (snapGrid)
            {
                SnapToGrid(ref pt);
            }
        }

        private Rectangle MakeRectangle(int x0, int y0, int x1, int y1)
        {
            return new Rectangle(
                Math.Min(x0, x1),
                Math.Min(y0, y1),
                Math.Abs(x0 - x1),
                Math.Abs(y0 - y1));
        }

        #endregion

        #region Utility Methods

        private bool OverCornerPoint(PointF mousePoint,out IEntity hitArea, ref eObjectOver objectOver)
        {
            if (GD.Entities != null)
            {
                foreach (IEntity entity in GD.Entities)
                {
                    for (int i = 0; i < entity.Points.Count; i++)
                    {
                        if (FindDistanceToPointSquared(entity.Points[i], mousePoint) < (GD.Nod + 1) * (GD.Nod + 1))
                        {
                            objectOver = eObjectOver.overCorner;
                            hitCoord = entity.Points[i];
                            hitArea = entity;
                            return true;
                        }
                    }
                }
            }
            objectOver = eObjectOver.nothing;
            hitArea = null;
            return false;
        }

        private bool OverEntity(PointF mousePoint, out IEntity hitArea, out eObjectOver objectOver)
        {
            if (GD.Entities != null)
            {
                foreach (IEntity entity in GD.Entities)
                {
                    if (entity.EntityType == eEntityType.area)
                    {
                        for (int i = 0; i < entity.Points.Count; i++)
                        {
                            if (pointInArea(mousePos, entity))
                            {
                                objectOver = eObjectOver.overArea;
                                hitArea = entity;
                                return true;
                            }
                            else
                                objectOver = eObjectOver.nothing;
                        }
                    }
                    if (entity.EntityType == eEntityType.dot)
                    {
                        if (FindDistanceToPointSquared(entity.Points[0], mousePoint) < (GD.Nod + 1.5) * (GD.Nod + 1.5))
                        {
                            objectOver = eObjectOver.overDot;
                            hitArea = entity;
                            return true;
                        }
                        else
                            objectOver = eObjectOver.nothing;
                    }
                    if (entity.EntityType == eEntityType.segment)
                    {
                        PointF closest;
                        if (FindDistanceToSegmentSquared(mousePoint, entity.Points[0], entity.Points[1], out closest) < (GD.Nod + 1) * (GD.Nod + 1))
                        {
                            objectOver = eObjectOver.overSegment;
                            hitArea = entity;
                            hitCoord = closest;
                            return true;
                        }
                        else
                            objectOver = eObjectOver.nothing;
                    }
            
                }
            }
            objectOver = eObjectOver.nothing;
            hitArea = null;
            return false;
        }

        private float FindDistanceToPointSquared(PointF pt1, PointF pt2)
        {
            float dx = pt1.X - pt2.X;
            float dy = pt1.Y - pt2.Y;
            return dx * dx + dy * dy;
        }

        private float FindDistanceToSegmentSquared(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return (float)Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            // return Math.Sqrt(dx * dx + dy * dy);
            return dx * dx + dy * dy;
        }

        private bool pointInArea(PointF mousePoint, IEntity area)
        {
            int i, j;
            bool inside = false;
            int nvert = area.Points.Count;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((area.Points[i].Y > mousePoint.Y) != (area.Points[j].Y > mousePoint.Y)) &&
                 (mousePoint.X < (area.Points[j].X - area.Points[i].X) * (mousePoint.Y - area.Points[i].Y) / (area.Points[j].Y - area.Points[i].Y) + area.Points[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private void ScreenToWorld(ref PointF p)
        {
            p.X = p.X / zoom - canvas.Width / 2 - curX;
            p.Y = -p.Y / zoom + canvas.Height / 2 + curY;
        }

        private void WorldToScreen(ref PointF p)
        {
            p.X = zoom * (p.X + canvas.Width / 2 + curX);
            p.Y = zoom * (-p.Y + canvas.Height / 2 + curY);
        }

        private void SelectPointsWithRectangle()
        {
            Pen selectPen = new Pen(Color.Yellow);
            selectPen.DashStyle = DashStyle.Dash;

            PointF pt0 = new PointF(selectionRectangle.X, selectionRectangle.Y);
            PointF pt1 = new PointF(pt0.X + selectionRectangle.Width, pt0.Y + selectionRectangle.Height);
            ScreenToWorld(ref pt0);
            ScreenToWorld(ref pt1);

            foreach (var entity in GD.Entities) 
            {
                if (entity.Points.TrueForAll(x => x.X > pt0.X) &&
                    entity.Points.TrueForAll(x => x.X < pt1.X) &&
                    entity.Points.TrueForAll(x => x.Y < pt0.Y) &&
                    entity.Points.TrueForAll(x => x.Y > pt1.Y) &&
                    entity.Selected == false)
                {
                    entity.Selected = true;
                    selectedCount = GD.Entities.FindAll(x => x.Selected == true).Count;
                }
            }
        }

        private void ZoomWithRectangle()
        {
            var oldzoom = zoom;

            var xRatio = (float)zoomRectangle.Width / canvas.Width;
            var yRatio = (float)zoomRectangle.Height / canvas.Height;
            var ratio = Math.Max(Math.Abs(xRatio), Math.Abs(yRatio));

            if (xRatio != 0 || yRatio != 0)
                zoom /= ratio;
            if (zoom == 0)
                zoom = 1;

            float MoveWidth = zoomRectangle.Width * ratio;
            float MoveHeight = zoomRectangle.Height * ratio;

            float x = zoomRectangle.Location.X + MoveWidth / 2;
            float y = zoomRectangle.Location.Y + MoveHeight / 2;

            float oldimagex = (x / oldzoom);  
            float oldimagey = (y / oldzoom);

            float newimagex = (x / zoom);     
            float newimagey = (y / zoom);

            curX = curX + newimagex - oldimagex;
            curY = curY + newimagey - oldimagey;


            canvas.Invalidate();

        }

        private void StartDotForm()
        {
            if (frmEntityProp != null)
            {
                frmEntityProp.Close();
            }
            frmEntityProp = new frmEntity(newDot);
            frmEntityProp.btnOkDot.Visible = false;
            frmEntityProp.StartPosition = FormStartPosition.Manual;
            frmEntityProp.Location = new Point(this.Location.X + this.Width - 225, this.Location.Y + this.Height - 200);
            frmEntityProp.Show();
        }

        private void StartSegmentForm()
        {
            if (frmEntityProp != null)
            {
                frmEntityProp.Close();
            }
            frmEntityProp = new frmEntity(newSegment);
            frmEntityProp.btnOkSegment.Visible = false;
            frmEntityProp.StartPosition = FormStartPosition.Manual;
            frmEntityProp.Location = new Point(this.Location.X + this.Width - 225, this.Location.Y + this.Height - 200);
            frmEntityProp.Show();
        }

        private void StartAreaForm()
        {
            if (frmEntityProp != null)
            {
                frmEntityProp.Close();
            }
            frmEntityProp = new frmEntity(newArea);
            frmEntityProp.btnOkArea.Visible = false;
            frmEntityProp.StartPosition = FormStartPosition.Manual;
            frmEntityProp.Location = new Point(this.Location.X + this.Width - 225, this.Location.Y + this.Height - 200);
            frmEntityProp.Show();
        }

        private void startEntityEditForm()
        {
            List<IEntity> selecteds = new List<IEntity>();
            selecteds.AddRange(GD.Entities.FindAll(x => x.Selected == true));
            if (selectedCount != 0)
            {
                frmEntityProp = new frmEntity(selecteds[0]);
                frmEntityProp.StartPosition = FormStartPosition.Manual;
                frmEntityProp.Location = new Point(this.Location.X + this.Width - 225, this.Location.Y + this.Height - 200);
                frmEntityProp.ShowDialog();
                

                if (frmEntityProp.DialogResult == DialogResult.OK)
                {
                    if (selecteds[0].GetType() == typeof(Area))
                    {
                        foreach (Area area in selecteds.FindAll(x => x.EntityType == eEntityType.area))
                        {
                            area.Color = frmEntityProp.Area.Color;
                            area.Thickness = frmEntityProp.Area.Thickness;
                        }
                    }
                    else if (selecteds[0].GetType() == typeof(Segment))
                    {
                        foreach (Segment segment in selecteds.FindAll(x => x.EntityType == eEntityType.segment))
                        {
                            segment.Color = frmEntityProp.Segment.Color;
                            segment.Size = frmEntityProp.Segment.Size;
                            segment.Count = frmEntityProp.Segment.Count;
                        }
                    }
                    else if (selecteds[0].GetType() == typeof(Dot))
                    {
                        foreach (Dot dot in selecteds.FindAll(x => x.EntityType == eEntityType.dot))
                        {
                            dot.Color = frmEntityProp.Dot.Color;
                            dot.LoadX = frmEntityProp.Dot.LoadX;
                            dot.LoadY = frmEntityProp.Dot.LoadY;
                            dot.XRestrained = frmEntityProp.Dot.XRestrained;
                            dot.YRestrained = frmEntityProp.Dot.YRestrained;
                        }
                    }
                }
            }
            canvas.Invalidate();
        }

        private void AddSegment(PointF pt)
        {
            if (newSegment == null)
            {
                newSegment = new Segment(new List<PointF>());
            }
            newSegment.Points.Add(pt);
            
            if (newSegment.Points.Count > 1)
            {
                newSegment.ID = GD.Entities.FindAll(x => x.EntityType == eEntityType.segment).Count + 1;               
                GD.Entities.Add(newSegment);
                newSegment = new Segment(new List<PointF>());
            }
        }

        private void AddDot(PointF pt)
        {
            if (newDot == null)
                newDot = new Dot(pt);
            newDot.Points[0] = pt;

            GD.Entities.Add(newDot);
            newDot = new Dot(pt);
        }

        private void AddArea(PointF pt)
        {
            if (newArea == null)
                newArea = new Area(new List<PointF>());
            newArea.ID = GD.Entities.FindAll(x => x.EntityType == eEntityType.area).Count + 1;
            newArea.Thickness = frmEntityProp.Area.Thickness;
            newArea.Color = frmEntityProp.Area.Color;
            newArea.Points.Add(pt);
        }

        private void CopyEntities(float dX, float dY,int count)
        {
            var Copylist = GD.Entities.FindAll(x => x.Selected == true);

            foreach (var copy in Copylist)
            {
                if (copy.EntityType == eEntityType.area)
                {
                    Area temp = (Area)copy;
                    for (int i = 1; i <= count; i++)
                    {
                        Area a = new Area(new List<PointF>());
                        for (int j = 0; j < copy.Points.Count; j++)
                        {
                            a.Points.Add(new PointF(copy.Points[j].X + dX * i, copy.Points[j].Y + dY * i));
                        }
                        a.Thickness = temp.Thickness;
                        a.Color = temp.Color;
                        a.ID = GD.Entities.FindAll(x => x.EntityType == eEntityType.area).Count + 1;
                        GD.Entities.Add(a);
                        a.Selected = false;
                    }
                }
                else if (copy.EntityType == eEntityType.segment)
                {
                    Segment temp = (Segment)copy;
                    for (int i = 1; i <= count; i++)
                    {
                        Segment a = new Segment(new List<PointF>());
                        for (int j = 0; j < copy.Points.Count; j++)
                        {
                            a.Points.Add(new PointF(copy.Points[j].X + dX * i, copy.Points[j].Y + dY * i));
                        }
                        a.Count = temp.Count;
                        a.Size = temp.Size;
                        a.Color = temp.Color;
                        a.ID = GD.Entities.FindAll(x => x.EntityType == eEntityType.segment).Count + 1;
                        GD.Entities.Add(a);
                        a.Selected = false;
                    }
                }
                else if (copy.EntityType == eEntityType.dot)
                {
                    Dot temp = (Dot)copy;
                    for (int i = 1; i <= count; i++)
                    {
                        Dot a = new Dot(new PointF());
                        a.Points[0] = (new PointF(copy.Points[0].X + dX * i, copy.Points[0].Y + dY * i));

                        a.LoadX = temp.LoadX;
                        a.LoadY = temp.LoadY;
                        a.XRestrained = temp.XRestrained;
                        a.YRestrained = temp.YRestrained;
                        a.Color = temp.Color;
                        a.ID = GD.Entities.FindAll(x => x.EntityType == eEntityType.dot).Count + 1;
                        GD.Entities.Add(a);
                        a.Selected = false;
                    }
                }
            }
        }

        private void MoveEntities(float dX, float dY)
        {
            var moveList = GD.Entities.FindAll(x => x.Selected == true);

            foreach (var move in moveList)
            {
                for (int i = 0; i < move.Points.Count; i++)
                {
                    move.Points[i] = new PointF(move.Points[i].X + dX, move.Points[i].Y + dY);
                }
            }
        }

        private void SortEntities()
        {
            var tempList = new List<IEntity>();
            foreach (Dot entity in GD.Entities.FindAll(x => x.EntityType == eEntityType.dot))
            {
                tempList.Add(entity);
            }
            foreach (Segment entity in GD.Entities.FindAll(x => x.EntityType == eEntityType.segment))
            {
                tempList.Add(entity);
            }
            foreach (Area entity in GD.Entities.FindAll(x => x.EntityType == eEntityType.area))
            {
                tempList.Add(entity);
            }
            GD.Entities = tempList;
        }

        private void ZoomExtent()
        {
            var oldzoom = zoom;

            var xRatio = GD.GridWidth * zoom / canvas.Width;
            var yRatio = GD.GridHeight * zoom / canvas.Height;

            if (xRatio != 0 || yRatio != 0)
                zoom = oldzoom* 0.8f / Math.Max(Math.Abs(xRatio), Math.Abs(yRatio));
            else
                zoom = 1;

            curX = (-canvas.Width / 2 + canvas.Width / (2 * zoom) - GD.GridWidth / 2);
            curY = (-canvas.Height / 2 + GD.GridHeight/0.9f); ;

        }

        private void CopyCommand()
        {
            if (selectedCount > 0)
            {
                action = eAction.copy;
                frmCopyMove copyForm = new frmCopyMove(action);
                DialogResult = copyForm.ShowDialog();
                if (DialogResult == DialogResult.OK)
                {
                    copyForm.GetCopyMoveValues(ref dX, ref dY, ref count, ref action);
                    if (action == eAction.copy)
                        CopyEntities(dX, dY,count);
                    else if (action == eAction.move)
                        MoveEntities(dX, dY);
                }

                canvas.Invalidate();
            }
        }

        private void MoveCommand()
        {
            if (selectedCount > 0)
            {
                action = eAction.move;
                frmCopyMove copyForm = new frmCopyMove(action);
                copyForm.ShowDialog();
                DialogResult = copyForm.DialogResult;
                if (DialogResult == DialogResult.OK)
                {
                    copyForm.GetCopyMoveValues(ref dX, ref dY, ref count, ref action);

                    if (action == eAction.copy)
                        CopyEntities(dX, dY,count);
                    else if (action == eAction.move)
                        MoveEntities(dX, dY);
                }


                canvas.Invalidate();
            }
        }

        private void EscapeCommand()
        {
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
            zoomRectangle.Width = 0;
            zoomRectangle.Height = 0;

            canvas.Cursor = Cursors.Arrow;
            action = eAction.idle;
            drawingObject = eDrawingObject.nothing;
            foreach (var entity in GD.Entities.FindAll(x => x.Selected == true))
            {
                entity.Selected = false;
                selectedCount =  GD.Entities.FindAll(x => x.Selected == true).Count;
            }
            if (inputBox != null)
            {
                inputBox.Dispose();
                inputBox = null;
            }
        }

        private void MeshCommand()
        {
            frmMesh meshForm = new frmMesh(GD);
            meshForm.ShowDialog();
            DialogResult = meshForm.DialogResult;
            if (DialogResult == DialogResult.OK)
            {
                GD.MeshSize = meshForm.MeshSize;
                GD.Horizon = meshForm.Horizon;
                GD.BoundaryType = meshForm.Boundary;

                Task mesh = Task.Factory.StartNew(() => Mesh());

                timer.Interval = 1000;
                timer.Tick += new EventHandler(this.T_Tick);
                timer.Start();

            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            time++;
            txtStatus.Text = "meshing: " + time;
        }

        private void SaveCommand()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.ValidateNames = true;

            saveDialog.Filter = " oet (*.oet)| *.oet";
            saveDialog.DefaultExt = "oet";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                GD.FileName = saveDialog.FileName;
                GD.FolderName = Path.GetDirectoryName(GD.FileName);
                GD.ExportFolder = GD.ExportFolder = string.IsNullOrWhiteSpace(GD.ExportFolder) ? GD.FolderName : GD.ExportFolder;
                Stream stream = File.Open(GD.FileName, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, GD);
                stream.Close();
                this.Text = "OET " + GD.FileNameWithoutPath;
            }
        }

        private void OpenCommand()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.ValidateNames = true;

            openDialog.Filter = " oet (.oet)| *.oet | All Files (.*)| *.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                GD.FileName = openDialog.FileName;
                string validateName = GD.FileName.Substring(GD.FileName.Length - 4);
                if (validateName == ".oet" || validateName == ".OET")
                {
                    Stream stream = File.Open(GD.FileName, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    GD = (GlobalDocument)formatter.Deserialize(stream);
                    GD.ExportFolder = GD.ExportFolder = string.IsNullOrWhiteSpace(GD.ExportFolder) ? GD.FolderName : GD.ExportFolder;
                    stream.Close();
                    ZoomExtent();
                    this.Text = "OET " + GD.FileNameWithoutPath;
                }
                else
                {
                    MessageBox.Show("invalid data type");
                    OpenCommand();
                }
            }
        }

        private void NewCommand()
        {
            {
                DialogResult result = MessageBox.Show("Save before closing?", "new", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveCommand();
                    PrepareUI();
                    GD = new GlobalDocument();
                    this.Text = "OET " + GD.FileNameWithoutPath;
                }
                else if (result == DialogResult.No)
                {
                    PrepareUI();
                    GD = new GlobalDocument();
                    this.Text = "OET " + GD.FileNameWithoutPath;
                }
            }
        }

        private void ExportCommand()
        {
            if (GD.FileName == null)
            {
                SaveCommand();
            }
            else 
            {
                var result = MessageBox.Show("Would you like to select a different folder for export data? ", "export", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                        GD.ExportFolder = folderDialog.SelectedPath;
                }
                else
                    GD.ExportFolder = GD.FolderName;
            }
                GD.GenerateInputMesh();
                GD.GenerateInputBoundary();
                GD.GenerateNodeThicknes();
                GD.GenerateGnuPlotData();

            Task plottask = Task.Factory.StartNew(() => GnuPlotStartCommand());
        }

        private void OLMStartCommand()
        {
            string olm = @"C:\Users\ozgun\Desktop\OneDrive\OET\OET\OLM\olm\olm\Release\olm.exe";
            //int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo(olm);
            processInfo.WorkingDirectory = Path.GetDirectoryName(olm);

            process = Process.Start(processInfo);
            process.WaitForExit();
        }

        private void GnuPlotStartCommand()
        {
            string command = "";
            frmGnuPlotInput frm = new frmGnuPlotInput();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                command = frm.Command;

            string Pgm = @"C:\Program Files\gnuplot\bin\gnuplot.exe";
            //string Pgm = @"C:\Program Files (x86)\gnuplot\bin\gnuplot.exe";
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            extPro.StartInfo.CreateNoWindow = true;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            gnupStWr.WriteLine("plot '" + GD.ExportFolder + "\\" + GD.FileNameWithoutPath + @".plotData.dat'" + " " +command +"");
            gnupStWr.Flush();
            extPro.WaitForExit();

                // GnuPlot.Plot(GD.ExportFolder + "\\" + GD.FileNameWithoutPath + @".plotData.dat", "w l lc var");
            }
        }

        private void Mesh()
        {
            Engine.MeshSize = GD.MeshSize;
            GD.Clear();
            Engine.GenerateNodesByEntity(GD);
            Engine.GenerateFramesByNodes(GD);
            FillStatus();
        }

        private void FillStatus()
        {
            statusForm = new frmStatus(GD);
            statusForm.richTextBox1.AppendText("----------------------OET MESHER----------------------\n\n");
            statusForm.richTextBox1.AppendText("Mesh Completed in: " + time.ToString() + " seconds\n");
            statusForm.richTextBox1.AppendText("Unit: " + GD.Unit.ToString() + "\n");
            statusForm.richTextBox1.AppendText("Mesh Size: " + (GD.MeshSize * GD.Nmm).ToString("#.###") + "\n");
            statusForm.richTextBox1.AppendText("Horizon: " + (GD.Horizon * GD.Nmm).ToString("#.###") + "\n");
            statusForm.richTextBox1.AppendText("Total Area: " + (GD.TotalArea * GD.Nmm * GD.Nmm).ToString("#.###") + "\n");
            statusForm.richTextBox1.AppendText("Entity Count: " + GD.Entities.Count.ToString() + "\n");
            statusForm.richTextBox1.AppendText("Node Count: " + GD.Nodes.Count.ToString() + "\n");
            statusForm.richTextBox1.AppendText("Frame Count: " + GD.Frames.Count.ToString() + "\n");

            timer.Stop();
            timer.Tick -= T_Tick;
            time = 0;

            statusForm.ShowDialog();

        }

        #endregion

        #region Events

        private void FrmUserInterface_KeyDown(object sender, KeyEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case (Keys.S):
                        SaveCommand();
                        break;
                    case (Keys.O):
                        OpenCommand();
                        break;
                    case (Keys.C ):
                        CopyCommand();
                        break;
                    case (Keys.M):
                        MoveCommand();
                        break;
                    case (Keys.A):
                        action = eAction.idle;
                        GD.Entities.ForEach(x => x.Selected = true);
                        break;
                }
            }
            switch (e.KeyCode)
            {
                case Keys.S:
                    action = eAction.draw;
                    drawingObject = eDrawingObject.segment;
                    StartSegmentForm();
                    break;
                case Keys.A:
                    action = eAction.draw;
                    drawingObject = eDrawingObject.Area;
                    StartAreaForm();
                    break;
                case Keys.D:
                    action = eAction.draw;
                    drawingObject = eDrawingObject.Dot;
                    StartDotForm();
                    break;
                case Keys.Z:
                    action = eAction.idle;
                    drawingObject = eDrawingObject.zoombox;
                    ZoomWithRectangle();
                    break;
                case Keys.Escape:
                    EscapeCommand();
                    break;
                case Keys.Delete:
                    GD.Entities.RemoveAll(x => x.Selected == true);
                    break;
            }

            canvas.Invalidate();
        }

        private void ModifyPoint(ref PointF mousePos, bool snapGrid, bool snapPoint, MouseEventArgs e)
        {
            mousePos = new PointF(e.X, e.Y);
            ScreenToWorld(ref mousePos);
            Snap(ref mousePos, snapGrid, snapPoint);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Pan
            if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && action ==eAction.pan))
            {
                cursor = Cursors.SizeAll;
                Point mousePosNow = e.Location;
                float deltaX = mousePosNow.X - mousePos.X;  // the distance the mouse has been moved since mouse was pressed
                float deltaY = mousePosNow.Y - mousePos.Y;

                curX = (prevX + (deltaX / zoom));      // calculate new offset of image based on the current zoom factor
                curY = (PrevY + (deltaY / zoom));

                if (canvas.Cursor != cursor)
                {
                    canvas.Cursor = cursor;
                }

                canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Left && action == eAction.pan)
            {
                Point mousePosNow = e.Location;
                float deltaX = mousePosNow.X - mousePos.X;  // the distance the mouse has been moved since mouse was pressed
                float deltaY = mousePosNow.Y - mousePos.Y;

                curX = (prevX + (deltaX / zoom));      // calculate new offset of image based on the current zoom factor
                curY = (PrevY + (deltaY / zoom));

                canvas.Invalidate();
            }
            // move Entity
            else if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver == eObjectOver.overCorner )
            {
                foreach (IEntity entity in GD.Entities.FindAll(x => x.Points.Contains(hitCoord)))
                {
                    var index = entity.Points.IndexOf(hitCoord);
                    ModifyPoint(ref mousePos, chkSnapGrid.Checked, chkSnapPoint.Checked, e);
                    entity.Points[index] = mousePos;
                }
                hitCoord = mousePos;

                canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Left && action == eAction.idle && drawingObject == eDrawingObject.selectionBox)
            {
                using (Pen select_pen = new Pen(Color.Red))
                {
                    select_pen.DashStyle = DashStyle.Dash;
                    selectionRectangle = MakeRectangle(pos0.X, pos0.Y, e.X, e.Y);
                    canvas.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Left && action == eAction.idle && drawingObject == eDrawingObject.zoombox)
            {
                cursor = Cursors.UpArrow;
                using (Pen select_pen = new Pen(Color.Green))
                {
                    select_pen.DashStyle = DashStyle.Dash;
                    zoomRectangle = MakeRectangle(pos0.X, pos0.Y, e.X, e.Y);
                    canvas.Invalidate();
                }
                if (canvas.Cursor != cursor)
                {
                    canvas.Cursor = cursor;
                }
            }
            // idle
            else if(action == eAction.idle && drawingObject != eDrawingObject.zoombox)
            {
                cursor = Cursors.Arrow;
                PointF mousePoint = e.Location;
                ScreenToWorld(ref mousePoint);
                mousePos = mousePoint;
                if (frmEntityProp != null && selectedCount == 0)
                {
                    frmEntityProp.Close();
                }

                if (OverCornerPoint(mousePos, out hitEntity, ref objectOver))
                {
                    cursor = Cursors.Hand;
                }
                else if(OverEntity(mousePos,out hitEntity,out objectOver))
                {
                    cursor = Cursors.Hand;
                }
                if (canvas.Cursor != cursor)
                {
                    canvas.Cursor = cursor;
                }
                canvas.Invalidate();
            }
            // drawing
            else if (action == eAction.draw)
            {
                canvas.Cursor = Cursors.Cross;
                ModifyPoint(ref mousePos, chkSnapGrid.Checked, chkSnapPoint.Checked, e);

                canvas.Invalidate();
            }
         }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && action == eAction.pan))
            {
                mousePos = e.Location;
                prevX = curX;
                PrevY = curY;
            }
            if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver == eObjectOver.nothing &&
                drawingObject != eDrawingObject.zoombox)
            {
                drawingObject = eDrawingObject.selectionBox;
                pos0.X = e.X;
                pos0.Y = e.Y;
            }
            if (e.Button == MouseButtons.Left && drawingObject == eDrawingObject.zoombox)
            {
                pos0.X = e.X;
                pos0.Y = e.Y;
            }
            if (e.Button == MouseButtons.Left && action == eAction.draw)
            {
                if (drawingObject == eDrawingObject.Area)
                {
                    PointF pt = new PointF(e.X, e.Y);
                    ModifyPoint(ref pt, chkSnapGrid.Checked, chkSnapPoint.Checked, e);
                    AddArea(pt);
                }
                else if (drawingObject == eDrawingObject.segment)
                {
                    PointF pt = new PointF(e.X, e.Y);
                    ModifyPoint(ref pt, chkSnapGrid.Checked, chkSnapPoint.Checked, e);
                    AddSegment(pt);
                }
                else if (drawingObject == eDrawingObject.Dot)
                {
                    PointF pt = new PointF(e.X, e.Y);
                    ModifyPoint(ref pt, chkSnapGrid.Checked, chkSnapPoint.Checked, e);
                    AddDot(pt);
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button ==MouseButtons.Left && action == eAction.idle && drawingObject != eDrawingObject.deselect &&
                drawingObject != eDrawingObject.zoombox)
            {
                drawingObject = eDrawingObject.nothing;
                SelectPointsWithRectangle();           
            }
            else if (drawingObject == eDrawingObject.zoombox && e.Button != MouseButtons.Middle)
            {
                ZoomWithRectangle();
                zoomRectangle.Width = 0;
                zoomRectangle.Height = 0;
            }
            cursor = Cursors.Arrow;
            if (canvas.Cursor != cursor)
            {
                canvas.Cursor = cursor;
            }

            canvas.Invalidate();
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == Keys.Shift && e.Button == MouseButtons.Left)
            {
                drawingObject = eDrawingObject.nothing;
                if (hitEntity != null)
                {
                    if (hitEntity.Selected == false)
                    {
                        selectedCount = GD.Entities.FindAll(x => x.Selected == true).Count;
                        hitEntity.Selected = true;
                    }
                }
            }
            else if ((ModifierKeys & Keys.Control) == Keys.Control && e.Button == MouseButtons.Left)
            {           
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;

                drawingObject = eDrawingObject.deselect;
                if (hitEntity != null)
                {
                    if (hitEntity.Selected == true)
                    {
                        hitEntity.Selected = false;
                        selectedCount = GD.Entities.FindAll(x => x.Selected == true).Count;
                    }
                }
            }
            else if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver != eObjectOver.nothing)
            {
                drawingObject = eDrawingObject.nothing;
                if (hitEntity != null)
                {
                    foreach (var entity in GD.Entities.FindAll(x => x.Selected == true))
                    {
                        entity.Selected = false;
                        selectedCount = GD.Entities.FindAll(x => x.Selected == true).Count;
                    }
                    drawingObject = eDrawingObject.deselect;
                    hitEntity.Selected = true;
                    selectedCount = GD.Entities.FindAll(x => x.Selected == true).Count;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                drawingObject = eDrawingObject.nothing;
                action = eAction.idle;
                if (newArea.Points.Count > 2)
                    GD.Entities.Add(newArea);

                foreach (var entity in GD.Entities.FindAll(x => x.Selected == true))
                {
                    entity.Selected = false;
                    selectedCount =  GD.Entities.FindAll(x => x.Selected == true).Count;
                }

                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;

                newArea = new Area(new List<PointF>());
                newSegment = new Segment(new List<PointF>());
                newDot = new Dot(new Point());
                if (frmEntityProp!=null)
                   frmEntityProp.Close();
            }
        }

        private void Canvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //zoom extent
            if (e.Button == MouseButtons.Middle)
            {
                ZoomExtent();
            }
            if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver == eObjectOver.overArea)
            {
                startEntityEditForm();
            }
            else if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver == eObjectOver.overSegment)
            {
                startEntityEditForm();
            }
            else if (e.Button == MouseButtons.Left && action == eAction.idle && objectOver == eObjectOver.overDot)
            {
                startEntityEditForm();
            }

        }

        private void Canvas_Resize(object sender, EventArgs e)
        {
            ZoomExtent();
            canvas.Invalidate() ;
        }

        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldzoom = zoom;

            if (e.Delta > 0 /*&& zoom < maxzoom*/)
            {
                zoom += 0.1f /*/ Math.Max(GD.GridHeight,GD.GridWidth)*/;
            }
            if (e.Delta < 0 && zoom > minzoom)
            {
                zoom -= 0.1f /*/ Math.Max(GD.GridHeight, GD.GridWidth)*/;
            }

            Point mousePosNow = e.Location;

            float x = mousePosNow.X - canvas.Location.X;    // Where location of the mouse in the pictureframe
            float y = mousePosNow.Y - canvas.Location.Y;
           
            float oldimagex = (x / oldzoom);  // Where in the IMAGE is it now
            float oldimagey = (y / oldzoom);
            
            float newimagex = (x / zoom);     // Where in the IMAGE will it be when the new zoom i made
            float newimagey = (y / zoom);

            curX = newimagex - oldimagex + curX;  // Where to move image to keep focus on one point
            curY = newimagey - oldimagey + curY;

            if (zoom <= maxzoom && zoom >= minzoom)
            {
                canvas.Invalidate();
            }
        }

        private void BtnTsDrawLine_Click(object sender, EventArgs e)
        {
            action = eAction.draw;
            drawingObject = eDrawingObject.segment;
            StartSegmentForm();
        }

        private void BtnTsDrawDot_Click(object sender, EventArgs e)
        {
            action = eAction.draw;
            drawingObject = eDrawingObject.Dot;
            StartDotForm();
        }

        private void BtnTsDrawArea_Click(object sender, EventArgs e)
        {
            action = eAction.draw;
            drawingObject = eDrawingObject.Area;
            StartAreaForm();
        }

        private void BtnTsEntityProp_Click(object sender, EventArgs e)
        {
            startEntityEditForm();
        }

        private void BtnTsDelete_Click(object sender, EventArgs e)
        {
            GD.Entities.RemoveAll(x => x.Selected == true);
            canvas.Invalidate();
        }

        private void BtnTsMove_Click(object sender, EventArgs e)
        {
            MoveCommand();
        }

        private void BtnTsCopy_Click(object sender, EventArgs e)
        {
            CopyCommand();
        }

        private void BtnTsNew_Click(object sender, EventArgs e)
        {
            NewCommand();
        }

        private void BtnTsSave_Click(object sender, EventArgs e)
        {
            SaveCommand();
        }

        private void BtnTsOpen_Click(object sender, EventArgs e)
        {
            OpenCommand();
        }

        private void BtnTsGridSettings_Click(object sender, EventArgs e)
        {
            frmGridSettings gridSettings = new frmGridSettings(GD);
            gridSettings.ShowDialog();
            DialogResult = gridSettings.DialogResult;
            if (DialogResult == DialogResult.OK)
            {
                GD.GridSize = gridSettings.GridSize;
                GD.GridHeight = gridSettings.GridHeight;
                GD.GridWidth = gridSettings.GridWidth;
                ZoomExtent();
            }
        }

        private void BtnTsZoomExtent_Click(object sender, EventArgs e)
        {
            ZoomExtent();
            canvas.Invalidate();
        }

        private void BtnTsZoomOut_Click(object sender, EventArgs e)
        {
            var oldzoom = zoom;

            if (zoom > minzoom)
            {
                zoom -= 0.2f;
            }

            float x = canvas.Width / 2 - canvas.Location.X;    // Where location of the mouse in the pictureframe
            float y = canvas.Height /2  - canvas.Location.Y;

            float oldimagex = (x / oldzoom);  // Where in the IMAGE is it now
            float oldimagey = (y / oldzoom);

            float newimagex = (x / zoom);     // Where in the IMAGE will it be when the new zoom i made
            float newimagey = (y / zoom);

            curX = newimagex - oldimagex + curX;  // Where to move image to keep focus on one point
            curY = newimagey - oldimagey + curY;

            if (zoom <= maxzoom && zoom >= minzoom)
            {
                canvas.Invalidate();
            }
        }

        private void BtnTsZoomIn_Click(object sender, EventArgs e)
        {
            var oldzoom = zoom;
            if (zoom < maxzoom)
            {
                zoom += 0.2f;
            }
            float x = canvas.Width / 2  - canvas.Location.X;    // Where location of the mouse in the pictureframe
            float y = canvas.Height /2  - canvas.Location.Y;

            float oldimagex = (x / oldzoom);  // Where in the IMAGE is it now
            float oldimagey = (y / oldzoom);

            float newimagex = (x / zoom);     // Where in the IMAGE will it be when the new zoom i made
            float newimagey = (y / zoom);

            curX = newimagex - oldimagex + curX;  // Where to move image to keep focus on one point
            curY = newimagey - oldimagey + curY;

            if (zoom <= maxzoom && zoom >= minzoom)
            {
                canvas.Invalidate();
            }
        }

        private void BtnTsZoomRectangle_Click(object sender, EventArgs e)
        {
            drawingObject = eDrawingObject.zoombox;
        }

        private void BtnTsPan_Click(object sender, EventArgs e)
        {
            action = eAction.pan;
        }

        private void BtnTsAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void BtnTsMesh_Click(object sender, EventArgs e)
        {
            MeshCommand();
        }

        private void BtnTsExport_Click(object sender, EventArgs e)
        {
            ExportCommand();
        }

        private void BtnTsOlm_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(OLMStartCommand));
            t.Start();
        }

        private void BtnTsGnuPlot_Click(object sender, EventArgs e)
        {
            Task task = Task.Factory.StartNew(() => GnuPlotStartCommand());

            //Thread t = new Thread(new ThreadStart(GnuPlotStartCommand));
            //t.Start();
        }

        private void CmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            GD.Unit = (eUnit)cmbUnits.SelectedIndex;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            var g = e.Graphics;

            width = canvas.ClientSize.Width;
            height = canvas.ClientSize.Height;
            
            //selection rectangle
            Pen select_pen = new Pen(Color.Red);
            select_pen.DashStyle = DashStyle.Dash;
            Pen zoom_pen = new Pen(Color.Green);
            zoom_pen.DashStyle = DashStyle.Dash;
            if (drawingObject == eDrawingObject.selectionBox)
                g.DrawRectangle(select_pen, selectionRectangle);
            else if (drawingObject == eDrawingObject.zoombox)
                g.DrawRectangle(zoom_pen, zoomRectangle);


            //Flip & transform mouse
            g.ScaleTransform(zoom, -zoom);
            g.TranslateTransform(0, -(float)height);
            g.TranslateTransform(curX + width / 2 + GD.Nod, -curY + height / 2);
  
            //Draw Calls
            DrawGrid(g);
            DrawAll(g);
            DrawNextEntity(g, mousePos);
            DrawEndPoints(g);
            DrawSnapPoint(g);
            DrawSelected(g);
            DrawOrigin(g);
            DrawCurrentCoordinate(g, mousePos);
        }

        #endregion
    }

    // toolstrip borderları buglı olduğu için bunu kullandım.
    public class MySR : ToolStripSystemRenderer
    {
        public MySR() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }

}
