using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameManager gameManager;	
	public int udpIdentifier; 	
	public string udpSendString;
	public string p2pIP = "10.1.1.76";	
	public string multicastIP = "239.0.0.222";
	public int port = 8051;
	private string[] newMessageStrAr;
	//private volatile NetworkMessage[] newNetworkMessages;
	private volatile List <NetworkMessage> newNetworkMessages = new List<NetworkMessage>();
	public bool netListLock; 
	
	// UDP Listener
	private UDPListener udpListener;	
	// UDP Peer to peer networking classes
	private UDPP2PSender udpP2PSender;	
	// UDP Multicast Sender
	private UDPMulticastSender udpMulticastSender;	
	

	public class NetworkMessage {

		public string messageDate;
		public string messageTime;
		public string messageSender;
		public string messageTitle;
		public string message;

		public void SetNetworkMessage(string date, string time, string sender, string messageFromBeanTitle, string messageFromBean) {
			messageDate = date;
			messageTime = time;
			messageSender = sender;
			messageTitle = messageFromBeanTitle;
			message = messageFromBean;

		}
		public bool SetNetworkMessageFromCSV(string networkData) {

			string[] newMessageStrAr = networkData.Split(';');

			if (newMessageStrAr.Length == 5) {
//				Debug.LogWarning("UDP Message Received = " + networkData);
				SetNetworkMessage(newMessageStrAr[0],newMessageStrAr[1],newMessageStrAr[2],newMessageStrAr[3],newMessageStrAr[4]);		
				return true;
			} else {
				Debug.LogWarning("Invalid UDP Message Received = " + networkData + " - MESSAGE NOT FORMATTED PROPERLY");
				return false;
			}
		}		
    } 
	
	
	// Use this for initialization
	public void ManualStart () {
		udpIdentifier = Random.Range(0, 1000);

		if (gameManager.networkingType == GameManager.NetworkingType.PointToPointUDP) {
			udpListener = gameObject.AddComponent<UDPListener>();
			udpListener.ManualStart(multicastIP,port);
			udpP2PSender = gameObject.AddComponent<UDPP2PSender>();
			udpP2PSender.ManualStart(p2pIP,port);			
		} else if (gameManager.networkingType == GameManager.NetworkingType.MulticastUDP) {
			udpListener = gameObject.AddComponent<UDPListener>();	
			udpListener.ManualStart(multicastIP,port);			
			udpMulticastSender = gameObject.AddComponent<UDPMulticastSender>(); 
			udpMulticastSender.ManualStart(multicastIP,port);
		}
	}
	
	// Update is called once per frame
	public void ManualUpdate () {
		if (gameManager.inputManager.mouseDownValid3dLocation == true) {
			
			udpSendString = (udpIdentifier + "," + gameManager.inputManager.hitPoint3d.x  + "," + gameManager.inputManager.hitPoint3d.y + "," + gameManager.inputManager.hitPoint3d.z);			
			
			if (gameManager.networkingType == GameManager.NetworkingType.PointToPointUDP) {			
				udpP2PSender.sendString(udpSendString);
			} else if (gameManager.networkingType == GameManager.NetworkingType.MulticastUDP) {	
				udpMulticastSender.sendString(udpSendString);
			}
//				Debug.Log (udpSendString);
		}
		UpdateGameWithNetworkMessages();
	
	}
	
	public void UpdateGameWithNetworkMessages() {
		netListLock = true;
		lock(newNetworkMessages)
		foreach (NetworkMessage newMessage in newNetworkMessages) {
			gameManager.NetworkMessageReceived(newMessage);
		}
		newNetworkMessages.Clear();
		netListLock = false;		
	}
	

	
	public void addNewNetworkMessage(string networkData) {
		NetworkMessage newMessage = new NetworkMessage();
		if (newMessage.SetNetworkMessageFromCSV(networkData) == true) {
			newNetworkMessages.Add(newMessage);		
		} else {
			newMessage = null;
		}
	}	
}
