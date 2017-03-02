using UnityEngine;
using System.Collections;
 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
 
public class UDPListener_old : MonoBehaviour {
	
    Thread receiveThread;
    UdpClient client; 
	public bool enableListener = true;
	
	public int port = 5081;
	public string lastReceivedUDPPacket="";
	public string allReceivedUDPPackets=""; // clean up this from time to time!	
	
	public void Start() {
		init(); 
	}
	
	private void init() {

        port = 8051;

		print("Sending to 127.0.0.1 : "+port);
		print("Test-Sending to this Port: nc -u 127.0.0.1  "+port+"");

		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
    }
	

    // receive thread

    private void ReceiveData() {

		client = new UdpClient(port);

        while (enableListener == true) {
			Thread.Sleep(300); 
			try {
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
			
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.UTF8.GetString(data);
				print(">> " + text);
							
				// latest UDPpacket				
				lastReceivedUDPPacket=text;
				allReceivedUDPPackets=allReceivedUDPPackets+text;
	
			} catch (Exception err) {
				print(err.ToString());
			}
		}
	}

    // getLatestUDPPacket
    // cleans up the rest
	public string getLatestUDPPacket() {
		allReceivedUDPPackets="";
		return lastReceivedUDPPacket;	
	}
	
	 public void OnApplicationQuit() {
		print("Stop");
		// end of application
		if (receiveThread != null) {
			receiveThread.Abort();
			receiveThread.Join();
		}
		 

		 
	}	
}