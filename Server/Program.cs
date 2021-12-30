using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Server
{
    class Program
    {
        static int countOfClients = 0;
        static List<Socket> socketList = new List<Socket>();
        static void Main(string[] args)
        {
            Console.WriteLine("4_Ur_Fr33D0M Server");
            Console.Write("Enter your Port: ");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Server starting...");
            try
            {
                Console.Title = "Clients online: " + countOfClients.ToString();
                TcpListener myList = new TcpListener(IPAddress.Any, port);
                myList.Start();
                Console.WriteLine("The server is running at port 8081...");
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                while (true)
                {
                    try
                    {
                        Socket s = myList.AcceptSocket();
                        var childSocketThread = new Thread(() =>
                        {
                            countOfClients += 1;
                            socketList.Add(s);
                            Console.Title = "Clients online: " + countOfClients.ToString();
                            try
                            {
                                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
                                while (true)
                                {
                                    try
                                    {
                                        string text = recvFromClient(s);
                                        if (s.Connected || !s.Blocking)
                                        {
                                            if (string.IsNullOrEmpty(text))
                                            {
                                                throw new Exception("CID");
                                            }
                                            Console.WriteLine("Received From Client " + s.RemoteEndPoint + ": " + text);
                                            SendToAll(text);
                                        }
                                        else
                                        {
                                            throw new Exception("CID");
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                }
                                throw new Exception("CID");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Eine Verbindung wurde unterbrochen..." + s.RemoteEndPoint);
                                s.Close();
                                countOfClients -= 1;
                                Console.Title = "Clients online: " + countOfClients.ToString();
                            }
                        });
                        childSocketThread.Start();
                    }
                    catch (Exception es)
                    {
                        countOfClients -= 1;
                        Console.Title = "Clients online: " + countOfClients.ToString();
                        Console.WriteLine(es.Message);
                        break;
                    }
                }
                myList.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }
        }
        static void SendToClient(string text, Socket s)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes(text));
        }
        static string recvFromClient(Socket s)
        {
            byte[] b = new byte[4096];
            int k = s.Receive(b);
            string Command = string.Empty;
            for (int i = 0; i < k; i++)
            {
                Command = Command + Convert.ToChar(b[i]);
            }
            return Command;
        }
        static void SendToAll(string text)
        {
            foreach (Socket s in socketList)
            {
                try
                {
                    SendToClient(text, s);
                    Console.WriteLine("sended");
                }
                catch (Exception es) 
                {
                    Console.WriteLine(es.Message);
                }
            }
        }
    }
}
