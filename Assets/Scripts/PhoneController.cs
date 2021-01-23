using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
	// Config Params ------------------
	[Header("State Positions")]
	[SerializeField]
	Transform cameraPos;
	[SerializeField]
	Vector3 restPos;
	[SerializeField]
	Vector3 restRot;
	[SerializeField]
	Vector3 menuPos;
	[SerializeField]
	Vector3 menuRot;
	[SerializeField]
	float menuTransitionTime = 0.5f;

	[Header("Other Params")]
	[SerializeField]
	float menuLookInfluence = 0.5f;

	// Component References ------------
	PhoneOS phoneOS;
	Light flashlight;
	PostProcessFX ppFX;

	// Internal Variables --------------
	bool lightOn = false;
	bool inRestPos = true;
	public Vector3 currentBasePos;
	Vector3 startPos;

	float currentTransitionTime = 0f;

	void Start()
	{
		phoneOS = FindObjectOfType<PhoneOS>();
		flashlight = GetComponentInChildren<Light>();
		ppFX = FindObjectOfType<PostProcessFX>();
	}

	void Update()
	{
		Flashlight();
		ScreenToggle();
		StateSwitch();
	}

	void ScreenToggle() 
	{
		if (phoneOS.IsDead()) return;
		if (Input.GetButtonDown("Screen"))
			phoneOS.ToggleScreen();
	}

	// this feels like spaghetti code
	public void ViewShift(float viewAngle, float xJitterValue, float yBobValue)
	{
		// this is literally here to preserve positioning of the phone when not in the menu
		// because the menu offselt calculation assumes the phone needs to stay at base y = 0
		// which isnt the case when we arent in the menu
		float offset = restPos.y;

		// find the height offset of the phone from the view angle and limit its influence
		if (!inRestPos)
			offset = menuLookInfluence * Mathf.Tan(Mathf.Deg2Rad * viewAngle) * transform.localPosition.z;

		// apply menu offset if any, along with phone bob offsets
		transform.localPosition = new Vector3(
			transform.localPosition.x + xJitterValue, offset + yBobValue, transform.localPosition.z); ;

	}

	private void StateSwitch()
	{
		if (Input.GetButtonDown("Menu"))
		{
			inRestPos = !inRestPos;
			startPos = transform.localPosition;
		}

		if (inRestPos && currentTransitionTime != 0)
		{
			PhoneStateLerp(restPos, startPos);
			currentTransitionTime -= Time.deltaTime;
		}
		else if (!inRestPos && currentTransitionTime != menuTransitionTime)
		{
			PhoneStateLerp(startPos, menuPos);
			currentTransitionTime += Time.deltaTime;

		}
		currentTransitionTime = Mathf.Clamp(currentTransitionTime, 0, menuTransitionTime);
	}

	private void Flashlight()
	{
		if (phoneOS.IsDead())
		{
			flashlight.gameObject.SetActive(false);
			lightOn = false;
			return;
		}
		if (Input.GetButtonDown("Flashlight"))
		{
			lightOn = !lightOn;
			phoneOS.ToggleLight();
			flashlight.gameObject.SetActive(!flashlight.gameObject.activeSelf);
		}
	}

	private void PhoneStateLerp(Vector3 restPostion, Vector3 menuPosition)
	{
		transform.localPosition = Vector3.Lerp(restPostion, menuPosition, currentTransitionTime / menuTransitionTime);
		transform.localRotation = Quaternion.Lerp(
			Quaternion.Euler(restRot), Quaternion.Euler(menuRot), currentTransitionTime / menuTransitionTime);

		// update Depth of Field
		ppFX.LerpFocalLength(currentTransitionTime / menuTransitionTime);

		// update current base pos for headbob + jitter
		currentBasePos = transform.localPosition;
	}
}
