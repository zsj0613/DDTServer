using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Buffer
{
	public class GPMultipleBuffer : AbstractBuffer
	{
		public GPMultipleBuffer(BufferInfo info) : base(info)
		{
		}
		public override void Start(GamePlayer player)
		{
			GPMultipleBuffer buffer = player.BufferList.GetOfType(typeof(GPMultipleBuffer)) as GPMultipleBuffer;
			if (buffer != null)
			{
				buffer.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(buffer);
			}
			else
			{
				base.Start(player);
				player.GPAddPlus *= (double)base.Info.Value;
			}
		}
		public override void Stop()
		{
			if (this.m_player != null)
			{
				this.m_player.GPAddPlus /= (double)base.Info.Value;
				base.Stop();
			}
		}
	}
}
