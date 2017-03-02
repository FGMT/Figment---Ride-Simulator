using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {
	
	public enum NetworkingType {None, PointToPointUDP, MulticastUDP, UnityNative};	
	
	public InputManager inputManager;
	public NetworkManager networkManager;
	public UnityNetworking unityNetworkManager;
	public NetworkingType networkingType;

	public bool showDebugOverlay = false;
	private List<string> debugMessages = new List<string>();
	public Rect debugGuiWindow = new Rect((Screen.width - 300),(10), 300,80);


	public Animation bikeAnimation;


	public float totalCycleCount = 0.0f;

	public float timeBeforeDecelerated = 2.0f;
	public float decelerationRate = 0.1f;

	public bool interpolatingSpeed = false;
	public int accelerationFixedFrameTicks = 8;
	public int currentInterpolationTick = 0;
	public float currentInterpolationPoint = 0;
	public float previousSetSpeed = 0.0f;
	public float currentSpeed = 0.0f;
	public float destinationSpeed = 0.0f;

	public float currentSpeedMultiplier = 1.0f;

	public float previousPedalRevTime = 0.0f;
	public float currentPedalRevsPerMinute = 0.0f;

	public bool reEvaluateBikeSpeed = true;

	


	// Use this for initialization
	void Start () {

		debugMessages.Add("Starting Ride Simulator");

		// Is this computer supposed to be a server?
		if (unityNetworkManager.isServer == true) {
			if (networkingType != NetworkingType.None) {
				Debug.Log("Starting UDP network manager for bluetooth signals via node-red");
				networkManager.ManualStart();
			}
		}		
	}
	

	// Update is called once per frame
	void FixedUpdate () {

		// Set the bike speed
		SetBikeSpeed();


		// Update the input manager and network manager
		inputManager.ManualUpdate();
		
		if (networkingType != NetworkingType.None) {
			networkManager.ManualUpdate();
		}	
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log("Manual Cycle Increment 1");
			CalculatePedalSpeed();
			IncrementCycleCount(1);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log("Quitting");
			Application.Quit();
		}
	}

	public void NetworkMessageReceived(NetworkManager.NetworkMessage networkMessage) {
		Debug.Log("Valid message received from " + networkMessage.messageSender + "," + networkMessage.messageTitle + "," + networkMessage.message);
		ProcessNetworkMessage(networkMessage);
	}

	public void ProcessNetworkMessage(NetworkManager.NetworkMessage networkMessage) {
		if (networkMessage.messageTitle == "CycleCount") {

			CalculatePedalSpeed();
			IncrementCycleCount(1);
		}
	}


	public void SetBikeSpeed() {
		CaclculateBikeSpeed();
		bikeAnimation["Take 001"].speed = currentSpeed;
    }

	public void IncrementCycleCount(int revsToAdd) {
		totalCycleCount += revsToAdd;
	}

	public void CalculatePedalSpeed() {
		currentPedalRevsPerMinute = 60 / (Time.time - previousPedalRevTime);
		previousPedalRevTime = Time.time;
		SetSpeedInterpolationValues();
	}

	public void SetSpeedInterpolationValues() {

		float newSpeed = currentPedalRevsPerMinute / 100 * currentSpeedMultiplier;

		if (newSpeed > currentSpeed) {

			destinationSpeed = currentPedalRevsPerMinute / 100 * currentSpeedMultiplier;
			previousSetSpeed = currentSpeed;
			interpolatingSpeed = true;
			currentInterpolationTick = 0;
        }

	}



	public void CaclculateBikeSpeed() {


		if (Time.time <= (previousPedalRevTime + timeBeforeDecelerated)) {
			if (interpolatingSpeed == true) {
				currentInterpolationTick++;

				currentInterpolationPoint = ((float)currentInterpolationTick / (float)accelerationFixedFrameTicks);
				currentSpeed = Mathf.Lerp(previousSetSpeed, destinationSpeed, currentInterpolationPoint);

				if (accelerationFixedFrameTicks <= currentInterpolationTick) {
					interpolatingSpeed = false;
					currentInterpolationTick = 0;
				}
			}
		}
		else {
			interpolatingSpeed = false;
			currentSpeed = currentSpeed - decelerationRate;

			if (currentSpeed < 0) {
				currentSpeed = 0;
			}
		}

	}

	void OnGUI() {
		if (showDebugOverlay == true) {
			debugGuiWindow = GUILayout.Window(0, debugGuiWindow, GameManagerDebugMenu, "Game Manager Debugging"); 
		}
		
	}

	public void AddDebugMessageToQueue(string messageToAdd) {
		debugMessages.Insert(0,messageToAdd);
		if (debugMessages.Count >= 5) {
			debugMessages.RemoveAt(4);
		}

	}
	
	void GameManagerDebugMenu(int windowID) {
		for (int i = 0; i < debugMessages.Count; i++) {
			GUILayout.Label(debugMessages[i]);	
		}

	}


	
}
