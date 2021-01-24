using UnityEngine;
using UnityEngine.UI;

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

	public float currentBatteryDrainRate = 0f;
	public float currentBattery;

	bool isDead = false;
	bool isFlashlight = true;
	bool isScreenOn = true;

	BatteryManager batteryManager;
	//TEMPORARY
	Text statusText;

	private void Start()
	{
		batteryManager = GetComponentInChildren<BatteryManager>();
		currentBattery = batteryMax;
		statusText = GetComponentInChildren<Text>();
	}

	private void Update()
	{
		if (isDead)
		{
			Destroy(GetComponentInChildren<Text>());
			return;
		}

		DrainBattery();
	}

	public bool IsDead() => isDead;

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

	private void CalculateDrainAmount()
	{
		currentBatteryDrainRate = baselineDrain;
		currentBatteryDrainRate += isFlashlight ? flashlightDrain : 0;
		currentBatteryDrainRate += isScreenOn ? screenDrain : 0;
	}

	public void ToggleLight() 
	{ 
		isFlashlight = !isFlashlight;

		statusText.text = isFlashlight ? "Flashlight ON" : "Flashlight OFF";
	}
	
	public void ToggleScreen()
	{
		isScreenOn = !isScreenOn;

		offScreen.gameObject.SetActive(isScreenOn ? false : true);
	}
}
