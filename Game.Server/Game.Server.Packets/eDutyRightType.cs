using System;
namespace Game.Server.Packets
{
	public enum eDutyRightType
	{
		_1_Ratify = 1,
		_2_Invite,
		_3_BanChat = 4,
		_4_Notice = 8,
		_5_Enounce = 16,
		_6_Expel = 32,
		_7_Diplomatism = 64,
		_8_Manage = 128,
		_9_ConsortiaUp = 256,
		_10_ChangeMan = 512,
		_11_Disband = 1024,
		_12_UpGrade = 2048,
		_13_Exit = 4096
	}
}
