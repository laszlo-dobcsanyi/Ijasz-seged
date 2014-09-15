using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;

namespace Íjász_segéd
{
    public class Eredmény
    {
        public string név;
        public Int64? sorszám;
        public string íjtípus;
        public int csapat;
        public int találat_10;
        public int találat_08;
        public int találat_05;
        public int mellé;
        public int? összpont;
        public int? százalék;
        public bool megjelent;

        public Eredmény(string _név, Int64? _sorszám, string _íjtípus, int _csapat, int _találat_10, int _találat_08, int _találat_05, int _mellé, int? _összpont, int? _százalék, bool _megjelent)
        {
            név = _név;
            sorszám = _sorszám;
            íjtípus = _íjtípus;
            csapat = _csapat;
            találat_10 = _találat_10;
            találat_08 = _találat_08;
            találat_05 = _találat_05;
            mellé = _mellé;
            összpont = _összpont;
            százalék = _százalék;
            megjelent = _megjelent;
        }
    }

    public class Verseny
    {
        public string azonosító;
        public int összes;
        public bool lezárt;

        public List<Eredmény> eredmények = null;

        public Verseny(string _azonosító, int _összes, bool _lezárt)
        {
            azonosító = _azonosító;
            összes = _összes;
            lezárt = _lezárt;
        }
    }

    public sealed class Panel_Eredmény : Control
    {
        private ComboBox combo_versenyek;
        private ComboBox combo_íjtípusok;

        public DataTable data;
        public DataGridView table;

        private Verseny verseny = null;

        public List<string> íjtípusok = new List<string>();
        public List<Verseny> versenyek = new List<Verseny>();

        public Panel_Eredmény()
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
            table.Width = 683;
            table.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            table.MultiSelect = false;
            table.ReadOnly = true;
            table.DataBindingComplete += table_DataBindingComplete;
            table.CellDoubleClick += módosítás_Click;

            //

            Label verseny_neve = new Label();
            verseny_neve.Text = "Verseny:";
            verseny_neve.Location = new Point(table.Location.X + table.Size.Width + 16, 16);

            combo_versenyek = new ComboBox();
            combo_versenyek.Size = new System.Drawing.Size(128, 24);
            combo_versenyek.Location = new System.Drawing.Point(verseny_neve.Location.X + verseny_neve.Width + 16, 16);
            combo_versenyek.DropDownStyle = ComboBoxStyle.DropDownList;
            combo_versenyek.SelectedIndexChanged += combo_versenyek_SelectedIndexChanged;

            //

            combo_íjtípusok = new ComboBox();
            combo_íjtípusok.Location = new Point(16 + 128 + 4, 16 + 1 * 32);
            combo_íjtípusok.DropDownStyle = ComboBoxStyle.DropDownList;

            //

            Controls.Add(table);

            Controls.Add(verseny_neve);
            Controls.Add(combo_versenyek);
        }

