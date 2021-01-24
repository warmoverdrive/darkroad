using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Phone Object transform, and checks user input related to the Phone.
/// <para>TODO: Extend input detection for future PhoneOS controls</para>
/// </summary>
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

	// Internal Variables --------------
	bool lightOn = true;
	bool inRestPos = true;
	public Vector3 currentBasePos;
	Vector3 startPos;
	float currentTransitionTime = 0f;

	// Component References ------------
	PhoneOS phoneOS;
	Light flashlight;
	PostProcessFX ppFX;
	PlayerDanger danger;

	/// <summary>
	/// Getter for flashlight status.
	/// </summary>
	/// <returns>true if light is on</returns>
	public bool IsLightOn() => lightOn;

	void Start()
	{
		phoneOS = FindObjectOfType<PhoneOS>();
		flashlight = GetComponentInChildren<Light>();
		ppFX = FindObjectOfType<PostProcessFX>();
		danger = GetComponentInParent<PlayerDanger>();
	}

	// Runs phone input methods if player is not dead.
	void Update()
	{
		if (danger.isDead)
			return;

		Flashlight();
		ScreenToggle();
		StateSwitch();
	}

	/// <summary>
	/// Checks for input to toggle the screen, and calls the ToggleScreen method
	/// from the PhoneOS.
	/// <para>Early return if phone is dead.</para>
	/// </summary>
	void ScreenToggle() 
	{
		if (phoneOS.IsDead()) return;
		if (Input.GetButtonDown("Screen"))
			phoneOS.ToggleScreen();
	}

	/// <summary>
	/// Handles the phone bobbing, as well as offset movement while in the
	/// menu, for looking up and down at the screen when its close to you.
	/// <para>This code could be improved, but it works well enough for now
	/// and I can't be bothered to overhaul it for what is essentially a low
	/// priority feature.</para>
	/// </summary>
	/// <param name="viewAngle">Angle of camera view relative to phone screen</param>
	/// <param name="xJitterValue">X orientation jitter value to add to local transform</param>
	/// <param name="yBobValue">Y value orientation bob value to add to local transform</param>
	public void ViewShift(float viewAngle, float xJitterValue, float yBobValue)
	{
		// this feels like spaghetti code

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

	/// <summary>
	/// Handles hand states for where the phone is located.
	/// <para>Toggles between "rest" position and "menu" position, calling
	/// Linear Interpolation methods to smoothly transition between them.
	/// Uses CurrentTransitionTime like a percentage between the to positions,
	/// subtracting from CurrentTransitionTime to return it to resting position,
	/// and adding to send it to menu position. Clamps CurrentTransitionTime to
	/// avoid weird edge cases relating to adding delta time.</para>
	/// <para>startPos variable is used to handle interpolating to a new location if
	/// transition is stopped or switched midway, in order to prevent jitter/bob from
	/// completely breaking the animation.</para>
	/// </summary>
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

	/// <summary>
	/// Handles reading input for toggling the flashlight object when the phone 
	/// is not dead.
	/// <para>Does an internal check for death, and will toggle the flashlight
	/// off if the phoneOS returns dead. Will also send a message to the phoneOS
	/// regarding flashlight state.</para>
	/// </summary>
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

	/// <summary>
	/// Handles the Linear Interpolation between phone positions.
	/// <para>Also updates Post Processing FX for depth of field changes, based on Lerp t value.</para>
	/// </summary>
	/// <param name="restPostion">Rest position for phone</param>
	/// <param name="menuPosition">Menu position for phone</param>
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
