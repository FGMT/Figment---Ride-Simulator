using UnityEngine;
using System.Collections;

public class UnityNetworking : MonoBehaviour {

	public GameManager gameManager;

	public string isServerFilePath = "/Resources/isServer.active";

	public bool isServer = false;
	public bool isClient = false;
	public bool showDebugOverlay = false;
	public Rect debugGuiWindow = new Rect(0,(Screen.height - 40), 500,40);
	public GameObject networkErrorUI;


	public bool useMasterServer = false;
	public string serverIpAddress = "10.1.1.243";
	public string secondaryServerIpAddress = "10.1.1.43";
	public int serverPort = 25000;


	public float connectionAttemptInterval = 10.0f;
	private float connectionAttemptTimer = 0.0f;

	private bool isServierInitialized = false;
	private NetworkConnectionError connectReturn = NetworkConnectionError.ConnectionFailed;


	void Awake() {
/*
		// if isServerFilePath file exists - make this a server
		if (System.IO.File.Exists(Application.dataPath + isServerFilePath)) {
			Debug.Log(Application.dataPath + isServerFilePath + " file found");
			isServer = true;
//			gameManager.ipadUI.SetActive(true);
//			gameManager.tvUI.SetActive(true);
		} else {
			Debug.Log(Application.dataPath + isServerFilePath + " file not found");
			isClient = true;
//			gameManager.ipadUI.SetActive(true);
//			gameManager.tvUI.SetActive(false);
		}

*/

		StartNetworkServer();


	}

	// Use this for initialization
	void Start () {
		connectionAttemptTimer = connectionAttemptInterval;			
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Keypad0)) {
//			StartNetworkServer();
//		}
//		if (Input.GetKeyDown (KeyCode.Keypad1)) {
//			ConnectToNetworkServer();
//		}

		if (isClient == true) {
			if (Network.connections.Length == 0) {
				connectionAttemptTimer += Time.deltaTime;

				if (connectionAttemptTimer >= connectionAttemptInterval) {


					if (useMasterServer == true) {
						Debug.Log("Scanning for network games: " + MasterServer.PollHostList().Length + " games found");
						HostData[] hostData = MasterServer.PollHostList();

						if (hostData.Length != 0) {
							string tmpIp = "";
							int i = 0;
							while (i < hostData[0].ip.Length) {
								tmpIp = hostData[0].ip[i] + " ";
								i++;
							}


							Debug.Log("Network Game Found: " + hostData[0].gameType + ":" + hostData[0].gameName + ", " + tmpIp + ". " + hostData[0].port);

							// Connect to game!
							connectReturn = Network.Connect(hostData[0]);

							Debug.Log("Connection attempt returned: " + connectReturn + " Network connection count = " + Network.connections.Length);


						}
					} else {
						Debug.Log("Trying to connect to " + serverIpAddress + " on port " + serverPort);
						connectReturn = Network.Connect(serverIpAddress, serverPort);
						if ((connectReturn != NetworkConnectionError.NoError) && (Network.connections.Length == 0)) {
							Debug.Log("Primary server connection failed, trying to connect to " + secondaryServerIpAddress + " on port " + serverPort);
							connectReturn = Network.Connect(secondaryServerIpAddress, serverPort);
							if (connectReturn != NetworkConnectionError.NoError) {
								Debug.Log("Connection successfull, network connection count = " + Network.connections.Length);
							}
						} else {
							Debug.Log("Connection successfull, network connection count = " + Network.connections.Length);
						}

					}
					if ((connectReturn != NetworkConnectionError.NoError)&&(useMasterServer == true)) {
						Debug.Log("trying to connect to network again");
						ConnectToNetworkServer();
					}
					connectionAttemptTimer = 0.0f;
				}


			}
		}
	}

	public void StartNetworkServer() {
		Debug.Log("Starting Network Server");
		Debug.Log("IP = " + Network.player.ipAddress + ", port = " + Network.player.port);
		Network.InitializeServer(5, serverPort, false);
		if (useMasterServer == true) {
			MasterServer.RegisterHost("La Trobe - Ride for coffee", "default game", "Auto join this!");
		}
	}

	void OnServerInitialized() {
		Debug.Log("Server initialized and ready");
		isServierInitialized = true;
//		gameManager.databaseManager.LoadDatabaseFromFile();

	}

	public void ConnectToNetworkServer () {
		Debug.Log("Connecting To Network Server");
//		isClient = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("La Trobe - Ride for coffee");

	}

	void OnGUI() {

		string ipAddress = Network.player.ipAddress;
		if (Network.connections.Length > 0) {

			GUI.contentColor = Color.black;
			if (isServer == true) {

				if (useMasterServer == true) {
					ipAddress = MasterServer.ipAddress;
				}
//				GUI.Label(new Rect(Screen.width - 120, Screen.height - 25, 200, 25),("S - " + ipAddress));	
			} else {
				networkErrorUI.SetActive(false);
//				GUI.Label(new Rect(Screen.width - 650, 0, 350, 40),("Ride for Coffee Connected - " + Network.connections[0].ipAddress), gameManager.guiManager.style);
			}
		} else {
			GUI.contentColor = Color.black;
			if (isServer == true) {
//				GUI.Label(new Rect(Screen.width - 120, Screen.height - 25, 200, 25),("S - " + ipAddress));
			} else {
//				GUI.Label(new Rect(Screen.width - 650, 0, 350, 40),("NOT CONNECTED - Ride for Coffee"), gameManager.guiManager.style);
				networkErrorUI.SetActive(true);
			}
		}
		GUI.contentColor = Color.black;


//		if (showDebugOverlay == true) {
//			debugGuiWindow = GUILayout.Window(0, debugGuiWindow, NetworkDebugMenu, "Network Debugging"); 
//		}

	}

	public void NetworkDebugMenu(int windowID) {

		if (Network.connections.Length > 0) {
			if (isServer == true) {
				GUILayout.Label("Local Server Active: network connections");	
				for (int i = 0; i < Network.connections.Length; i++) {
					GUILayout.Label(Network.connections[i].ipAddress + ":" + Network.connections[i].port);
				}
//				GUILayout.Label("Database location = " + Application.dataPath + gameManager.databaseManager.dbFilePath);
			} else {
				GUILayout.Label("Connected To Server: " + Network.connections[0].ipAddress + ":" + Network.connections[0].port);	
			}

		} else {
			if (isServer == true) {
				if (isServierInitialized == false) {
					GUILayout.Label("Local Server Initializing");	
				} else {
					GUILayout.Label("Local Server Active: No network connections");				
				}
			} else {
				GUILayout.Label("Trying To Connect To Server - is client = " + isClient + " connection timer = " + connectionAttemptTimer);	
			}
		}
	}

	void OnConnectedToServer() {
		Debug.Log("Connected To Server!");
	}

	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("New player connected - " + player.ipAddress + ":" + player.port);
	}


}
