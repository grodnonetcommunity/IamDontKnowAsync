﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IamDontKnowAsync
{
    class Program
    {
        static void Main()
        {
            MakeRequest();

            Console.ReadLine();
        }

        public static void MakeRequest()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            const string host = "localhost";
            
            socket.ConnectTask(new DnsEndPoint(host, 80))
                .ContinueWith(r0 =>
                {
                    var request = $"GET http://{host}/ HTTP/1.1\r\n" +
                                  "Host: localhost\r\n" +
                                  "Accept: text/html\r\n\r\n";

                    var buffer = Encoding.ASCII.GetBytes(request);
                    socket.SendTask(buffer, 0, buffer.Length)
                        .ContinueWith(r1 =>
                        {
                            var sended = r1.Result;

                            Console.WriteLine($"Request sended: {sended}");

                            var response = new byte[1 * 1024 * 1024];

                            socket.ReceivedTask(response, 0, response.Length)
                                .ContinueWith(r2 =>
                                {
                                    var received = r2.Result;

                                    Console.WriteLine($"Response received: {received}");
                                    Console.WriteLine(Encoding.UTF8.GetString(response, 0, received));

                                    socket.DisconnectTask(false)
                                        .ContinueWith(r3 =>
                                        {
                                            Console.WriteLine("Disconnected");
                                        });
                                });
                        });
                });
        }
    }
}
