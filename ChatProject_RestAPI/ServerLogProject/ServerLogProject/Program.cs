using System.Net;

namespace soso
{

    class Users
    {
        public List<string> users = new List<string>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool isLogin = false; //로그인 성공여부 판단
            bool isLogout = false; //로그아웃 여부 판단
            Users user_arr = new Users();

            LinkedList<string> list = new LinkedList<string>();

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:3000/fuckyou/");
            listener.Start();
            Console.WriteLine("Listening..");

            string path = @"Log.txt";


            
            string serverLog = ""; //서버 로그내용
            
            while (true)
            {
                HttpListenerContext context = listener.GetContext(); //httplistener 객체
                HttpListenerRequest request = context.Request; // 요청
                HttpListenerResponse response = context.Response; // 응답

                Stream body = request.InputStream;
                System.Text.Encoding encoding = request.ContentEncoding;
                System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

                string s = reader.ReadToEnd(); //client에서의 Love 메세지를 읽어옴.


                string responseString = "";// 클라이언트로 보낼 메세지.

                string[] s_arr = s.Split(','); //Context-Type이랑 application/json이랑 나눔.

                

                if (s_arr.Length==1) //아이디 값을 받았을 때 실행
                {
                    
                    if (user_arr.users.Count==0) //서버에서 클라이언트로 로그인 성공여부 출력함.
                    {
                        isLogin=true;
                        user_arr.users.Add(s);
                        responseString = "login success";
                    }
                    else
                    {
                        user_arr.users.Add(s);
                        for (int i = 0; i < user_arr.users.Count; i++)
                        {

                            if (i!=user_arr.users.Count-1 && s==user_arr.users[i])
                            {
                                isLogin= false;
                                responseString = "login fail";

                                user_arr.users.RemoveAt(user_arr.users.Count-1); // 배열에서 제거.
                                break;
                            }
                            else
                            {
                                isLogin = true;
                                responseString = "login success";
                            }
                        }
                    }

                    if (isLogin) //로그인 성공할 때의 시간 체크
                    {
                        DateTime nowTime = DateTime.Now;
                        serverLog = serverLog+ s+" 님이 로그인 하셨습니다. "+nowTime +"\n";
                        File.WriteAllText(path, serverLog, System.Text.Encoding.Default);
                    }
                }

                if (s_arr.Length==2) //클라이언트에서 받아온 메세지 
                {

                    if (s_arr[1]=="\"msg\":\"logout\"}") //로그아웃 버튼 눌렀을 때
                    {
                        for (int i = 0; i < user_arr.users.Count; i++)
                        {
                            if (s_arr[0]+"}"==user_arr.users[i])
                            {
                                user_arr.users.RemoveAt(i); // 배열에서 제거.
                                isLogout=true;
                                break;
                            }
                            else
                            {
                                isLogout = false;
                            }

                            Console.WriteLine(user_arr.users[i]);
                        }

                        if (isLogout)
                        {
                            DateTime nowTime = DateTime.Now;
                            serverLog = serverLog+ s_arr[0]+"}"+" 님이 로그아웃 하셨습니다. "+nowTime +"\n";
                            File.WriteAllText(path, serverLog, System.Text.Encoding.Default);

                            string logoutText = "logoutSuccess";

                            byte[] buffer_logout = System.Text.Encoding.UTF8.GetBytes(logoutText); //buffer에 byte배열로 저장.
                            response.OutputStream.Write(buffer_logout, 0, buffer_logout.Length); //buffer에 저장한 내용을 클라이언트로 넘겨줌.
                            response.OutputStream.Close();
                        }
                        else
                        {
                            string logoutText = "logoutFail";

                            byte[] buffer_logout = System.Text.Encoding.UTF8.GetBytes(logoutText); //buffer에 byte배열로 저장.
                            response.OutputStream.Write(buffer_logout, 0, buffer_logout.Length); //buffer에 저장한 내용을 클라이언트로 넘겨줌.
                            response.OutputStream.Close();
                        }


                    }
                    else //클라이언트에서 메세지 받으면 저장해놓음.
                    {
                        // 누가, 몇시에, 어떤 메세지를 서버로 날렸다를 저장시켜놔야함.

                        DateTime nowTime = DateTime.Now;
                        serverLog = serverLog + s+" "+nowTime +"\n";
                        File.WriteAllText(path, serverLog, System.Text.Encoding.Default);

                        responseString = s_arr[0]+"} 님이 보낸 메세지 : "+"{"+s_arr[1]+"\n";

                        list.AddLast(responseString);


                        while(list.Count>20)
                        {
                            list.RemoveFirst();
                        }

                        foreach(string str in list)
                        {
                            byte[] bufferMsg = System.Text.Encoding.UTF8.GetBytes(str); //buffer에 byte배열로 저장.
                            response.OutputStream.Write(bufferMsg, 0, bufferMsg.Length); //buffer에 저장한 내용을 클라이언트로 넘겨줌.
                        }


                        response.OutputStream.Close();

                    }

                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString); //buffer에 byte배열로 저장.
                response.OutputStream.Write(buffer, 0, buffer.Length); //buffer에 저장한 내용을 클라이언트로 넘겨줌.

                response.OutputStream.Close();

            }
            listener.Stop();
        }


    }
}