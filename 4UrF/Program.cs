using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace _4UrF
{
    class Program
    {
        static string username;
        static string key;
        static bool checking;
        static bool newmsg = false;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Username? :");
                username = Console.ReadLine();
                Console.Write("Key? :");
                key = Console.ReadLine();
                Console.WriteLine(Decrypt(key, "TheI$MYfR33D==M²³{[]}!§$%&/()="));
                try
                {
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");
                    tcpclnt.Connect(Decrypt(key, "TheI$MYfR33D==M²³{[]}!§$%&/()="), 8081);
                    Stream stm = tcpclnt.GetStream();
                    Console.Clear();
                    Console.WriteLine("Connected!");
                    Task recv = new Task(() => 
                    {
                        while (true)
                        {
                            try
                            {
                                if (!checking)
                                {
                                    newmsg = true;
                                    Console.WriteLine(Decrypt(RecvFromServer(stm), "TheI$MYfR33D==M²³{[]}!§$%&/()="));
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    checking = false;
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    });
                    recv.Start();
                    while (true)
                    {
                        string entry = Encrypt(username + ": " + Console.ReadLine(), "TheI$MYfR33D==M²³{[]}!§$%&/()=");
                        checking = true;
                        SendToServer(entry, stm);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to Connect... Retry");
                    Console.ReadLine();
                    continue;
                }
            }
        }
        static void SendToServer(string text, Stream stm)
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
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;
        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
        public static string Encrypt(string plainText, string passPhrase)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
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
    }
}
