using System;
namespace Game.Base.Packets
{
	public interface IStreamProcessor
	{
		void SetFsm(int adder, int muliter);
		void SendTCP(GSPacketIn pkg);
		void ReceiveBytes(int numBytes);
		void Dispose();
	}
}
