using System;
using System.Net;
using System.Windows.Forms;

namespace Íjász_segéd
{
    public class AddressForm : Form
    {
        public bool accepted = false;
        public int port;
        public IPAddress address;
        public string név;

        private TextBox port_box;
        private TextBox address_box;
        private TextBox név_box;

        public AddressForm()
        {
            InitializeForm();
            InitializeContent();
        }

        private void InitializeForm()
        {
            Text = "Szerver hálózati cím megadása";
            ClientSize = new System.Drawing.Size(256 + 64 + 16, 96);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximumSize = Size;
        }

        private void InitializeContent()
        {
            Label név_label = new Label();
            név_label.Text = "Név:";
            név_label.Size = new System.Drawing.Size(48, 24);
            név_label.Location = new System.Drawing.Point(16, 16 + 0 * 24); 
            
            Label address_label = new Label();
            address_label.Text = "Cím:";
            address_label.Size = new System.Drawing.Size(48, 24);
            address_label.Location = new System.Drawing.Point(16, 16 + 1 * 24);

            Label port_label = new Label();
            port_label.Text = "Port:";
            port_label.Size = new System.Drawing.Size(48, 24);
            port_label.Location = new System.Drawing.Point(16, 16 + 2 * 24);

            ///

            név_box = new TextBox();
            név_box.Size = new System.Drawing.Size(128, 24);
            név_box.Location = new System.Drawing.Point(név_label.Location.X + név_label.Size.Width + 16, név_label.Location.Y);

            address_box = new TextBox();
            address_box.Size = new System.Drawing.Size(128, 24);
            address_box.Text = "192.168.1.101";
            address_box.Location = new System.Drawing.Point(address_label.Location.X + address_label.Size.Width + 16, address_label.Location.Y);

            port_box = new TextBox();
            port_box.Size = new System.Drawing.Size(128, 24);
            port_box.Location = new System.Drawing.Point(port_label.Location.X + port_label.Size.Width + 16, port_label.Location.Y);

            Button accept_button = new Button();
            accept_button.Text = "Rendben";
            accept_button.Size = new System.Drawing.Size(96, 24);
            accept_button.Location = new System.Drawing.Point(address_box.Location.X + address_box.Size.Width + 16, port_box.Location.Y);
            accept_button.Click += accept_button_Click;

            Controls.Add(név_label);
            Controls.Add(address_label);
            Controls.Add(port_label);
            Controls.Add(név_box);
            Controls.Add(address_box);
            Controls.Add(port_box);
            Controls.Add(accept_button);
        }

        private void accept_button_Click(object _sender, EventArgs _event)
        {
            if (!(0 < név_box.Text.Length && név_box.Text.Length <= 64)) { MessageBox.Show("Nem megfelelő a név hossza(1-64)!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            név = név_box.Text;

            try { address = IPAddress.Parse(address_box.Text); }
            catch { MessageBox.Show("Nem megfelelő az IP-cím!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            try { port = Convert.ToInt32(port_box.Text); }
            catch { MessageBox.Show("Nem megfelelő a port!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            accepted = true;

            Close();
        }
    }

}
