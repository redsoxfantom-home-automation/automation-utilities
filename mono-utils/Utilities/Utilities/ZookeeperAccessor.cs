using System;
using org.apache.zookeeper;
using NLog;
using System.Threading.Tasks;
using System.Threading;

namespace Utilities
{
	public class ZookeeperAccessor : Watcher
	{
		private ZooKeeper instance;
		private bool isConnected;
		private string connectionString;
		private static Logger logger = LogManager.GetCurrentClassLogger();
		ManualResetEventSlim resetEvent = new ManualResetEventSlim (false);

		public ZookeeperAccessor (string zookeeperHost, int zookeeperPort)
		{
			connectionString = String.Format ("{0}:{1}",zookeeperHost,zookeeperPort);
			AttemptConnection ();
		}

		public void RegisterService(string serviceName, string serviceHost, int servicePort)
		{
			
		}

		private void AttemptConnection()
		{
			if(!isConnected)
			{
				logger.Info ("Attempting to connect to zookeeper instance at {0}", connectionString);
				instance = new ZooKeeper (connectionString,5000,this);
				resetEvent.Wait ();
			}
		}

		public override Task process(WatchedEvent evt)
		{
			switch(evt.getState())
			{
				case Event.KeeperState.SyncConnected:
					logger.Info ("Connection achieved");
					isConnected = true;
					resetEvent.Set ();
					return Task.FromResult (false);
				case Event.KeeperState.Disconnected:
				case Event.KeeperState.Expired:
					logger.Error ("Connection to zookeeper lost. Will attempt reconnection");
					isConnected = false;
					return Task.Factory.StartNew (() =>
					{
						AttemptReconnection ();
					});
				default:
					return Task.FromResult (false);
			}
		}

		void AttemptReconnection ()
		{
			int attemptNumber = 1;
			while (!isConnected)
			{
				logger.Info ("Attempting reconnection (attempt number: {0})", attemptNumber);
				AttemptConnection ();
				attemptNumber++;
				Thread.Sleep (5000);
			}
		}
	}
}

