using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(4)]
	public class InviteCommand : ISpaCommandHandler
	{
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom == null)
			{
				result = false;
			}
			else
			{
				if (player.CurrentSpaRoom.Spa_Room_Info.PlayerID != player.PlayerCharacter.ID)
				{
					result = false;
				}
				else
				{
					if (player.CurrentSpaRoom.Count >= player.CurrentSpaRoom.Spa_Room_Info.MaxCount)
					{
						result = false;
					}
					else
					{
						GSPacketIn pkg = packet.Clone();
						pkg.ClearContext();
						int id = packet.ReadInt();
						GamePlayer invitePlayer = WorldMgr.GetPlayerById(id);
						if (invitePlayer != null && invitePlayer.CurrentRoom == null && invitePlayer.CurrentMarryRoom == null && invitePlayer.CurrentSpaRoom == null)
						{
							pkg.WriteByte(4);
							pkg.WriteInt(player.PlayerCharacter.ID);
							pkg.WriteString(player.PlayerCharacter.NickName);
							pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.RoomID);
							pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.RoomName);
							pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.Pwd);
							invitePlayer.Out.SendTCP(pkg);
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}
	}
}
