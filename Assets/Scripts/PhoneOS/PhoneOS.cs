using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages PhoneOS GUI elements and internal data such as battery life and active tasks.
/// <para>TODO: Extend for in-game menu functionality.</para>
/// </summary>
public class PhoneOS : MonoBehaviour
{
	// Config Params ---------------
	[Header("OS Properties")]
	[SerializeField]
	float batteryMax = 100f;
	[Header("Operation Drain Rates")]
	[SerializeField]
	float baselineDrain = 0.1f;
	[SerializeField]
	float flashlightDrain = 2f;
	[SerializeField]
	float screenDrain = 0.5f;
	[Header("OS Screen Objects")]
	[SerializeField]
	Image offScreen;

	// Internal Variables ----------
	public float currentBatteryDrainRate = 0f;
	public float currentBattery;
	bool isDead = false;
	bool isFlashlight = true;
	bool isScreenOn = true;

	// Component References --------
	BatteryManager batteryManager;
	//TEMPORARY
	Text statusText;

	private void Start()
	{
		batteryManager = GetComponentInChildren<BatteryManager>();
		currentBattery = batteryMax;
		statusText = GetComponentInChildren<Text>();
	}

	// if the phone is not dead, drain the battery.
	private void Update()
	{
		if (isDead)
		{
			// destroy temporary text
			Destroy(GetComponentInChildren<Text>());
			return;
		}
		DrainBattery();
	}

	/// <summary>
	/// Check if PhoneOS is dead.
	/// </summary>
	/// <returns>true if PhoneOS is out of battery.</returns>
	public bool IsDead() => isDead;

	/// <summary>
	/// Handles battery drain and communication to the BatteryManager GUI element.
	/// <para>Calculates Drain amount and subtracts it from the current battery value. 
	/// Then checks for battery percentage, sending the state result to BatteryManager
	/// to be refected in the GUI object. If the battery charge is 0 or below, triggers
	/// phone death.</para>
	/// </summary>
	private void DrainBattery()
	{
		CalculateDrainAmount();

		currentBattery -= currentBatteryDrainRate * Time.deltaTime;
		currentBattery = Mathf.Clamp(currentBattery, 0, batteryMax);

		if (currentBattery <= 0)
		{
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.dead);
			isDead = true;
		}
		else if (currentBattery == batteryMax)
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.high);
		else if (currentBattery < batteryMax / 5)
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.low);
		else if (currentBattery < (batteryMax / 5) * 2)
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.midLow);
		else if (currentBattery < (batteryMax / 5) * 3)
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.mid);
		else if (currentBattery < (batteryMax / 5) * 4)
			batteryManager.ChangeBatteryState(BatteryManager.BatteryState.midHigh);
	}

	/// <summary>
	/// Calculates drain amount by adding to the baseline all active task's drain
	/// values. Can be extended easily for new tasks to drain battery.
	/// </summary>
	private void CalculateDrainAmount()
	{
		currentBatteryDrainRate = baselineDrain;
		currentBatteryDrainRate += isFlashlight ? flashlightDrain : 0;
		currentBatteryDrainRate += isScreenOn ? screenDrain : 0;
	}

	/// <summary>
	/// Toggles the flashlight task in PhoneOS, allowing battery drain to account
	/// for it.
	/// <para>TEMPORARY -> Also toggles UI text, to be removed with PhoneOS implementation.</para>
	/// </summary>
	public void ToggleLight() 
	{ 
		isFlashlight = !isFlashlight;

		statusText.text = isFlashlight ? "Flashlight ON" : "Flashlight OFF";
	}
	
	/// <summary>
	/// Toggles the screen task in PhoneOS, allowing the battry drain to account
	/// for it. Additionally hides the "offscreen" panel, displaying the PhoneOS UI.
	/// </summary>
	public void ToggleScreen()
	{
		isScreenOn = !isScreenOn;

		offScreen.gameObject.SetActive(isScreenOn ? false : true);
	}
}
