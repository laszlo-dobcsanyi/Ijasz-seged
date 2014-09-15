using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Íjász_segéd
{
    public struct Induló
    {
        public string név;
        public string nem;
        public string születés;
        public string engedély;
        public string egyesület;
        public int eredmények;

        public Induló(string _név, string _nem, string _születés, string _engedély, string _egyesület, int _eredmények)
        {
            név = _név;
            nem = _nem;
            születés = _születés;
            engedély = _engedély;
            egyesület = _egyesület;
            eredmények = _eredmények;
        }
    }

    public sealed class Panel_Induló : Control
    {
        private DataTable data;
        private DataGridView table;

        private TextBox keresés;

        public Panel_Induló()
        {
            InitializeContent();
        }

        private void InitializeContent()
        {
            table = new DataGridView();
            table.DataSource = CreateSource();
            table.Dock = DockStyle.Left;
            table.RowHeadersVisible = false;
            table.AllowUserToResizeRows = false;
            table.AllowUserToResizeColumns = false;
            table.AllowUserToAddRows = false;
            table.Width = 703;
            table.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            table.MultiSelect = false;
            table.ReadOnly = true;
            table.DataBindingComplete += table_DataBindingComplete;
            table.CellDoubleClick += módosítás_Click;

            ///

            Button hozzáadás = new Button();
            hozzáadás.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            hozzáadás.Text = "Hozzáadás";
            hozzáadás.Size = new System.Drawing.Size(96, 32);
            hozzáadás.Location = new System.Drawing.Point(ClientRectangle.Width - 96, ClientRectangle.Height - 32 - 16);
            hozzáadás.Click += hozzáadás_Click;

            Button beírás = new Button();
            beírás.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            beírás.Text = "Beírás";
            beírás.Size = new System.Drawing.Size(96, 32);
            beírás.Location = new System.Drawing.Point(ClientRectangle.Width - 96 - 8 - hozzáadás.Width, ClientRectangle.Height - 32 - 16);
            beírás.Click += beírás_Click;


            Label label_keresés = new Label();
            label_keresés.Text = "Név keresés:";
            label_keresés.Location = new System.Drawing.Point(table.Location.X + table.Size.Width + 16, 16 + 0 * 32);

            keresés = new TextBox();
            keresés.Location = new System.Drawing.Point(table.Location.X + table.Size.Width + label_keresés.Width + 16, 16 + 0 * 32);
            keresés.TextChanged += keresés_TextChanged;
            keresés.MaxLength = 30;
            keresés.Width = 150;

            Controls.Add(label_keresés);
            Controls.Add(keresés);

            Controls.Add(table);

            Controls.Add(hozzáadás);
            Controls.Add(beírás);
        }

        private DataTable CreateSource()
        {
            data = new DataTable();

            data.Columns.Add(new DataColumn("Név", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Nem", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Születési idő", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Engedélyszám", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Egyesület név", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Eredmények", System.Type.GetType("System.Int32")));

            return data;
        }

        #region Acessors
        private delegate void Induló_Hozzáadás_Callback(Induló _induló);
        public void Induló_Hozzáadás(Induló _induló)
        {
            if (InvokeRequired)
            {
                Induló_Hozzáadás_Callback callback = new Induló_Hozzáadás_Callback(Induló_Hozzáadás);
                Invoke(callback, new object[] { _induló });
            }
            else
            {
                DataRow row = data.NewRow();
                row[0] = _induló.név;
                row[1] = _induló.nem;
                row[2] = _induló.születés;
                row[3] = _induló.engedély;
                row[4] = _induló.egyesület;
                row[5] = _induló.eredmények;

                data.Rows.Add(row);
            }
        }

        private delegate void Induló_Módosítás_Callback(string _név, Induló _induló);
        public void Induló_Módosítás(string _név, Induló _induló)
        {
            if (InvokeRequired)
            {
                Induló_Módosítás_Callback callback = new Induló_Módosítás_Callback(Induló_Módosítás);
                Invoke(callback, new object[] { _név, _induló });
            }
            else
            {
                foreach (DataRow current in data.Rows)
                {
                    if (_név == current[0].ToString())
                    {
                        current[0] = _induló.név;
                        current[1] = _induló.nem;
                        current[2] = _induló.születés;
                        current[3] = _induló.engedély;
                        current[4] = _induló.egyesület;
                        current[5] = _induló.eredmények;
                        break;
                    }
                }
            }
        }

        private delegate void Induló_Átnevezés_Callback(string _eredeti, string _új);
        public void Induló_Átnevezés(string _eredeti, string _új)
        {
            if (InvokeRequired)
            {
                Induló_Átnevezés_Callback callback = new Induló_Átnevezés_Callback(Induló_Átnevezés);
                Invoke(callback, new object[] { _eredeti, _új });
            }
            else
            {
                foreach (DataRow current in data.Rows)
                {
                    if (_eredeti == (string)current[0])
                    {
                        current[0] = _új;
                        break;
                    }
                }

                foreach(Verseny verseny in Program.mainform.eredmény_panel.versenyek)
                {
                    if (verseny.eredmények != null)
                    {
                        foreach(Eredmény eredmény in verseny.eredmények)
                        {
                            if (eredmény.név == _eredeti)
                            {
                                eredmény.név = _új;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private delegate void Induló_Törlés_Callback(string _név);
        public void Induló_Törlés(string _név)
        {
            if (InvokeRequired)
            {
                Induló_Törlés_Callback callback = new Induló_Törlés_Callback(Induló_Törlés);
                Invoke(callback, new object[] { _név });
            }
            else
            {
                foreach (DataRow current in data.Rows)
                {
                    if (_név == current[0].ToString())
                    {
                        data.Rows.Remove(current);
                        break;
                    }
                }
            }
        }

        //

        public void Induló_Eredmény_Növelés(string _név)
        {
            foreach (DataRow current in data.Rows)
            {
                if (_név == (string)current[0])
                {
                    current[5] = ((int)current[5]) + 1;
                    break;
                }
            }
        }

        public void Induló_Eredmény_Csökkentés(string _név)
        {
            foreach (DataRow current in data.Rows)
            {
                if (_név == (string)current[0])
                {
                    current[5] = ((int)current[5]) - 1;
                    break;
                }
            }
        }
        #endregion

        #region EventHandlers
        private void beírás_Click(object sender, EventArgs e)
        {
            if (Program.mainform.eredmény_panel.versenyek.Count == 0) { MessageBox.Show("Nincsen még verseny adatok!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (Program.mainform.eredmény_panel.íjtípusok.Count == 0) { MessageBox.Show("Nincsen még íjtípus adatok!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (table.SelectedRows.Count != 1) { MessageBox.Show("Nem megfelelő az induló kiválasztás!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            Form_Induló_Beírás beírás_form = new Form_Induló_Beírás(table.SelectedRows[0].Cells[0].Value.ToString());
            beírás_form.ShowDialog();
        }

        private void table_DataBindingComplete(object _sender, EventArgs _event)
        {
            table.DataBindingComplete -= table_DataBindingComplete;

            table.Columns[0].Width = 200;

            foreach (DataGridViewColumn column in table.Columns) column.SortMode = DataGridViewColumnSortMode.NotSortable;

            //rendezés
            table.Sort(table.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void hozzáadás_Click(object _sender, EventArgs _event)
        {
            Form_Induló induló = new Form_Induló();
            induló.ShowDialog();
        }

        private void módosítás_Click(object _sender, EventArgs _event)
        {
            if ((table.SelectedRows.Count == 0) || (table.SelectedRows[0].Index == data.Rows.Count)) { MessageBox.Show("Nincs kiválasztva induló!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; };

            foreach (DataGridViewRow current in table.Rows)
            {
                if (table.SelectedRows[0] == current)
                {
                    Form_Induló induló = new Form_Induló(new Induló(current.Cells[0].Value.ToString(),
                        current.Cells[1].Value.ToString(),
                        current.Cells[2].Value.ToString(),
                        current.Cells[3].Value.ToString(), 
                        current.Cells[4].Value.ToString(), 
                        Convert.ToInt32(current.Cells[5].Value) ) ) ;
                    induló.ShowDialog();
                    return;
                }
            }
        }

        private void keresés_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in table.Rows)
            {
                int talált = 0;
                for (int i = 0; i < keresés.Text.Length; i++)
                {
                    if (row.Cells[0].Value.ToString().Length<keresés.Text.Length)
                    {
                        break;
                    }
                    if (row.Cells[0].Value.ToString()[i] == keresés.Text[i] || row.Cells[0].Value.ToString()[i]==Char.ToUpper(keresés.Text[i]) )
                    {
                        talált++;
                    }
                }
                if (talált==keresés.Text.Length )
                {
                    table.Rows[row.Index].Selected = true;
                    table.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }
            }
        }        
        #endregion

        public sealed class Form_Induló : Form
        {
            private Induló? eredeti = null;

            private TextBox box_név;
            private TextBox box_nem;
            private DateTimePicker date_születés;
            private TextBox box_engedély;
            private TextBox box_egyesület;
            private Label eredmények_száma;

            public Form_Induló()
            {
                InitializeForm();
                InitializeContent();
                InitializeData();
            }

            public Form_Induló(Induló _induló)
            {
                eredeti = _induló;

                InitializeForm();
                InitializeContent();
                InitializeData(_induló);
            }

            private void InitializeForm()
            {
                Text = "Induló";
                ClientSize = new System.Drawing.Size(400 - 32, 232);
                MinimumSize = ClientSize;
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            }

            private void InitializeContent()
            {
                Label név = new Label();
                név.Text = "Név:";
                név.Location = new System.Drawing.Point(16, 16 + 0 * 32);

                Label nem = new Label();
                nem.Text = "Nem:";
                nem.Location = new System.Drawing.Point(név.Location.X, 16 + 1 * 32);

                Label születés = new Label();
                születés.Text = "Születési idő:";
                születés.Location = new System.Drawing.Point(név.Location.X, 16 + 2 * 32);

                Label engedély = new Label();
                engedély.Text = "Engedélyszám:";
                engedély.Location = new System.Drawing.Point(név.Location.X, 16 + 3 * 32);

                Label egyesület = new Label();
                egyesület.Text = "Egyesület név:";
                egyesület.Location = new System.Drawing.Point(név.Location.X, 16 + 4 * 32);

                Label eredmények = new Label();
                eredmények.Text = "Eredmények:";
                eredmények.Location = new System.Drawing.Point(név.Location.X, 16 + 5 * 32);

                ///

                box_név = new TextBox();
                box_név.Location = new System.Drawing.Point(név.Location.X + név.Size.Width + 16, név.Location.Y);
                box_név.Size = new System.Drawing.Size(128 + 64, 24);
                box_név.MaxLength = 30;

                box_nem = new TextBox();
                box_nem.Location = new System.Drawing.Point(nem.Location.X + nem.Size.Width + 16, nem.Location.Y);
                box_nem.Size = new System.Drawing.Size(64, 24);
                box_nem.MaxLength = 10;

                date_születés = new DateTimePicker();
                date_születés.Location = new System.Drawing.Point(születés.Location.X + születés.Size.Width + 16, születés.Location.Y);
                date_születés.Size = box_név.Size;
                date_születés.Value = DateTime.Now;

                box_engedély = new TextBox();
                box_engedély.Location = new System.Drawing.Point(engedély.Location.X + engedély.Size.Width + 16, engedély.Location.Y);
                box_engedély.Size = box_név.Size;
                box_engedély.MaxLength = 30;

                box_egyesület = new TextBox();
                box_egyesület.Location = new System.Drawing.Point(egyesület.Location.X + egyesület.Size.Width + 16, egyesület.Location.Y);
                box_egyesület.Size = box_név.Size;
                box_egyesület.MaxLength = 30;

                eredmények_száma = new Label();
                eredmények_száma.Location = new System.Drawing.Point(eredmények.Location.X + eredmények.Size.Width + 16, eredmények.Location.Y);

                Button rendben = new Button();
                rendben.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                rendben.Text = "Rendben";
                rendben.Size = new System.Drawing.Size(96, 32);
                rendben.Location = new System.Drawing.Point(ClientRectangle.Width - 96 - 16, ClientRectangle.Height - 32 - 16);
                rendben.Click += rendben_Click;

                ///

                Controls.Add(név);
                Controls.Add(nem);
                Controls.Add(születés);
                Controls.Add(engedély);
                Controls.Add(egyesület);
                Controls.Add(eredmények);

                Controls.Add(box_név);
                Controls.Add(box_nem);
                Controls.Add(date_születés);
                Controls.Add(box_engedély);
                Controls.Add(box_egyesület);
                Controls.Add(eredmények_száma);
                Controls.Add(rendben);
            }

            private void InitializeData()
            {
                box_név.Text = "";
                box_név.Enabled = true;
                box_nem.Text = "";
                date_születés.Value = DateTime.Now;
                box_engedély.Text = "";
                box_egyesület.Text = "";
                eredmények_száma.Text = "0";
            }

            private void InitializeData(Induló _induló)
            {
                box_név.Text = _induló.név;
                box_név.Enabled = false;
                box_nem.Text = _induló.nem == "N" ? "Nő" : "Férfi";
                box_nem.Enabled = (_induló.eredmények > 0 ? false : true);
                date_születés.Value = DateTime.Parse(_induló.születés);
                date_születés.Enabled = (_induló.eredmények > 0 ? false : true);
                box_engedély.Text = _induló.engedély;
                box_egyesület.Text = _induló.egyesület;
                eredmények_száma.Text = _induló.eredmények.ToString();
            }

            private void rendben_Click(object _sender, EventArgs _event)
            {
                if (date_születés.Value.Year == DateTime.Now.Year && date_születés.Value.Month == DateTime.Now.Month && date_születés.Value.Day == DateTime.Now.Day) { MessageBox.Show("A születési dátum nem lehet a mai nap!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!(0 < box_név.Text.Length && box_név.Text.Length <= 30)) { MessageBox.Show("Nem megfelelő a név hossza (1 - 30 hosszú kell legyen)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!Program.IsCorrectSQLText(box_név.Text)) { MessageBox.Show("Nem megengedett karakterek a mezőben!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!(0 < box_nem.Text.Length && box_nem.Text.Length <= 10)) { MessageBox.Show("Nem megfelelő a nem hossza (1 - 10 hosszú kell legyen)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                bool nő = false;
                if (box_nem.Text.ToLower() == "n" || box_nem.Text.ToLower() == "nő") nő = true;
                else if (!(box_nem.Text.ToLower() == "f" || box_nem.Text.ToLower() == "férfi")) { MessageBox.Show("Nem megfelelő nem!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!(box_engedély.Text.Length <= 30)) { MessageBox.Show("Nem megfelelő az engedély hossza (0 - 30 hosszú kell legyen)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!Program.IsCorrectSQLText(box_engedély.Text)) { MessageBox.Show("Nem megengedett karakterek a mezőben!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!(box_egyesület.Text.Length <= 30)) { MessageBox.Show("Nem megfelelő az egyesület hossza (0 - 30 hosszú kell legyen)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!Program.IsCorrectSQLText(box_egyesület.Text)) { MessageBox.Show("Nem megengedett karakterek a mezőben!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                if (eredeti != null)
                    Program.network.Send(ClientCommand.INDULÓ_MÓDOSÍTÁS, eredeti.Value.név + ";" + box_név.Text + ";" + (nő ? "N" : "F") + ";" + date_születés.Value.ToShortDateString() + ";" + box_engedély.Text + ";" + box_egyesület.Text);
                else
                    Program.network.Send(ClientCommand.INDULÓ_HOZZÁADÁS, box_név.Text + ";" + (nő ? "N" : "F") + ";" + date_születés.Value.ToShortDateString() + ";" + box_engedély.Text + ";" + box_egyesület.Text);

                Close();
            }
        }
        
        public sealed class Form_Induló_Beírás : Form
        {
            public ComboBox combo_verseny;
            private Label label_név;
            private ComboBox combo_íjtípus;
            private ComboBox combo_csapat;
            private CheckBox check_megjelent;

            public Form_Induló_Beírás(string _név)
            {
                label_név = new Label();
                label_név.Text = _név;

                InitializeForm();
                InitializeContent();
                InitializeData();
            }

            private void InitializeForm()
            {
                Text = "Beírás";
                ClientSize = new System.Drawing.Size(464 - 64, 200);
                MinimumSize = ClientSize;
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            }

            private void InitializeContent()
            {
                Label név = new Label();
                név.Text = "Név:";
                név.Location = new System.Drawing.Point(32, 16 + 0 * 32);

                Label verseny = new Label();
                verseny.Text = "Verseny:";
                verseny.Location = new System.Drawing.Point(név.Location.X, 16 + 1 * 32);
               
                Label íjtípus = new Label();
                íjtípus.Text = "Íjtípus:";
                íjtípus.Location = new System.Drawing.Point(név.Location.X, 16 + 2 * 32);

                Label csapat = new Label();
                csapat.Text = "Csapatszám:";
                csapat.Location = new System.Drawing.Point(név.Location.X, 16 + 3 * 32);

                Label megjelent = new Label();
                megjelent.Text = "Megjelent:";
                megjelent.Location = new System.Drawing.Point(név.Location.X, 16 + 4 * 32);


                ///

                label_név.Location = new System.Drawing.Point(név.Location.X + név.Size.Width + 16, név.Location.Y);
                label_név.Size = new System.Drawing.Size(128 + 64, 24);

                combo_verseny = new ComboBox();
                combo_verseny.Location = new System.Drawing.Point(verseny.Location.X + verseny.Size.Width + 16, verseny.Location.Y);
                combo_verseny.Size = label_név.Size;
                combo_verseny.DropDownStyle = ComboBoxStyle.DropDownList;

                combo_íjtípus = new ComboBox();
                combo_íjtípus.Location = new System.Drawing.Point(íjtípus.Location.X + íjtípus.Size.Width + 16, íjtípus.Location.Y);
                combo_íjtípus.Size = label_név.Size;
                combo_íjtípus.DropDownStyle = ComboBoxStyle.DropDownList;


                combo_csapat = new ComboBox();
                combo_csapat.Location = new System.Drawing.Point(csapat.Location.X + csapat.Size.Width + 16, csapat.Location.Y);
                combo_csapat.Size = label_név.Size;
                combo_csapat.DropDownStyle = ComboBoxStyle.DropDownList;

                for (int i = 0; i < 35; i++) combo_csapat.Items.Add(i + 1);
                combo_csapat.SelectedItem = combo_csapat.Items[0];

                check_megjelent = new CheckBox();
                check_megjelent.Location = new System.Drawing.Point(combo_csapat.Location.X, megjelent.Location.Y);

                Button rendben = new Button();
                rendben.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                rendben.Text = "Rendben";
                rendben.Size = new System.Drawing.Size(96, 32);
                rendben.Location = new System.Drawing.Point(ClientRectangle.Width - 96 - 16, ClientRectangle.Height - 32 - 16);
                rendben.Click += rendben_Click;

                ///

                foreach (Verseny current in Program.mainform.eredmény_panel.versenyek)
                    if (!current.lezárt) combo_verseny.Items.Add(current.azonosító);

               
                foreach (string current in Program.mainform.eredmény_panel.íjtípusok)
                    combo_íjtípus.Items.Add(current);

                ///

                Controls.Add(combo_verseny);

                Controls.Add(név);
                Controls.Add(íjtípus);
                Controls.Add(csapat);
                Controls.Add(megjelent);

                Controls.Add(verseny);
                Controls.Add(label_név);
                Controls.Add(combo_íjtípus);
                Controls.Add(combo_csapat);
                Controls.Add(rendben);
                Controls.Add(check_megjelent);
            }

            private void InitializeData()
            {
                combo_verseny.SelectedIndex = 0;
                combo_íjtípus.SelectedIndex = 0;
                combo_csapat.SelectedIndex = 0;
                check_megjelent.Checked = false;
            }

            #region EventHandlers
            private void rendben_Click(object _sender, EventArgs _event)
            {
                if (combo_verseny.SelectedItem == null) { MessageBox.Show("Nincs kiválasztva verseny!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (combo_íjtípus.SelectedItem == null) { MessageBox.Show("Nincs kiválasztva íjtípus!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!Program.IsCorrectSQLText(combo_csapat.Text)) { MessageBox.Show("Nem megengedett karakterek a mezőben!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                Verseny verseny = null;
                foreach(Verseny current in Program.mainform.eredmény_panel.versenyek)
                {
                    if (current.azonosító == combo_verseny.Text)
                    {
                        verseny = current;
                        break;
                    }
                }
                if(verseny != null)
                    if (verseny.lezárt) { MessageBox.Show("A verseny már le van zárva!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                else
                    { MessageBox.Show("Nem található a verseny a segéd adatai között!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                bool nyomtatás = false;
                if (MessageBox.Show("Nyomtassak beírólapot ennek a versenyzőnek: " + label_név.Text + "?", "Nyomtatás", MessageBoxButtons.YesNo) == DialogResult.Yes) nyomtatás = true;

                Program.network.Send(ClientCommand.INDULÓ_BEÍRÁS, label_név.Text + ";" + combo_verseny.Text + ";" + combo_íjtípus.Text + ";" + combo_csapat.Text + ";" + check_megjelent.Checked + ";" + nyomtatás);

                Close();
            }
            #endregion
        }
    }
}
