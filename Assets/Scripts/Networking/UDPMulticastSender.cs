using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPMulticastSender : MonoBehaviour {

	private int port = 8052;
	private string multicastIP = "239.0.0.223";
//	private string multicastIP = "10.1.1.230";

	private IPEndPoint remoteEndPoint;
	// private UdpClient udpclient;	
	private Socket sock ;		
	
	// start from unity3d when instantiated.
    public void ManualStart(string initMulticastingAddress = "239.0.0.222", int initPort = 8051) {
//	public void ManualStart(string initMulticastingAddress = "10.1.1.230", int initPort = 8051) {
		multicastIP = initMulticastingAddress;
		port = initPort;		
        init(); 
    }

	// initiate udp multicast sending.
    public void init() {	
		
		IPAddress multicastaddress = IPAddress.Parse(multicastIP);
        remoteEndPoint = new IPEndPoint(multicastaddress, port); 	
		
		// You can do networking in two ways... using a udpclient or sockets
        //udpclient = new UdpClient();
        //udpclient.JoinMulticastGroup(multicastaddress);		
		
		sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastaddress));	
		sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
		sock.Connect(remoteEndPoint);
		
    }	
	
	// public function to send string once multicasting is initiated.
	public void sendString(string message) {		
		byte[] data = Encoding.UTF8.GetBytes(message);
        //udpclient.Send(data, data.Length, remoteEndPoint);
		sock.Send(data,data.Length,SocketFlags.None);
		

    }
	
	public void closePort() {
		sock.Close();
	}

}