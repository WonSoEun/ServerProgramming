using System.Net.Sockets;

public class MyTcpClient
{
    private static Mutex clientMutex = new Mutex();
    private static bool isYouinput = false; // 상대방이 입력하고 있는가를 판단함. (true - 입력중, false - 입력x)
    private static bool isReceive = false; // 내가 입력중인데 메세지를 받고 있는가를 판단함.

    private static void Send(NetworkStream stream, LinkedList<string> list) //보내기 쓰래드
    {
        String message = null;

        while (Console.ReadKey().Key == ConsoleKey.Enter) //enter 키 입력 시 작성 가능 
        {

            while (isYouinput && Console.ReadKey().Key == ConsoleKey.Enter) //상대방이 입력하고 있는 도중에 보낼 때
            {
                if (isYouinput)
                {
                    clientMutex.WaitOne();

                    Console.Write("[수] ");
                    message = Console.ReadLine();


                    list.AddLast("[수] "+message);

                    Byte[] sendMessage = System.Text.Encoding.Default.GetBytes(message);
                    stream.Write(sendMessage, 0, sendMessage.Length);
                    clientMutex.ReleaseMutex();
                }

            }

            string waiting = "수> 대화를 입력중입니다..";
            Byte[] waitData = System.Text.Encoding.Default.GetBytes(waiting);

            stream.Write(waitData, 0, waitData.Length);
            isReceive = true;

            Console.Write("[수] ");
            message = Console.ReadLine();


            list.AddLast("[수] "+message);
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


            if (message =="/q") //  /q를 입력했을 때, 시스템 종료
            {
                Environment.Exit(0);
            }



            Byte[] data = System.Text.Encoding.Default.GetBytes(message);

            stream.Write(data, 0, data.Length);  //보내기



        }


    }

    private static void Receive(NetworkStream stream, LinkedList<string> list) //받기 쓰래드
    {
        Byte[] bytes = new Byte[256];

        String responseData = String.Empty;

        try
        {
            while (true)
            {
                Int32 bytesCli = stream.Read(bytes, 0, bytes.Length);
                responseData = System.Text.Encoding.UTF8.GetString(bytes, 0, bytesCli);

                if (responseData == "주> 대화를 입력중입니다..")
                {
                    Console.WriteLine();
                    Console.WriteLine(responseData);
                    isYouinput = true;
                }
                else
                {
                    if (isReceive==false)
                    {
                        Console.Clear();
                        UIPopUp();
                    }


                    list.AddLast("[주] "+responseData);

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
        Console.WriteLine("금지사항 : Enter키 이후 [수] 가 나왔을 때까지 절대 아무키도 입력하지 마세요.");
        Console.WriteLine("금지사항 : 공백따위 입력하지 마시오. (개발자의 마음이 아파집니다.)");
        Console.WriteLine("===============================================");
        Console.WriteLine();
    }

    public static void Main()
    {
        LinkedList<String> list = new LinkedList<string>();
        try
        {
            string[] inputServerPort = new string[3];
            string[] serverPort = new string[3];
            string server = null;
            Int32 port = 0;
            UIPopUp();

            Console.WriteLine("/// Enter 후 연결할 서버 IP주소와 포트번호를 입력하시오. 양식을 지켜주세요. (ex./c (IP주소):(포트번호) ///");

            if (Console.ReadKey().Key == ConsoleKey.Enter) //포트 연결
            {
                Console.Write("[수] ");
                inputServerPort = Console.ReadLine().Split();

                serverPort[0] = inputServerPort[1];
                serverPort = serverPort[0].Split(":");

                server = serverPort[0];
                port = int.Parse(serverPort[1]);

                if (server != "127.0.0.1" && port!=9000)
                {
                    Console.WriteLine("연결할 수 없습니다.");
                }
                else
                {
                    //포트연결 성공
                    Console.Clear();
                    UIPopUp();
                    Console.WriteLine("127.0.0.1:9000에 접속시도중..");
                    TcpClient client = new TcpClient(server, port);

                    Console.WriteLine("'주'님께 연결되었습니다.");

                    NetworkStream stream = client.GetStream();


                    Thread thread_S = new Thread(() => Send(stream, list));
                    thread_S.Start();


                    Thread thread_R = new Thread(() => Receive(stream, list));
                    thread_R.Start();
                    thread_R.Join();


                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("다시 연결해주세요.");
            }


        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

    }
}