        private DataTable CreateSource()
        {
            data = new DataTable();

            data.Columns.Add(new DataColumn("Név", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Sorszám", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Íjtípus", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Csapatszám", System.Type.GetType("System.String")));
            data.Columns.Add(new DataColumn("Tíz", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Nyolc", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Öt", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Mellé", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Összes", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("%", System.Type.GetType("System.Int32")));
            data.Columns.Add(new DataColumn("Megjelent", System.Type.GetType("System.Boolean")));

            return data;
        }


        #region Accessors
        private delegate void Eredmény_Hozzáadás_Callback(string _azonosító, Eredmény _eredmény);
        public void Eredmény_Hozzáadás(string _azonosító, Eredmény _eredmény)
        {
            if (InvokeRequired)
            {
                Eredmény_Hozzáadás_Callback callback = new Eredmény_Hozzáadás_Callback(Eredmény_Hozzáadás);
                Invoke(callback, new object[] { _azonosító, _eredmény });
            }
            else
            {
                if (verseny != null && _azonosító == verseny.azonosító)
                {
                    DataRow row = data.NewRow();
                    row[0] = _eredmény.név;
                    row[1] = _eredmény.sorszám;
                    row[2] = _eredmény.íjtípus;
                    row[3] = _eredmény.csapat;
                    row[4] = _eredmény.találat_10;
                    row[5] = _eredmény.találat_08;
                    row[6] = _eredmény.találat_05;
                    row[7] = _eredmény.mellé;
                    row[8] = _eredmény.összpont;
                    row[9] = _eredmény.százalék;
                    row[10] = _eredmény.megjelent;
                    data.Rows.Add(row);

                    verseny.eredmények.Add(_eredmény);
                }
                else
                {
                    foreach (Verseny current in versenyek)
                    {
                        if (current.azonosító == _azonosító)
                        {
                            if (current.eredmények != null) current.eredmények.Add(_eredmény);
                            break;
                        }
                    }
                }
            }

        }

        private delegate void Eredmény_Beírás_Hozzáadás_Callback(string _azonosító, string _név, Int64 _sorszám, string _íjtípus, int _csapat, bool _megjelent);
        public void Eredmény_Beírás_Hozzáadás(string _azonosító, string _név, Int64 _sorszám, string _íjtípus, int _csapat, bool _megjelent)
        {
            if (InvokeRequired)
            {
                Eredmény_Beírás_Hozzáadás_Callback callback = new Eredmény_Beírás_Hozzáadás_Callback(Eredmény_Beírás_Hozzáadás);
                Invoke(callback, new object[] { _azonosító, _név, _sorszám, _íjtípus, _csapat, _megjelent });
            }
            else
            {
                if (verseny != null && _azonosító == verseny.azonosító)
                {
                    DataRow row = data.NewRow();
                    row[0] = _név;
                    row[1] = _sorszám;
                    row[2] = _íjtípus;
                    row[3] = _csapat;
                    row[4] = 0;
                    row[5] = 0;
                    row[6] = 0;
                    row[7] = 0;
                    row[8] = 0;
                    row[9] = 0;
                    row[10] = _megjelent;
                    data.Rows.Add(row);

                    verseny.eredmények.Add(new Eredmény(_név, _sorszám, _íjtípus, _csapat, 0, 0, 0, 0, 0, 0, _megjelent));
                }
                else
                {
                    foreach (Verseny current in versenyek)
                    {
                        if (current.azonosító == _azonosító)
                        {
                            if (current.eredmények != null) current.eredmények.Add(new Eredmény(_név, _sorszám, _íjtípus, _csapat, 0, 0, 0, 0, 0, 0, _megjelent));
                            break;
                        }
                    }
                }

                Program.mainform.induló_panel.Induló_Eredmény_Növelés(_név);
            }
        }

        private delegate void Eredmény_Beírás_Módosítás_Callback(string _azonosító, string _név, string _íjtípus, int _csapat, bool _megjelent);
        public void Eredmény_Beírás_Módosítás(string _azonosító, string _név, string _íjtípus, int _csapat, bool _megjelent)
        {
            if (InvokeRequired)
            {
                Eredmény_Beírás_Módosítás_Callback callback = new Eredmény_Beírás_Módosítás_Callback(Eredmény_Beírás_Módosítás);
                Invoke(callback, new object[] { _azonosító, _név, _íjtípus, _csapat, _megjelent });
            }
            else
            {
                if (verseny != null && _azonosító == verseny.azonosító)
                {
                    foreach (DataRow current in data.Rows)
                    {
                        if (_név == (string)current[0])
                        {
                            //current[0] = _eredmény.név;
                            //current[1] = _sorszám;
                            current[2] = _íjtípus;
                            current[3] = _csapat;
                            //current[4] = _találat_10;
                            //current[5] = _találat_08;
                            //current[6] = _találat_05;
                            //current[7] = _mellé;
                            //current[8] = _találat_10 * 10 + _találat_08 * 8 + _találat_05 * 5;
                            //current[9] = _százalék;
                            current[10] = _megjelent;
                            break;
                        }
                    }

                    foreach (Eredmény eredmény in verseny.eredmények)
                    {
                        if (eredmény.név == _név)
                        {
                            eredmény.íjtípus = _íjtípus;
                            eredmény.csapat = _csapat;
                            eredmény.megjelent = _megjelent;
                            break;
                        }
                    }

                }
                else
                {
                    foreach (Verseny current in versenyek)
                    {
                        if (current.azonosító == _azonosító)
                        {
                            if (current.eredmények != null)
                                foreach (Eredmény eredmény in verseny.eredmények)
                                {
                                    if (eredmény.név == _név)
                                    {
                                        eredmény.íjtípus = _íjtípus;
                                        eredmény.csapat = _csapat;
                                        eredmény.megjelent = _megjelent;
                                        break;
                                    }
                                }
                            break;
                        }
                    }
                }
            }
        }

        private delegate void Eredmény_Módosítás_Callback(string _azonosító, string _név, int _találat_10, int _találat_08, int _találat_05, int _mellé, int _százaléky);
        public void Eredmény_Módosítás(string _azonosító, string _név, int _találat_10, int _találat_08, int _találat_05, int _mellé, int _százalék)
        {
            if (InvokeRequired)
            {
                Eredmény_Módosítás_Callback callback = new Eredmény_Módosítás_Callback(Eredmény_Módosítás);
                Invoke(callback, new object[] {_azonosító, _név, _találat_10, _találat_08, _találat_05, _mellé, _százalék });
            }
            else
            {
                if (_azonosító == combo_versenyek.Text)
                {
                    foreach (DataRow current in data.Rows)
                    {
                        if (_név == (string)current[0])
                        {
                            //current[0] = _eredmény.név;
                            //current[1] = _eredmény.sorszám;
                            //current[2] = _eredmény.íjtípus;
                            //current[3] = _eredmény.csapat;
                            current[4] = _találat_10;
                            current[5] = _találat_08;
                            current[6] = _találat_05;
                            current[7] = _mellé;
                            current[8] = _találat_10 * 10 + _találat_08 * 8 + _találat_05 * 5;
                            current[9] = _százalék;
                            //current[10] = _eredmény.megjelent;
                            break;
                        }
                    }
                }
            }
        }

        private delegate void Eredmény_Törlés_Callback(string _azonosító, string _név);
        public void Eredmény_Törlés(string _azonosító, string _név)
        {
            if (InvokeRequired)
            {
                Eredmény_Törlés_Callback callback = new Eredmény_Törlés_Callback(Eredmény_Törlés);
                Invoke(callback, new object[] { _azonosító, _név });
            }
            else
            {
                if (_azonosító == combo_versenyek.Text)
                {
                    foreach (DataRow current in data.Rows)
                    {
                        if (_név == (string)current[0])
                        {
                            data.Rows.Remove(current);
                            break;
                        }
                    }
                }

                Program.mainform.induló_panel.Induló_Eredmény_Csökkentés(_név);
            }
        }

        //

        private delegate void Verseny_Hozzáadás_Callback(string _azonosító, int _össze, bool _lezárt);
        public void Verseny_Hozzáadás(string _azonosító, int _összes, bool _lezárt)
        {
            if (InvokeRequired)
            {
                Verseny_Hozzáadás_Callback callback = new Verseny_Hozzáadás_Callback(Verseny_Hozzáadás);
                Invoke(callback, new object[] { _azonosító, _összes, _lezárt });
            }
            else
            {
                combo_versenyek.Items.Add(_azonosító);
                versenyek.Add(new Verseny(_azonosító, _összes, _lezárt));
            }
        }

        private delegate void Verseny_Módosítás_Callback(string _azonosító, string _új, int _összes);
        public void Verseny_Módosítás(string _azonosító, string _új, int _összes)
        {
            if (InvokeRequired)
            {
                Verseny_Módosítás_Callback callback = new Verseny_Módosítás_Callback(Verseny_Módosítás);
                Invoke(callback, new object[] { _azonosító, _új, _összes });
            }
            else
            {
                for (int current = 0; current < combo_versenyek.Items.Count; ++current)
                {
                    if (_azonosító == combo_íjtípusok.Items[current].ToString())
                    {
                        combo_íjtípusok.Items[current] = _új;
                        break;
                    }
                }

                for (int current = 0; current < versenyek.Count; ++current)
                {
                    if (versenyek[current].azonosító == _azonosító)
                    {
                        versenyek[current].azonosító = _új;
                        versenyek[current].összes = _összes;
                        break;
                    }
                }
            }
        }

        private delegate void Verseny_Törlés_Callback(string _azonosító);
        public void Verseny_Törlés(string _azonosító)
        {
            if (InvokeRequired)
            {
                Verseny_Törlés_Callback callback = new Verseny_Törlés_Callback(Verseny_Törlés);
                Invoke(callback, new object[] { _azonosító });
            }
            else
            {
                combo_íjtípusok.Items.Remove(_azonosító);
                foreach(Verseny verseny in versenyek)
                {
                    if (verseny.azonosító == _azonosító)
                    {
                        versenyek.Remove(verseny);
                        break;
                    }
                }
            }
        }

        private delegate void Verseny_Lezárás_Callback(string _azonosító);
        public void Verseny_Lezárás(string _azonosító)
        {
            foreach(Verseny verseny in versenyek)
            {
                if (verseny.azonosító == _azonosító)
                {
                    verseny.lezárt = true;
                    break;
                }
            }

            combo_versenyek.Items.Remove(_azonosító);
        }

        private delegate void Verseny_Megnyitás_Callback(string _azonosító);
        public void Verseny_Megnyitás(string _azonosító)
        {
            foreach (Verseny verseny in versenyek)
            {
                if (verseny.azonosító == _azonosító)
                {
                    verseny.lezárt = false;
                    break;
                }
            }

            combo_versenyek.Items.Remove(_azonosító);
        }

        //

        private delegate void Íjtípus_Hozzáadás_Callback(string _íjtípus);
        public void Íjtípus_Hozzáadás(string _íjtípus)
        {
            if (InvokeRequired)
            {
                Íjtípus_Hozzáadás_Callback callback = new Íjtípus_Hozzáadás_Callback(Íjtípus_Hozzáadás);
                Invoke(callback, new object[] { _íjtípus });
            }
            else
            {
                combo_íjtípusok.Items.Add(_íjtípus);
                íjtípusok.Add(_íjtípus);
            }
        }

        private delegate void Íjtípus_Módosítás_Callback(string _azonosító, string _íjtípus);
        public void Íjtípus_Módosítás(string _azonosító, string _íjtípus)
        {
            if (_azonosító == _íjtípus) return;

            if (InvokeRequired)
            {
                Íjtípus_Módosítás_Callback callback = new Íjtípus_Módosítás_Callback(Íjtípus_Módosítás);
                Invoke(callback, new object[] { _azonosító, _íjtípus });
            }
            else
            {
                for (int current = 0; current < combo_íjtípusok.Items.Count; ++current)
                {
                    if (_azonosító == combo_íjtípusok.Items[current].ToString())
                    {
                        combo_íjtípusok.Items[current] = _íjtípus;
                        break;
                    }
                }

                for (int current = 0; current < íjtípusok.Count; ++current)
                {
                    if (_azonosító == íjtípusok[current])
                    {
                        íjtípusok[current] = _íjtípus;
                        break;
                    }
                }
            }
        }

        private delegate void Íjtípus_Törlés_Callback(string _íjtípus);
        public void Íjtípus_Törlés(string _azonosító)
        {
            if (InvokeRequired)
            {
                Íjtípus_Törlés_Callback callback = new Íjtípus_Törlés_Callback(Íjtípus_Törlés);
                Invoke(callback, new object[] { _azonosító });
            }
            else
            {
                combo_íjtípusok.Items.Remove(_azonosító);
                íjtípusok.Remove(_azonosító);
            }
        }
        #endregion

        #region EventHandlers
        private void table_DataBindingComplete(object _sender, EventArgs _event)
        {
            table.DataBindingComplete -= table_DataBindingComplete;

            table.Columns[0].Width = 120;
            table.Columns[1].Width = 50;
            table.Columns[2].Width = 80;
            table.Columns[3].Width = 80;
            table.Columns[4].Width = 50;
            table.Columns[5].Width = 50;
            table.Columns[6].Width = 50;
            table.Columns[7].Width = 50;
            table.Columns[8].Width = 50;
            table.Columns[9].Width = 40;
            table.Columns[10].Width = 60;

            foreach (DataGridViewColumn column in table.Columns) column.SortMode = DataGridViewColumnSortMode.NotSortable;

            //rendezés
            table.Sort(table.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void combo_versenyek_SelectedIndexChanged(object _sender, EventArgs _event)
        {
            foreach(Verseny current in versenyek)
            {
                if (current.azonosító == combo_versenyek.Text)
                {
                    verseny = current;
                    if (current.eredmények == null)
                    {
                        current.eredmények = new List<Eredmény>();
                        Program.network.Send(ClientCommand.EREDMÉNYEK, current.azonosító);
                    }
                    break;
                }
            }

            if (verseny == null) { MessageBox.Show("HIBA, nem találtam a kiválasztott versenyt? (#1)"); return;}

            data.Rows.Clear();
            foreach(Eredmény current in verseny.eredmények)
            {
                DataRow row = data.NewRow();
                row[0] = current.név;
                row[1] = current.sorszám;
                row[2] = current.íjtípus;
                row[3] = current.csapat;
                row[4] = current.találat_10;
                row[5] = current.találat_08;
                row[6] = current.találat_05;
                row[7] = current.mellé;
                row[8] = current.összpont;
                row[9] = current.százalék;
                row[10] = current.megjelent;

                data.Rows.Add(row);
            }
        }

        private void módosítás_Click(object _sender, EventArgs _event)
        {
            if (verseny == null) return;
            if (combo_versenyek.SelectedItem == null) return; 
            if ((table.SelectedRows.Count == 0) || (table.SelectedRows[0].Index == data.Rows.Count)) return;
            if (verseny.lezárt) { MessageBox.Show("A verseny már le van zárva!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; };

            Form_Eredmény eredmény_form = new Form_Eredmény(combo_versenyek.Text, verseny.összes, new Eredmény(
                table.SelectedRows[0].Cells[0].Value.ToString(),
                Convert.ToInt32(table.SelectedRows[0].Cells[1].Value),
                table.SelectedRows[0].Cells[2].Value.ToString(),
                Convert.ToInt32(table.SelectedRows[0].Cells[3].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[4].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[5].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[6].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[7].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[8].Value),
                Convert.ToInt32(table.SelectedRows[0].Cells[9].Value),
                Convert.ToBoolean(table.SelectedRows[0].Cells[10].Value)
                ));
            eredmény_form.ShowDialog();
        }
        #endregion

        public sealed class Form_Eredmény : Form
        {
            public string verseny_azonosító = null;
            private Eredmény eredeti;
            private int összespont;

            private TextBox box_név;
            private ComboBox combo_csapat;
            private TextBox box_találat_10;
            private TextBox box_találat_8;
            private TextBox box_találat_5;
            private TextBox box_mellé;
            private Label label_összes;
            private Label label_százalék;
            private CheckBox box_megjelent;

            public Form_Eredmény(string _verseny, int _összespont, Eredmény _eredmény)
            {
                eredeti = _eredmény;
                összespont = _összespont;
                verseny_azonosító = _verseny;

                InitializeForm();
                InitializeContent();
                InitializeData(_eredmény);
            }

            private void InitializeForm()
            {
                Text = "Eredmény";
                ClientSize = new System.Drawing.Size(464 - 64, 320 + 32);
                MinimumSize = ClientSize;
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            }

            private void InitializeContent()
            {
                Label név = new Label();
                név.Text = "Név:";
                név.Location = new System.Drawing.Point(32, 16 + 0 * 32);

                Label íjtípus = new Label();
                íjtípus.Text = "Íjtípus:";
                íjtípus.Location = new System.Drawing.Point(név.Location.X, 16 + 1 * 32);

                Label csapat = new Label();
                csapat.Text = "Csapatszám:";
                csapat.Location = new System.Drawing.Point(név.Location.X, 16 + 2 * 32);

                Label találat_10 = new Label();
                találat_10.Text = "Tíz találat:";
                találat_10.Location = new System.Drawing.Point(név.Location.X, 16 + 3 * 32);

                Label találat_8 = new Label();
                találat_8.Text = "Nyolc találat:";
                találat_8.Location = new System.Drawing.Point(név.Location.X, 16 + 4 * 32);

                Label találat_5 = new Label();
                találat_5.Text = "Öt találat:";
                találat_5.Location = new System.Drawing.Point(név.Location.X, 16 + 5 * 32);

                Label mellé = new Label();
                mellé.Text = "Mellé találat:";
                mellé.Location = new System.Drawing.Point(név.Location.X, 16 + 6 * 32);

                Label összes = new Label();
                összes.Text = "Összes találat:";
                összes.Location = new System.Drawing.Point(név.Location.X, 16 + 7 * 32);

                Label százalék = new Label();
                százalék.Text = "Eredmény százalék:";
                százalék.Location = new System.Drawing.Point(név.Location.X, 16 + 8 * 32);
                százalék.AutoSize = true;

                Label megjelent = new Label();
                megjelent.Text = "Megjelent:";
                megjelent.Location = new System.Drawing.Point(név.Location.X, 16 + 9 * 32);

                ///

                box_név = new TextBox();
                box_név.Location = new System.Drawing.Point(név.Location.X + név.Size.Width + 16, név.Location.Y);
                box_név.Size = new System.Drawing.Size(128 + 64, 24);

                combo_csapat = new ComboBox();
                combo_csapat.Location = new System.Drawing.Point(csapat.Location.X + csapat.Size.Width + 16, csapat.Location.Y);
                combo_csapat.Size = box_név.Size;
                combo_csapat.DropDownStyle = ComboBoxStyle.DropDownList;

                for (int i = 0; i < 35; i++) combo_csapat.Items.Add(i + 1);

                box_találat_10 = new TextBox();
                box_találat_10.Location = new System.Drawing.Point(találat_10.Location.X + találat_10.Size.Width + 16, találat_10.Location.Y);
                box_találat_10.Size = new System.Drawing.Size(64, 24);

                box_találat_8 = new TextBox();
                box_találat_8.Location = new System.Drawing.Point(találat_8.Location.X + találat_8.Size.Width + 16, találat_8.Location.Y);
                box_találat_8.Size = box_találat_10.Size;

                box_találat_5 = new TextBox();
                box_találat_5.Location = new System.Drawing.Point(találat_5.Location.X + találat_5.Size.Width + 16, találat_5.Location.Y);
                box_találat_5.Size = box_találat_10.Size;

                box_mellé = new TextBox();
                box_mellé.Location = new System.Drawing.Point(mellé.Location.X + mellé.Size.Width + 16, mellé.Location.Y);
                box_mellé.Size = box_találat_10.Size;

                label_összes = new Label();
                label_összes.Location = new System.Drawing.Point(összes.Location.X + összes.Size.Width + 16, összes.Location.Y);

                label_százalék = new Label();
                label_százalék.Location = new System.Drawing.Point(százalék.Location.X + százalék.Size.Width + 16, százalék.Location.Y);

                box_találat_10.TextChanged += eredmény_számolás;
                box_találat_8.TextChanged += eredmény_számolás;
                box_találat_5.TextChanged += eredmény_számolás;
                box_mellé.TextChanged += eredmény_számolás;

                //

                box_megjelent = new CheckBox();
                box_megjelent.Checked = false;
                box_megjelent.Location = new System.Drawing.Point(megjelent.Location.X + megjelent.Size.Width + 16, megjelent.Location.Y - 4);

                Button rendben = new Button();
                rendben.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                rendben.Text = "Rendben";
                rendben.Size = new System.Drawing.Size(96, 32);
                rendben.Location = new System.Drawing.Point(ClientRectangle.Width - 96 - 16, ClientRectangle.Height - 32 - 16);
                rendben.Click += rendben_Click;

                ///

                Controls.Add(név);
                Controls.Add(íjtípus);
                Controls.Add(csapat);
                Controls.Add(találat_10);
                Controls.Add(találat_8);
                Controls.Add(találat_5);
                Controls.Add(mellé);
                Controls.Add(összes);
                Controls.Add(százalék);
                Controls.Add(megjelent);

                Controls.Add(box_név);
                Controls.Add(Program.mainform.eredmény_panel.combo_íjtípusok);
                Controls.Add(combo_csapat);
                Controls.Add(box_találat_10);
                Controls.Add(box_találat_8);
                Controls.Add(box_találat_5);
                Controls.Add(box_mellé);
                Controls.Add(label_összes);
                Controls.Add(label_százalék);
                Controls.Add(box_megjelent);
                Controls.Add(rendben);
            }

            private void InitializeData(Eredmény _eredmény)
            {
                box_név.Text = _eredmény.név;
                box_név.Enabled = false;
                Program.mainform.eredmény_panel.combo_íjtípusok.Text = _eredmény.íjtípus;
                Program.mainform.eredmény_panel.combo_íjtípusok.Enabled = false;
                combo_csapat.SelectedItem = Convert.ToInt32(_eredmény.csapat);
                combo_csapat.Enabled = false;

                box_találat_10.Text = _eredmény.találat_10.ToString();
                box_találat_8.Text = _eredmény.találat_08.ToString();
                box_találat_5.Text = _eredmény.találat_05.ToString();
                box_mellé.Text = _eredmény.mellé.ToString();
                label_összes.Text = _eredmény.összpont.ToString();
                label_százalék.Text = _eredmény.százalék.ToString() + "%";
                box_megjelent.Checked = _eredmény.megjelent;
                box_megjelent.Enabled = false;
            }

            #region EventHandlers
            private void eredmény_számolás(object _sender, EventArgs _event)
            {
                int találat_10;
                try { találat_10 = Convert.ToInt32(box_találat_10.Text); if (találat_10 < 0) { return; } }
                catch { return; }
                int találat_8;
                try { találat_8 = Convert.ToInt32(box_találat_8.Text); if (találat_8 < 0) { return; } }
                catch { return; }
                int találat_5;
                try { találat_5 = Convert.ToInt32(box_találat_5.Text); if (találat_5 < 0) { return; } }
                catch { return; }
                int találat_mellé;
                try { találat_mellé = Convert.ToInt32(box_mellé.Text); if (találat_mellé < 0) { return; } }
                catch { return; }

                label_összes.Text = (Convert.ToInt32(box_találat_10.Text) * 10 + Convert.ToInt32(box_találat_8.Text) * 8 + Convert.ToInt32(box_találat_5.Text) * 5).ToString();
                label_százalék.Text = ((int)(((double)Convert.ToInt32(label_összes.Text) / (összespont * 10)) * 100)).ToString() + "%";
            }

            private void rendben_Click(object _sender, EventArgs _event)
            {
                if (!(0 < box_név.Text.Length && box_név.Text.Length <= 30)) { MessageBox.Show("Nem megfelelő a név hossza (1 - 30 hosszú kell legyen)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (Program.mainform.eredmény_panel.combo_íjtípusok.SelectedItem == null) { MessageBox.Show("Nincs kiválasztva íjtípus!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                int találat_10;
                try { találat_10 = Convert.ToInt32(box_találat_10.Text); if (találat_10 < 0) { MessageBox.Show("Nem megfelelő a 10 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } }
                catch { MessageBox.Show("Nem megfelelő a 10 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                int találat_8;
                try { találat_8 = Convert.ToInt32(box_találat_8.Text); if (találat_8 < 0) { MessageBox.Show("Nem megfelelő a 8 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } }
                catch { MessageBox.Show("Nem megfelelő a 8 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                int találat_5;
                try { találat_5 = Convert.ToInt32(box_találat_5.Text); if (találat_5 < 0) { MessageBox.Show("Nem megfelelő az 5 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } }
                catch { MessageBox.Show("Nem megfelelő az 5 találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                int találat_mellé;
                try { találat_mellé = Convert.ToInt32(box_mellé.Text); if (találat_mellé < 0) { MessageBox.Show("Nem megfelelő a mellé találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } }
                catch { MessageBox.Show("Nem megfelelő a mellé találatok formátuma!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                int összes = találat_10 * 10 + találat_8 * 8 + találat_5 * 5;
                int százalék = (int)(((double)összes / (összespont * 10)) * 100);

                if (!((találat_10 == 0 && találat_8 == 0 && találat_5 == 0 && találat_mellé == 0) || (összespont == találat_10 + találat_8 + találat_5 + találat_mellé))) { MessageBox.Show("Nem megfelelő a lövések darabszáma!\n" + "Lövések darabszáma: " + (összespont).ToString() + "\nBeírt lövések: " + (találat_10 + találat_8 + találat_5 + találat_mellé).ToString(), "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                Program.network.Send(ClientCommand.EREDMÉNY_MÓDOSÍTÁS, verseny_azonosító + ";" + eredeti.név + ";" + Program.mainform.eredmény_panel.combo_íjtípusok.Text + ";" + (combo_csapat.SelectedIndex + 1) +
                    ";" + box_találat_10.Text + ";" + box_találat_8.Text + ";" + box_találat_5.Text + ";" + box_mellé.Text + ";" + box_megjelent.Checked);

                Close();
            }
            #endregion
        }
    }
}
