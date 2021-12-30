using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4UrF_GUI
{
    public partial class Connector : Form
    {
        string username = "";
        string ky = "<RSAKeyValue><Modulus>rQ2azKUgbLSaBv0M4iqBhSEaInj21vEs8hngxvUrkTI3s9QjKqujKVNHZP8zL/hAyZ1d1HheUdXXgVqmRAeRB3OQt7lo+6AFKvjBx5E1M9ILG8C4NfJMfI+QowvLxHaqSuuyR76FFliX1fWa0M+9f7aaA32dfxEAUPOesBpzKELCG1aViGrnNnlgua9WdJXnsQGoud7LHz+7E7o0t+JUpPkFxOrhtJ2RWhQ4O4JPVlBbkRF+TFXowwTRQNyE0EbNEEE/iXcdQjS4nknZw25CDkpL708+UZ2VNQZy3T0A7vQRZN4JakhjtfJ/Xa12NqTT82I8AM94dF1WHfEUj59OJw==</Modulus><Exponent>AQAB</Exponent><P>0y6vpAThKg8kvshD78FARUNOBF3jZBsTGofpQlQRFEiD8UoG0zHc70zNFAReGmwXT3UV1mD+hAZXfyZqeUcRNGNbJFP136ISK/NTpDTdIMW2jh4pieaFAbOV1b42Mmq2D5zzoGQcPvk2j79cq6u11c5WZSme5/TiQBj5RYssC8U=</P><Q>0cdl9wjaaCX98ijUSwOU6CwRl1Cnx48ZsuikI3RqdptpIx3IUtHWvGfNRu6ZfCS9AylqOE0+G+YcJMp3ZP267hivHNz6x0eQwx6dpFJtKpXdIDDYTOf+aFSpQiiQNEooVC9Q6Z2GXcBMRmhXYYz4NPZqibuYVFUx1kCpmYrY9Ps=</Q><DP>pc/VRqIyLLThNA7IELqafQegfRs+zD3Z3Q3znwmks1lQh6aI/0/6WcXii6H2RSazksaq070qeX0FPCdsmDatkoWkbTyjI+j7/Zg7BwYezXmPobya/tSJgFGCBuYnZaSVuGKSg0+3Qdao+WrWF1BmIC63dQyd4SBMeOli5zCj78U=</DP><DQ>zZx+AVppD8mlFPQV2AyDp+iBuWjxefR1gNjrAGE7dFMxAp1PWnWX3oRzPEHiqD4uEGpIVTTIWnn5PcpAqfjTfkktYcHp/ubTKZvZcVTk8OeAm+ukJgUBY2sPluvBI69HxfO2f7PJHhy75gqhEtkj8m1P0P3Q+V+jgFdSpE7mBX8=</DQ><InverseQ>DZtXrQe1AXuvNsOlUHwFq2ew95EMIzQPTkUv/pl3q+m+lsHVKg7dF3xdNXuq0GwOIvshfgE831MNHaHwjdRXX9Z3IBKltRuJOQVBKgnwOaoLqXe6KGi2sQCTtCrZSU0sDJjgezV+Umi48S5+PFViAaKpCOBXgshnE/YaMkLHc80=</InverseQ><D>MA7PJnXVA1tESxyLL2POhEIlF92v7xEbAkDdg2Mirjfb8DzBZ2Ay948BDLMya+ftVhA1toOW7fMM3gdmlQ3k4MTfM6Zx2S8fQEfEyL8ynyl9nD9w6MpZI15bNGxaMj0iHZxG7lWicAo6YFjMnaCTZ5w2OTHxQxBdnh2JvC4cPNmYVngtF1UJrk18k6AjODO1hfSS2spmRgTzqZUQArvw24ONi+Xfwcmy1Ll7EmIz3eQMOBDbhnRPHgdDJirdhW6TYIXFLKBqzT3vUuJlsuXp5MeCDrpwPKBPzSevwzI8slTC2NYH4+/lysTXoP2agqy+QgzZdQhZ7QRaiRGPGC4ZwQ==</D></RSAKeyValue>";

        static Stream stm;
        public Connector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sending();
        }

        private void Connector_Load(object sender, EventArgs e)
        {
            textBox1.ScrollBars = ScrollBars.Horizontal;
            textBox1.Text = "";
            textBox2.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;
        }

        private void textBox5_Click_1(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        private void textBox4_Click_1(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }
        public delegate void ControlStringConsumer(Control control, string text);  // defines a delegate type

        public void SetText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new ControlStringConsumer(SetText), new object[] { control, text });  // invoking itself
            }
            else
            {
                textBox1.Text += Environment.NewLine + "____________________________________________________________________" + Decrypt(text, ky);
            }
        }
        public static string Decrypt(string cipher, string PrivateKey)
        {
            byte[] decrypted;

            using (RSACryptoServiceProvider RSA2048 = new RSACryptoServiceProvider(2048))
            {
                RSA2048.FromXmlString(PrivateKey);
                decrypted = RSA2048.Decrypt(Convert.FromBase64String(cipher), false);
            }

            return Encoding.UTF8.GetString(decrypted);
        }
        public static string Encrypt(string plain, string PublicKey)
        {
            byte[] encrypted;

            using (RSACryptoServiceProvider RSA2048 = new RSACryptoServiceProvider(2048))
            {
                RSA2048.FromXmlString(PublicKey);
                encrypted = RSA2048.Encrypt(Encoding.UTF8.GetBytes(plain), false);
            }

            return Convert.ToBase64String(encrypted);
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        static void SendToServer(string text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(text);
            stm.Write(asen.GetBytes(text), 0, ba.Length);
        }
        static string RecvFromServer(Stream stm)
        {
            byte[] bb = new byte[4096];
            int k = stm.Read(bb, 0, 4096);
            string recv = "";
            for (int i = 0; i < k; i++)
                recv += Convert.ToChar(bb[i]).ToString();
            return recv;
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Sending();
            }
        }
        void Sending()
        {
            if (textBox2.Text.StartsWith("/"))
            {
                if (textBox2.Text.StartsWith("/clear"))
                {
                    textBox1.Text = "";
                }
                else if (textBox2.Text.StartsWith("/exit"))
                {
                    Application.Exit();
                }
                else
                {
                    textBox2.Text += Environment.NewLine + "Dieses Kommando existiert nicht!";
                }
            }
            else
            {
                string entry = Encrypt(username + ": " + textBox2.Text, ky);
                SendToServer(entry);
            }
            textBox2.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void Connector_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SendToServer(Encrypt("SERVER: " + username + " LEFT THE PARTY", ky));
            }
            catch
            {
                Application.Exit();
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(textBox5.Text, int.Parse(textBox3.Text));
                stm = tcpclnt.GetStream();
                Task recv = new Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            SetText(textBox1, RecvFromServer(stm));
                        }
                        catch (Exception es)
                        {
                            MessageBox.Show(es.Message);
                        }
                    }
                });
                recv.Start();
                panel1.Visible = false;
                textBox2.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
                this.Text = "4UrFr33D0W";
                timer1.Start();
                username = textBox4.Text;
                SendToServer(Encrypt("SERVER: " + username + " JOINED THE PARTY !", ky));
            }
            catch (Exception es)
            {
                MessageBox.Show("Failed to Connect" + Environment.NewLine + es.Message);
            }
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                textBox5.Text = "Enter IP";
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox3.Text = "Enter Port";
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                textBox4.Text = "Enter Username";
            }
        }

    }
}
