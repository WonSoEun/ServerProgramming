#include <iostream>
#include <thread>
#include <mutex>
#include <string>
#include <chrono>

//스텐다드 라이브러리

using namespace std;

string me = "안녕!!";
mutex me_maum; // mutex 변수 : 내가 접근했을 때 다른애들 접근 못하게 하는것
// 누군가가 해제해줘야함.

void Propose(string my_name, string his_name)
{
	cout << "\n\n";
	cout << my_name << ": " << his_name << "사랑해~" << endl;
	cout << my_name << ": " << his_name << "나 너 좋아!!" << endl;
	this_thread::sleep_for(std::chrono::milliseconds(1000));
}

void GetLove(string name)
{
	me_maum.lock(); //mutex 막아줌,
	me = name;
	Propose("쏘 : ", me);
	this_thread::sleep_for(std::chrono::milliseconds(5000));
	cout << "\n";
	cout << "쏘 : " << name << "나 너 이제 싫엉";
	me_maum.unlock();  //mutex 풀어줌. 그렇기에 다른 쓰래드를 가져올 수 있는 거.
}


void main() //쓰레드 0
{
	thread A_LOVE(GetLove, "A야 "); //쓰레드 1
	thread B_LOVE(GetLove, "B야 "); //쓰레드 2

	A_LOVE.join(); //쓰레드가 끝날때까지 기다려라. 
	B_LOVE.join();
}