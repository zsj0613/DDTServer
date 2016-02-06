using System;
namespace Game.Logic
{
	public enum eGameState
	{
		Inited,
		Prepared,
		Loading,
		GameStart,
		PreparePlaying,
		Playing,
		PrepareGameOver,
		GameOver,
		Stopped,
		SessionPrepared,
		ALLSessionStopped
	}
}
