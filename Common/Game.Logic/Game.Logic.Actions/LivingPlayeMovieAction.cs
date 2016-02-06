using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingPlayeMovieAction : BaseAction
	{
		private Living m_living;
		private string m_action;
		private LivingCallBack m_callBack;
		private int m_movieTime;
		public LivingPlayeMovieAction(Living living, string action, int delay, int movieTime, LivingCallBack callBack) : base(delay, movieTime)
		{
			this.m_living = living;
			this.m_action = action;
			this.m_callBack = callBack;
			this.m_movieTime = movieTime;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendLivingPlayMovie(this.m_living, this.m_action);
			if (this.m_callBack != null)
			{
				this.m_living.CallFuction(this.m_callBack, this.m_movieTime);
			}
			base.Finish(tick);
		}
	}
}
