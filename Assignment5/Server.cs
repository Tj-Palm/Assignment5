using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Assignment1;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace Assignment5
{
    class Server
    {
        private List<Book> books;
        //public static Book book;

        public Server()
        {
            books = new List<Book>();
            books.Add(new Book("Title", "Forfatter", 250, "qweqweqweqwer"));
            books.Add(new Book("Title1", "Forfatter1", 250, "qwertyqwertyu"));
            books.Add(new Book("Title2", "Forfatter2", 250, "asdasdasdasdf"));
            books.Add(new Book("Title3", "Forfatter3", 250, "asdfghasdfghj"));
        }


        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 4646);
            listener.Start();

            Console.WriteLine("Server startet");
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Client: " + socket.Client.RemoteEndPoint + " has connected.");
                Task.Run(() =>
                {
                    TcpClient tempSocket = socket;
                    DoClient(tempSocket);
                });
            }
        }

        public void DoClient(TcpClient socket)
        {
            using (socket)
            {
                NetworkStream ns = socket.GetStream();
                StreamWriter sw = new StreamWriter(ns);
                StreamReader sr = new StreamReader(ns);
                sw.AutoFlush = true;

                string line;
                while (true)
                {
                    line = sr.ReadLine();
                    Console.WriteLine("Client: " + line);

                    string[] readstr = line.Split(' ',2);

                    if (readstr[0] == "GetAll")
                    {
                        string getAllBook = JsonConvert.SerializeObject(books);
                        sw.WriteLine(getAllBook);
                    }

                    if (readstr[0] == "Get")
                    {
                        string getBook = JsonConvert.SerializeObject(books.FindAll(book => book.Isbn13 == readstr[1]));
                        sw.WriteLine(getBook);
                    }

                    if (readstr[0] == "Save")
                    {
                        Book saveBook = JsonConvert.DeserializeObject<Book>(readstr[1]);
                        books.Add(saveBook);
                    }
                }
            }
        }
    }
}