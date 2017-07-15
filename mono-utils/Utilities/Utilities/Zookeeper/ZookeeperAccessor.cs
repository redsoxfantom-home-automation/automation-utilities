using System;
using NLog;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using ZooKeeperNet;

namespace Utilities.Zookeeper
{
	public class ZookeeperAccessor : IWatcher
	{
		private ZooKeeper instance;
		private bool isConnected;
		private string connectionString;
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private string host;
		ManualResetEventSlim resetEvent = new ManualResetEventSlim (false);

		public ZookeeperAccessor (string zookeeperHost, int zookeeperPort)
		{
			host = System.Net.Dns.GetHostName ();
			connectionString = String.Format ("{0}:{1}",zookeeperHost,zookeeperPort);
			AttemptConnection ();
		}

		public void RegisterService(string apiLevel, string serviceName, int port)
		{
			if(!isConnected)
			{
				throw new ApplicationException ("Attempted to register service when not connected to zookeeper");
			}

			string rootPath = String.Format ("/services/{0}/{1}", apiLevel, serviceName);
			string hostNamePath = String.Format ("{0}/host", rootPath);
			string portPath = String.Format ("{0}/port", rootPath);

			logger.Info (String.Format ("Registering {0}:{1} to {2}",host,port,rootPath));
			CreateNode ("/services", new byte[0], CreateMode.Persistent);
			CreateNode ("/services/"+apiLevel, new byte[0], CreateMode.Persistent);
			CreateNode (rootPath, new byte[0], CreateMode.Persistent);
			CreateNode (hostNamePath, Encoding.ASCII.GetBytes (host),CreateMode.Ephemeral);
			CreateNode (portPath, Encoding.ASCII.GetBytes (port.ToString()),CreateMode.Ephemeral);
		}

		private void CreateNode(string path,byte[] data, CreateMode mode)
		{
			if (instance.Exists (path, false) == null)
			{
				instance.Create (path, data, Ids.OPEN_ACL_UNSAFE, mode);
			}
		}

		private void AttemptConnection()
		{
			if(!isConnected)
			{
				logger.Info ("Attempting to connect to zookeeper instance at {0}", connectionString);
				instance = new ZooKeeper (connectionString,new TimeSpan(0,0,5),this);
				resetEvent.Wait ();
			}
		}

		public void Process(WatchedEvent evt)
		{
			switch(evt.State)
			{
				case KeeperState.SyncConnected:
					ProcessConnectedClient (evt);
					return;
				case KeeperState.Disconnected:
				case KeeperState.Expired:
					logger.Error ("Connection to zookeeper lost. Will attempt reconnection");
					isConnected = false;
					Task.Factory.StartNew (() =>
					{
						AttemptReconnection ();
					});
					return;
			}
		}

		void ProcessConnectedClient(WatchedEvent evt)
		{
			if (evt.Type == EventType.None)
			{
				// This event was just to let us know that we connected successfully
				logger.Info ("Connection achieved");
				isConnected = true;
				resetEvent.Set ();
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

