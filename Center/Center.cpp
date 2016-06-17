#include "stdafx.h"
#include <string>
#include <Windows.h>

using namespace System;
using namespace Lsj::Util;
using std::string;
using namespace Center::Server;
using namespace Game::Base;




void Start(String^ a);
bool WINAPI ConsoleHandler(DWORD CEvent);
bool WINAPI StopServer();





int main(array<System::String^> ^args)
{
	if (args->Length>=1)
	{
		String^ a = args[0];
		if (a != "")
		{
			Start(a);
		}
		else
		{
			WinForm::Notice("No Key!");
		}
	}
	else
	{
		WinForm::Notice("Please run with the Launcher!");
	}
}

void Start(String^ a)
{
	Console::Title = a;
	Console::WriteLine("Starting the Server ... Please Wait!");
	CenterServer::CreateInstance(gcnew CenterServerConfig());
	CenterServer::Instance->Start();
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
bool WINAPI StopServer()
{
	if (CenterServer::Instance)
	{
		CenterServer::Instance->Stop();
	}
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)ConsoleHandler, false);
	return TRUE;
}