using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPListener : MonoBehaviour {

	public NetworkManager networkManager;
	private int port = 5081;
	private UdpClient client;
	private IPEndPoint RemoteIpEndPoint;
	private string multicastIP = "239.0.0.222";
	public string lastReceivedUDPPacket;
	
	static int UDPValue;
	private Thread t_udp;
	
	public void ManualStart(string inputMulticastIP = "239.0.0.222", int inputPort = 5081)
	{
		networkManager = gameObject.GetComponent<NetworkManager>();
		
		port = inputPort;
		multicastIP = inputMulticastIP;

		client = new UdpClient(port);
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		client.JoinMulticastGroup(IPAddress.Parse(multicastIP));			
		t_udp = new Thread(new ThreadStart(UDPRead));		 
		t_udp.Name = "UDP thread";
		t_udp.Start();
	}

	public void UDPRead()
	{
		while (true)
		{
			try
			{
				byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
				string returnData = Encoding.ASCII.GetString(receiveBytes);

				while (networkManager.netListLock == true) {
					Thread.Sleep(20);
				}
				networkManager.addNewNetworkMessage(returnData);
		    }
		    catch (Exception e)
			{
			    Debug.Log("Not so good " + e.ToString());
			}
		    Thread.Sleep(20);
		}
	}
		
	void OnDisable()
	{
		if (t_udp != null)  t_udp.Abort();
		if (client != null) client.Close();
	}
}

