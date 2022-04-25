using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace WindowsFormsApp0425
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpListener Server;
        Socket Client;
        Thread Th_Svr;
        Thread Th_Clt;
        Hashtable HT = new Hashtable();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_Svr = new Thread(ServerSub);
            Th_Svr.IsBackground = true;
            Th_Svr.Start();
            button1.Enabled = false;
        }
        private void ServerSub()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));

            Server = new TcpListener(EP);
            Server.Start(100);
            while (true)
            {
                Client = Server.AcceptSocket();
                Th_Clt = new Thread(Listen);
                Th_Clt.IsBackground = true;
                Th_Clt.Start();
            }
        }
        private void Listen()
        {
            Socket Sck = Client;
            Thread Th = Th_Clt;
            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLen = Sck.Receive(B);
                    string Msg = Encoding.Default.GetString(B, 0, inLen);
                    string Cmd = Msg.Substring(0, 1);
                    string Str = Msg.Substring(1);
                    switch (Cmd)                                          //依據命令碼執行功能
                    {
                        case "0":                    //有新使用者上線：新增使用者到名單中
                            HT.Add(Str, Sck);        //連線加入雜湊表，Key:使用者，Value:連線物件(Socket)
                            listBox1.Items.Add(Str); //加入上線者名單
                            break;
                        case "9":
                            HT.Remove(Str);             //移除使用者名稱為Name的連線物件
                            listBox1.Items.Remove(Str); //自上線者名單移除Name
                            Th.Abort();                 //結束此客戶的監聽執行緒
                            break;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            }
        }
    }
}
