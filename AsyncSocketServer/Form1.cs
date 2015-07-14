using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocketServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Socket m_ServerSocket;
        private List<Socket> m_ClientSocket;
        private byte[] szData;
        private void Form1_Load(object sender, EventArgs e)
        {
            m_ClientSocket = new List<Socket>();

            m_ServerSocket = new Socket(
                                AddressFamily.InterNetwork, 
                                SocketType.Stream, 
                                ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 10000);
            m_ServerSocket.Bind(ipep);  
            m_ServerSocket.Listen(20);  

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed 
                += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            m_ServerSocket.AcceptAsync(args);
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;
            m_ClientSocket.Add(ClientSocket);

            if (m_ClientSocket != null)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                szData = new byte[1024]; 
                args.SetBuffer(szData, 0, 1024);
                args.UserToken = m_ClientSocket;
                args.Completed 
                    += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                ClientSocket.ReceiveAsync(args);
            }
            e.AcceptSocket = null;
            m_ServerSocket.AcceptAsync(e);
        }
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] szData = e.Buffer;    // 데이터 수신
                string sData = Encoding.Unicode.GetString(szData);

                string Test = sData.Replace("\0", "").Trim();
                SetText(Test);
                for (int i = 0; i < szData.Length; i++)
                {
                    szData[i] = 0;
                }
                e.SetBuffer(szData, 0, 1024);
                ClientSocket.ReceiveAsync(e);
            }
            else
            {
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
                m_ClientSocket.Remove(ClientSocket);
            }
        }
        private delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (richTextBox1.TextLength > 0)
                {
                    richTextBox1.AppendText("\n");
                }
                richTextBox1.AppendText(text);
                richTextBox1.ScrollToCaret();
            }
        }
        protected override void Dispose(bool disposing)
        {
            foreach (Socket pBuffer in m_ClientSocket)
            {
                if(pBuffer.Connected)
                    pBuffer.Disconnect(false);
                pBuffer.Dispose();
            }
            m_ServerSocket.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
