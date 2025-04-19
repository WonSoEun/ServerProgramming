using System;
using System.Diagnostics;

class Program
{
    public delegate void Ass();

    static void Main(string[] args)
    {
        //yield return  - 기준점 
        //mainCoroutine.MoveNext(); - 다음 기준점까지의 코드를 이행함.
        Ass isSame = null;
        isSame = EndMsg;

        bool isChoice = true; //주의 옷고르기

        Stopwatch sw = new Stopwatch();

        string mt = "주 : 나 옷 입는다";
        string st = "";

        IEnumerator<string> es = Su();

        while (true)
        {
            sw.Reset();
            sw.Start();

            es.MoveNext();
            Console.WriteLine(mt);
            st = es.Current;
            Console.WriteLine(st);
            Console.WriteLine();
            int ju_a = 0;

            while (true)
            {
                es.MoveNext();
                sw.Stop();
                st = es.Current;

                if (sw.ElapsedMilliseconds>3000)
                {
                    if (isChoice)
                    {
                        Random ran = new Random();
                        ju_a = ran.Next(1, 6);
                        isChoice=false;
                    }
                    mt = "주 : 나 "+ju_a+"번 옷 입었어";
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                    Console.WriteLine(mt);

                    if (st=="end")
                    {
                        es.MoveNext();
                        st= es.Current;
                        Console.WriteLine("수 : 나 "+st+" 옷 입었어");
                        break;
                    }
                }
                sw.Start();
            }


            if (st==ju_a+"번")
            {
                Console.Write("같은 옷 입었넹~^^ ");
                isSame();
                break;
            }
            else
            {
                Console.WriteLine("다른 옷 입었네... 다시 입자...");
                Console.WriteLine();
                isChoice=true;
            }
            mt = null;
        }

    }

    public static IEnumerator<string> Su()
    {
        Stopwatch sw = new Stopwatch();
        string talk = "수 : 나 옷 입는다";

        while (true)
        {
            Random ran = new Random();
            int su_a = ran.Next(1, 6);

            sw.Reset();
            sw.Start();
            yield return talk;

            while (true)
            {
                sw.Stop();
                if (sw.ElapsedMilliseconds>5000)
                {
                    break;
                }
                sw.Start();
                yield return talk;

            }
            talk = "end";
            yield return talk;
            talk = su_a+"번";
            yield return talk;
            talk = null;

        }
    }

    static void EndMsg()
    {
        Console.WriteLine("아 그럼 출발해야지~");
    }
}

