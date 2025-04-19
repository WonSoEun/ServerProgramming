#include <iostream>
#include <thread>
#include <mutex>
#include <string>
#include <chrono>

//���ٴٵ� ���̺귯��

using namespace std;

string me = "�ȳ�!!";
mutex me_maum; // mutex ���� : ���� �������� �� �ٸ��ֵ� ���� ���ϰ� �ϴ°�
// �������� �����������.

void Propose(string my_name, string his_name)
{
	cout << "\n\n";
	cout << my_name << ": " << his_name << "�����~" << endl;
	cout << my_name << ": " << his_name << "�� �� ����!!" << endl;
	this_thread::sleep_for(std::chrono::milliseconds(1000));
}

void GetLove(string name)
{
	me_maum.lock(); //mutex ������,
	me = name;
	Propose("�� : ", me);
	this_thread::sleep_for(std::chrono::milliseconds(5000));
	cout << "\n";
	cout << "�� : " << name << "�� �� ���� �Ⱦ�";
	me_maum.unlock();  //mutex Ǯ����. �׷��⿡ �ٸ� �����带 ������ �� �ִ� ��.
}


void main() //������ 0
{
	thread A_LOVE(GetLove, "A�� "); //������ 1
	thread B_LOVE(GetLove, "B�� "); //������ 2

	A_LOVE.join(); //�����尡 ���������� ��ٷ���. 
	B_LOVE.join();
}