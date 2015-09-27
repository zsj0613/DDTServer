#include "stdafx.h"
#include <string>
#include <Windows.h>


using namespace Web::Server;
using namespace Lsj::Util;
using std::string;
using namespace Game::Base;
using System::Console;
using System::String;
using System::IntPtr;




void MarshalString(System::String ^ s, string& os);
void Start();
bool WINAPI ConsoleHandler(DWORD CEvent);
int StopServer();





int main(array<String^> ^args)
{
	if (args->Length>=1)
	{
		string key = "$^&^(*&)*(J1534765";
		string a = "";
		MarshalString(args[0], a);
		if (a == key)
		{
			Start();
		}
		else
		{
			WinForm::Notice("Error Key!");
		}
	}
	else
	{
		WinForm::Notice("Please run with the Launcher!");
	}
}

void Start()
{
	Console::Title = "DDTank Web Service";
	Console::WriteLine("Starting the Server ... Please Wait!");
	WebServer::CreateInstance();
	WebServer::Instance->Start();
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)ConsoleHandler, true);
	while (true)
	{
		System::Threading::Thread::Sleep(1);
	}
}

bool WINAPI ConsoleHandler(DWORD CEvent)
{
	switch (CEvent)
	{
	case CTRL_C_EVENT:
	case CTRL_BREAK_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_LOGOFF_EVENT:
	case CTRL_SHUTDOWN_EVENT:
		return StopServer();
	default:
		return FALSE;
	}
}
int StopServer()
{
	if (WebServer::Instance)
	{
		WebServer::Instance->Stop();
	}
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)ConsoleHandler, false);
	return TRUE;
}

void MarshalString(String ^ s, string& os) {
	const char* chars =
		(const char*)(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	System::Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr((void*)chars));
}
