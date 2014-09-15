using System;
using System.Drawing;
using System.Windows.Forms;

namespace Íjász_segéd
{
    public partial class MainForm : Form
    {
        public Panel_Induló     induló_panel;
        public Panel_Eredmény   eredmény_panel;

        private TabControl      menu;
        private StatusStrip     status;

        public MainForm()
        {
            InitializeForm();
            InitializeContent();
        }

        private void InitializeForm()
        {
            Text = "Íjász segéd";
            ClientSize = new System.Drawing.Size(1024, 768);
            MinimumSize = ClientSize;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeContent()
        {
            #region Status
            ToolStripStatusLabel Tulajdonos = new ToolStripStatusLabel("Turul Koppány Íjászai HE, Hunvér Kft.");
            ToolStripStatusLabel Készítők = new ToolStripStatusLabel("©Belinyák Nándor és Társai.");
            Készítők.BorderSides = ToolStripStatusLabelBorderSides.Left;

            status = new StatusStrip();
            status.Items.Add(Tulajdonos);
            status.Items.Add(Készítők);
            #endregion

            #region Menu
            TabPage Induló = new TabPage("Induló");
            induló_panel = new Panel_Induló();
            induló_panel.Dock = DockStyle.Fill;
            Induló.Controls.Add(induló_panel);

            TabPage Eredmény = new TabPage("Eredmény");
            eredmény_panel = new Panel_Eredmény();
            eredmény_panel.Dock = DockStyle.Fill;
            Eredmény.Controls.Add(eredmény_panel);

            //

            menu = new TabControl();
            menu.TabPages.Add(Induló);
            menu.TabPages.Add(Eredmény);

            menu.DrawItem += menu_DrawItem;
            menu.DrawMode = TabDrawMode.OwnerDrawFixed;
            menu.Padding = new Point(18, 5);

            menu.Dock = DockStyle.Fill;
            #endregion

            Controls.Add(status);
            Controls.Add(menu);
            menu.BringToFront();
        }

        #region EventHandlers
        private void menu_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage currentab = menu.TabPages[e.Index];
            SolidBrush textbrush = new SolidBrush(Color.Black);
            Rectangle itemrect = menu.GetTabRect(e.Index);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                Font f = new Font(menu.Font.Name, menu.Font.Size + 1, FontStyle.Bold);
                e.Graphics.DrawString(currentab.Text, f, textbrush, itemrect, sf);
            }
            else e.Graphics.DrawString(currentab.Text, e.Font, textbrush, itemrect, sf);
            textbrush.Dispose();
        }
        #endregion
    }
}
