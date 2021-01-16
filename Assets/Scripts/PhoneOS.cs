using UnityEngine;
using UnityEngine.UI;

public class PhoneOS : MonoBehaviour
{
	// Config Params ---------------
	[Header("OS Properties")]
	[SerializeField]
	float batteryMax = 100f;
	[SerializeField]
	float batteryDrainRate = 1f;
	[Header("OS Screen Objects")]
	[SerializeField]
	Image offScreen;

	public float currentBattery;

	bool isActive = true;
	bool isDead = false;

	BatteryManager batteryManager;

	private void Start()
	{
		batteryManager = GetComponentInChildren<BatteryManager>();
		currentBattery = batteryMax;
	}

	private void Update()
	{
		if (isActive)
			DrainBattery();
		if (isDead)
			Destroy(GetComponentInChildren<Text>());
	}

	public bool IsDead() => isDead;

	private void DrainBattery()
	{
		currentBattery -= batteryDrainRate * Time.deltaTime;
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

	public void ToggleLight()
	{
		// TODO, make this check for activity, IE script a toggle for turning the screen off
		// so that we don't get out of sync calls
		offScreen.gameObject.SetActive(!offScreen.gameObject.activeSelf);
		isActive = !isActive;
	}
}
