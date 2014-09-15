using System;
using System.Windows.Forms;

namespace Íjász_segéd
{
    public static class Program
    {
        public static MainForm mainform;
        public static Network network;

        [STAThread]
        public static void Main()
        {
            try
            {
                mainform = new MainForm();
                network = new Network();

                AddressForm addressform;
                do
                {
                    addressform = new AddressForm();
                    Application.Run(addressform);
                    if (!addressform.accepted) return;
                } while ((!(addressform.accepted && network.Connect(addressform.address, addressform.port, addressform.név))));

                Application.Run(mainform);
            }
            catch (Exception e)
            {
                MessageBox.Show("Váratlan hiba:\n- " + e.Message + "\n\nAz Íjász segédet újra kell indítani!", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                System.Environment.Exit(1);
            }
            finally
            {
                network.Shutdown();
            }
        }

        public static bool IsCorrectSQLText(string _text)
        {
            if (_text.Contains("'") || _text.Contains("\"") || _text.Contains("(") || _text.Contains(")") || _text.Contains(";")) return false;
            return true;
        }
    }
}
