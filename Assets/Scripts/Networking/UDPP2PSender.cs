using UnityEngine;
using System.Collections;
 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
 
public class UDPP2PSender : MonoBehaviour
{
    private static int localPort;
//    public string ipAddress = "127.0.0.1";  // define in init
    private string ipAddress = "127.0.0.1";	
    private int port = 80510;  // define in init

	private IPEndPoint remoteEndPoint;
    private UdpClient udpClient;
	//private Socket sock ;	
	
    // start from unity3d
    public void ManualStart(string initIPAddress = "127.0.0.1", int initPort = 8051) {
		ipAddress = initIPAddress;
		port = initPort;			
        init(); 
    }	

	// initiate udp sending.
    private void init() {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);			
		udpClient = new UdpClient();
		//sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);		

    }		
	
	// public function to send message to specific ip
	public void sendString(string message) {		
		byte[] data = Encoding.UTF8.GetBytes(message);
		udpClient.Send(data, data.Length, remoteEndPoint);	
		//sock.SendTo(data, remoteEndPoint);		
    }	
}
