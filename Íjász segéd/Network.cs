using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Íjász_segéd
{
    public enum ClientCommand
    {
        NONE = 0,
        LOGIN = 1,

        INDULÓ_HOZZÁADÁS = 10,
        INDULÓ_MÓDOSÍTÁS = 12,
        INDULÓ_BEÍRÁS = 13,

        EREDMÉNY_MÓDOSÍTÁS = 22,
        EREDMÉNYEK = 23
    }

    public enum ServerCommand
    {
        NONE = 0,
        ERROR = 1,

        INDULÓ_HOZZÁADÁS = 12,
        INDULÓ_MÓDOSÍTÁS = 13,
        INDULÓ_ÁTNEVEZÉS = 14,
        INDULÓ_TÖRLÉS = 15,

        VERSENY_HOZZÁADÁS = 20,
        VERSENY_MÓDOSÍTÁS = 21,
        VERSENY_TÖRLÉS = 22,
        VERSENY_LEZÁRÁS = 23,
        VERSENY_MEGNYITÁS = 24,

        ÍJTÍPUS_HOZZÁADÁS = 30,
        ÍJTÍPUS_MÓDOSÍTÁS = 31,
        ÍJTÍPUS_TÖRLÉS = 32,

        EREDMÉNY_ADAT = 40,
        EREDMÉNY_BEÍRÁS_HOZZÁADÁS = 41,
        EREDMÉNY_BEÍRÁS_MÓDOSÍTÁS = 42,
        EREDMÉNY_MÓDOSÍTÁS = 43,
        EREDMÉNY_TÖRLÉS = 44
    }

    public sealed class Network
    {
        public const int MAX_PACKET_SIZE = 4 * 256 * 512;

        private Thread thread;
        private Socket client;

        public Network()
        {

        }

        public bool Connect(IPAddress _address, int _port, string _név)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(_address, _port);

                // TODO majd kövi patch fícsör a timeout!
                /*IAsyncResult result = client.BeginConnect(_address, _port, null, null);
                if(!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
                {
                    MessageBox.Show("Nem sikerült kapcsolatot teremteni a szerverrel!", "Letelt idő!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }*/
            }
            catch (Exception e)
            {
                MessageBox.Show("Hiba a hálózat megnyitása során!\n" + e.Message, "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            thread = new Thread(new ThreadStart(Receive));
            thread.Start();

            Send(ClientCommand.LOGIN, _név);
            return true;
        }

        private void Receive()
        {
            try
            {
                byte[] packet = new byte[MAX_PACKET_SIZE];
                while (true)
                {
                    int size = client.Receive(packet, MAX_PACKET_SIZE, SocketFlags.None);

                    ProcessData(Encoding.Unicode.GetString(packet, 0, size).Split(new char[]{';'}));
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show("Hálózati hiba az üzenet fogadása során!\n" + e.Message, "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!client.Connected)
                {
                    MessageBox.Show("Megszűnt a kapcsolat az Íjásszal!\nA segéd most bezáródik!" + e.Message, "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Shutdown();
                    System.Environment.Exit(0);
                }
            }
        }

        public void Send(ClientCommand _command, string _data)
        {
            byte[] packet = System.Text.Encoding.Unicode.GetBytes(_command.ToString() + ";" + _data);
            try
            {
                client.Send(packet, packet.Length, SocketFlags.None);
            }
            catch (Exception e) { MessageBox.Show("Hálózati hiba az üzenet küldése során!\n" + e.Message, "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ProcessData(string[] data)
        {
            int c = -1;
            do
            {
                ServerCommand command = ServerCommand.NONE;
                try
                {
                     command = (ServerCommand)Enum.Parse(typeof(ServerCommand), data[++c]);
                }
                catch { return; }

                switch (command)
                {
                    case ServerCommand.INDULÓ_HOZZÁADÁS:
                        if (data.Length <= c + 6) { MessageBox.Show("Hálózati hiba!\nA kapott induló hozzáadás adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.induló_panel.Induló_Hozzáadás(new Induló(data[++c], data[++c], data[++c], data[++c], data[++c], Convert.ToInt32(data[++c])));
                        break;

                    case ServerCommand.INDULÓ_MÓDOSÍTÁS:
                        if (data.Length <= c + 7) { MessageBox.Show("Hálózati hiba!\nA kapott induló módosítás adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.induló_panel.Induló_Módosítás(data[++c], new Induló(data[++c], data[++c], data[++c], data[++c], data[++c], Convert.ToInt32(data[++c])));
                        break;

                    case ServerCommand.INDULÓ_ÁTNEVEZÉS:
                        if (data.Length <= c + 2) { MessageBox.Show("Hálózati hiba!\nA kapott induló átnevezés adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.induló_panel.Induló_Átnevezés(data[++c], data[++c]);
                        break;

                    case ServerCommand.INDULÓ_TÖRLÉS:
                        Program.mainform.induló_panel.Induló_Törlés(data[++c]);
                        break;
                    
                    ///

                    case ServerCommand.VERSENY_HOZZÁADÁS:
                        if (data.Length <= c + 2) { MessageBox.Show("Hálózati hiba!\nA verseny adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Verseny_Hozzáadás(data[++c], Convert.ToInt32(data[++c]), Convert.ToBoolean(data[++c]));
                        break;

                    case ServerCommand.VERSENY_MÓDOSÍTÁS:
                        if (data.Length <= c + 3) { MessageBox.Show("Hálózati hiba!\nA verseny módosítás adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Verseny_Módosítás(data[++c], data[++c], Convert.ToInt32(data[++c]));
                        break;

                    case ServerCommand.VERSENY_TÖRLÉS:
                        Program.mainform.eredmény_panel.Verseny_Törlés(data[++c]);
                        break;

                    case ServerCommand.VERSENY_LEZÁRÁS:
                        Program.mainform.eredmény_panel.Verseny_Lezárás(data[++c]);
                        break;

                    case ServerCommand.VERSENY_MEGNYITÁS:
                        Program.mainform.eredmény_panel.Verseny_Megnyitás(data[++c]);
                        break;

                    ///

                    case ServerCommand.ÍJTÍPUS_HOZZÁADÁS:
                        Program.mainform.eredmény_panel.Íjtípus_Hozzáadás(data[++c]);
                        break;

                    case ServerCommand.ÍJTÍPUS_MÓDOSÍTÁS:
                        Program.mainform.eredmény_panel.Íjtípus_Módosítás(data[++c], data[++c]);
                        break;

                    case ServerCommand.ÍJTÍPUS_TÖRLÉS:
                        Program.mainform.eredmény_panel.Íjtípus_Törlés(data[++c]);
                        break;

                    ///

                    case ServerCommand.EREDMÉNY_ADAT:
                        if (data.Length <= c + 12)
                        { MessageBox.Show("Hálózati hiba!\nAz eredmény hozzáadás adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Eredmény_Hozzáadás(data[++c], new Eredmény(data[++c], Convert.ToInt64(data[++c]), data[++c], Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]),
                            Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]),
                            Convert.ToBoolean(data[++c])));
                            break;

                    case ServerCommand.EREDMÉNY_BEÍRÁS_HOZZÁADÁS:
                        if (data.Length <= c + 6)
                        { MessageBox.Show("Hálózati hiba!\nAz eredmény beírás(hozzáadás) adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Eredmény_Beírás_Hozzáadás(data[++c], data[++c], Convert.ToInt64(data[++c]), data[++c], Convert.ToInt32(data[++c]), Convert.ToBoolean(data[++c]));
                        break;

                    case ServerCommand.EREDMÉNY_BEÍRÁS_MÓDOSÍTÁS:
                        if (data.Length <= c + 5)
                        { MessageBox.Show("Hálózati hiba!\nAz eredmény beírás(hozzáadás) adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Eredmény_Beírás_Módosítás(data[++c], data[++c], data[++c], Convert.ToInt32(data[++c]), Convert.ToBoolean(data[++c]));
                        break;

                    case ServerCommand.EREDMÉNY_MÓDOSÍTÁS:
                        if (data.Length <= c + 8)
                        { MessageBox.Show("Hálózati hiba!\nAz eredmény módosítás adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Eredmény_Módosítás(data[++c], data[++c], Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]),
                            Convert.ToInt32(data[++c]), Convert.ToInt32(data[++c]));
                        break;

                    case ServerCommand.EREDMÉNY_TÖRLÉS:
                        if (data.Length <= c + 2)
                        { MessageBox.Show("Hálózati hiba!\nAz eredmény törlés adatainak száma nem megfelelő!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                        Program.mainform.eredmény_panel.Eredmény_Törlés(data[++c], data[++c]);
                        break;

                    ///

                    case ServerCommand.ERROR:
                        MessageBox.Show("Hibaüzenet az Íjásztól!\n" + data[++c], "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            } while (c < data.Length);
        }

        public void Shutdown()
        {
            if (thread != null) thread.Abort();
            if (client != null) client.Close();
        }
    }
}
