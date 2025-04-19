using System.Net;
using System.Net.Sockets;

class MyTcpListener
{
    private static Mutex serverMutex = new Mutex();
    private static bool isYouinput = false; // 상대방이 입력하고 있는가를 판단함. (true - 입력중, false - 입력x)
    private static bool isReceive = false; // 내가 입력중인데 메세지를 받고 있는가를 판단함.


    private static void Send(NetworkStream stream, LinkedList<string> list) //보내기
    {
        String data = null;

        while (Console.ReadKey().Key == ConsoleKey.Enter)
        {
            while (isYouinput && Console.ReadKey().Key == ConsoleKey.Enter)
            {
                if (isYouinput)
                {

                    serverMutex.WaitOne();

                    Console.Write("[주] ");
                    data = Console.ReadLine();


                    list.AddLast("[주] "+ data);

                    Byte[] sendMessage = System.Text.Encoding.Default.GetBytes(data);
                    stream.Write(sendMessage, 0, sendMessage.Length);
                    serverMutex.ReleaseMutex();
                }


            }

            string waiting = "주> 대화를 입력중입니다..";
            Byte[] waitData = System.Text.Encoding.Default.GetBytes(waiting);

            stream.Write(waitData, 0, waitData.Length);
            isReceive=true;

            Console.Write("[주] ");
            data = Console.ReadLine();


            list.AddLast("[주] "+ data);
            isReceive=false;

            if (isReceive==false)
            {
                if (list.Count>10)
                {
                    Console.Clear();
                    UIPopUp();
                    while (list.Count>10)
                    {
                        list.RemoveFirst();
                    }
                    foreach (string b in list)
                    {

                        Console.WriteLine(b);
                    }
                }
                else
                {
                    Console.Clear();
                    UIPopUp();
                    foreach (string a in list)
                    {

                        Console.WriteLine(a);

                    }
                }
            }


            if (data =="/q")
            {
                Environment.Exit(0);
            }


            byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

            stream.Write(msg, 0, msg.Length); //보내기

        }


    }

    private static void Receive(NetworkStream stream, LinkedList<string> list) //받기
    {

        Byte[] bytes = new Byte[256];
        String data = null;
        int i;
        try
        {
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {

                data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);

                if (data == "수> 대화를 입력중입니다..")
                {
                    Console.WriteLine();
                    Console.WriteLine(data);
                    isYouinput = true;
                }
                else
                {
                    if (isReceive==false)
                    {
                        Console.Clear();
                        UIPopUp();
                    }

                    list.AddLast("[수] "+data);     //전송받은 내용 List에 넣기


                    if (isReceive==false)
                    {

                        if (list.Count>10)
                        {
                            while (list.Count>10)
                            {
                                list.RemoveFirst();
                            }
                            foreach (string b in list)
                            {
                                Console.WriteLine(b);
                            }
                            isYouinput=false;
                        }
                        else
                        {
                            foreach (string a in list)
                            {
                                Console.WriteLine(a);

                            }
                            isYouinput=false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Environment.Exit(0);
        }





    }

    private static void UIPopUp()
    {
        Console.WriteLine("===============================================");
        Console.WriteLine("채팅 입력 : Enter");
        Console.WriteLine("채팅 종료 : /q");
        Console.WriteLine("금지사항 : Enter키 이후 [주] 가 나왔을 때까지 절대 아무키도 입력하지 마세요.");
        Console.WriteLine("금지사항 : 공백따위 입력하지 마시오. (개발자의 마음이 아파집니다.)");
        Console.WriteLine("===============================================");
        Console.WriteLine();

    }

    public static void Main()
    {
        LinkedList<String> list = new LinkedList<string>();
        TcpListener server = null;

        try
        {

            Int32 port = 9000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);


            server.Start();

            UIPopUp();

            Console.WriteLine("Waiting connection... ");


            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("'수' 님이 " + localAddr + "에서 접속하셨습니다.");


            NetworkStream stream = client.GetStream();

            Thread thread_S = new Thread(() => Send(stream, list));
            thread_S.Start();

            Thread thread_R = new Thread(() => Receive(stream, list));
            thread_R.Start();
            thread_R.Join();




        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }

        //Console.Read();

    }
}