using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace RSASignature
{
    public class BackendServer
    {
        private TcpListener server;
        public string ip;
        public int port;

        public BackendServer(string ip , int host) {
            IPAddress iPAddress = IPAddress.Parse(ip);
            server = new TcpListener(iPAddress, host);
        }

        public BackendServer()
        {
            this.ip = "127.0.0.1";
            port = 5001;
            //IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            IPAddress ip = IPAddress.Parse (this.ip);
            server = new TcpListener(ip, port);
        }
        public string GetLocalIPAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                               .AddressList
                               .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                               ?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting local IP address: " + ex.Message);
            }
            return ipAddress;
        }

        public async void StartListening()
        {
            try
            {
                server.Start();
                Trace.WriteLine("Server started. Listening for incoming connections...");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                    Trace.WriteLine("Client connected");


                    _ = HandleClientAsync(client);
                }
            }
            catch (SocketException ex)
            {
                Trace.WriteLine("Error while listening for connections: " + ex.Message);
            }
        }
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(stream);
                    object[] arr = obj as object[];

                    Trace.WriteLine(arr[0] as string);

                    // Now 'obj' contains the deserialized object sent by the client
                    // You can cast it to the appropriate type and process it further
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling client connection: " + ex.Message);
            }
            finally
            {
                // Ensure the client is closed properly
                client.Close();
            }
        }
    }

}
