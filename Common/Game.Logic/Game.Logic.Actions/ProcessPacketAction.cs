using Game.Base.Packets;
using Game.Logic.Cmd;
using Game.Logic.Phy.Object;
using Lsj.Util.Logs;
using System;
using System.Reflection;
namespace Game.Logic.Actions
{
	public class ProcessPacketAction : IAction
	{
		private static LogProvider log => LogProvider.Default;
		private Player m_player;
		private GSPacketIn m_packet;
		public ProcessPacketAction(Player player, GSPacketIn pkg)
		{
			this.m_player = player;
			this.m_packet = pkg;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_player.IsActive)
			{
                byte a = this.m_packet.ReadByte();
                eTankCmdType type = (eTankCmdType)a;
				try
				{
					ICommandHandler handleCommand = CommandMgr.LoadCommandHandler((int)type);
					if (handleCommand != null)
					{
						handleCommand.HandleCommand(game, this.m_player, this.m_packet);
					}
					else
					{
						ProcessPacketAction.log.Error(string.Format("Player Id: {0}", this.m_player.Id));
					}
				}
				catch (Exception ex)
				{
					ProcessPacketAction.log.Error(string.Format("Player Id: {0}  cmd:0x{1:X2}", this.m_player.Id, a), ex);
				}
			}
		}
		public bool IsFinished(BaseGame game, long tick)
		{
			return true;
		}
	}
}
