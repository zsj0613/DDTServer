using System;
namespace Game.Logic.Actions
{
	public class PlayBackgroundSoundAction : BaseAction
	{
		private bool m_isPlay;
		public PlayBackgroundSoundAction(bool isPlay, int delay) : base(delay, 1000)
		{
			this.m_isPlay = isPlay;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			((PVEGame)game).SendPlayBackgroundSound(this.m_isPlay);
			base.Finish(tick);
		}
	}
}
