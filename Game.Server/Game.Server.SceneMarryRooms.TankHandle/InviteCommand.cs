using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(4)]
	public class InviteCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
			{
				result = false;
			}
			else
			{
				if (!player.CurrentMarryRoom.Info.GuestInvite)
				{
					if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
					{
						result = false;
						return result;
					}
				}
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				int id = packet.ReadInt();
				GamePlayer invitedplayer = WorldMgr.GetPlayerById(id);
				if (invitedplayer != null && invitedplayer.CurrentRoom == null && invitedplayer.CurrentMarryRoom == null && invitedplayer.CurrentSpaRoom == null)
				{
					pkg.WriteByte(4);
					pkg.WriteInt(player.PlayerCharacter.ID);
					pkg.WriteString(player.PlayerCharacter.NickName);
					pkg.WriteInt(player.CurrentMarryRoom.Info.ID);
					pkg.WriteString(player.CurrentMarryRoom.Info.Name);
					pkg.WriteString(player.CurrentMarryRoom.Info.Pwd);
					pkg.WriteInt(player.MarryMap);
					invitedplayer.Out.SendTCP(pkg);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}
}
