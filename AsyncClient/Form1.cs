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

namespace AsyncClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //그냥 Git을 테스트 해보기 위해서 하는거임
        //델컴에서 수정테스트
        //이번에는 메인에서 테스트 해본다
        private Socket m_ClientSocket;

        private void Form1_Load(object sender, EventArgs e)
        {
            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000); //포트 대기 설정
            //IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 10000); //포트 대기 설정
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = ipep;

            m_ClientSocket.ConnectAsync(args);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length > 0)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] szData = Encoding.Unicode.GetBytes(textBox1.Text);
                args.SetBuffer(szData, 0, szData.Length);
                m_ClientSocket.SendAsync(args);
                textBox1.Text = "";
                textBox1.Focus();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if(m_ClientSocket.Connected)
                m_ClientSocket.Disconnect(false);
            m_ClientSocket.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
