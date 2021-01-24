using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles FPS control for a player. Features:
/// <list type="bullet">
/// <item>Movement and Sprint</item>
/// <item>Mouse Look</item>
/// <item>Head Bob</item>
/// <item>FOV Zoom</item>
/// </list>
/// </summary>
public class FPSController : MonoBehaviour
{
	// Config Parameters ---------------
	[Header("Movement")]
	[SerializeField]
	float moveSpeed = 2;
	[SerializeField]
	float runSpeed = 5;
	[SerializeField]
	float mass = 2;
	[SerializeField]
	LayerMask groundMask;

	[Header("View")]
	[SerializeField]
	GameObject headObject;
	[SerializeField]
	float viewSensitivity = 400f;
	[SerializeField]
	float baseFOV = 60;
	[SerializeField]
	float zoomFOV = 30;
	[SerializeField]
	float zoomTime = 0.5f;

	[Header("HeadBob")]
	[SerializeField]
	float bobRange = 0.025f;
	[SerializeField]
	float bobIntensity = 0.5f;
	[SerializeField]
	float runBobMultiplier = 2f;
	[SerializeField]
	GameObject phoneObject;
	[SerializeField]
	float phoneBobMultiplier = 0.25f;
	[SerializeField]
	float phoneJitterRange = 0.5f;
	[SerializeField]
	float phoneJitterIntensity = 0.25f;

	// Internal Variables --------------
	bool isRunning = false;
	bool isMoving = false;
	bool isGrounded;
	bool isZoomed;
	float currentZoomTime = 0f;
	float currentBobIntensity = 0f;
	float currentPhoneJitter = 0f;
	float xRotation = 0;
	Vector3 velocity;
	Vector3 cameraBase;
	float gravity = -9.81f;
	float groundDistance = 0.1f;

	// Component References ------------
	CharacterController controller;
	PhoneController phone;
	Camera mainCam;
	PlayerDanger danger;


	// Hooks component references and locks the cursor.
	void Start()
	{
		mainCam = GetComponentInChildren<Camera>();
		controller = GetComponent<CharacterController>();
		phone = GetComponentInChildren<PhoneController>();
		danger = GetComponent<PlayerDanger>();
		cameraBase = mainCam.transform.localPosition;

		Cursor.lockState = CursorLockMode.Locked;
	}

	// Checks if dead before processing input methods
	void Update()
	{
		if (danger.isDead) return;
		Move();
		Look();
		HeadBob();
		Zoom();
	}

	/// <summary>
	/// Handles view zoom.
	/// <para>Checks for input, and linearly interpolates between base camera FOV
	/// to zoom camera FOV based on input status.</para>
	/// </summary>
	private void Zoom()
	{
		isZoomed = Input.GetButton("Middle Click") ? true : false;

		if (isZoomed && currentZoomTime < zoomTime)
		{
			mainCam.fieldOfView = Mathf.Lerp(baseFOV, zoomFOV, currentZoomTime / zoomTime);
			currentZoomTime += Time.deltaTime;
		}
		else if (!isZoomed && currentZoomTime > 0)
		{
			mainCam.fieldOfView = Mathf.Lerp(baseFOV, zoomFOV, currentZoomTime / zoomTime);
			currentZoomTime -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Handles player movement and sprint checks.
	/// <para>Checks for running input, and then for Horizontal and Vertical
	/// axis values. Uses the CharacterControler components Move method to 
	/// tranlate the player.</para>
	/// <para>This method also has leftover gravity checks that are beyond
	/// the scope of what we need for this project but it does help handle
	/// more steep terrain in a "realistic" fashion.</para>
	/// </summary>
	private void Move()
	{
		isRunning = Input.GetButton("Run");

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		if (x != 0 || z != 0)
			isMoving = true;
		else isMoving = false;

		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * (isRunning ? runSpeed : moveSpeed) * Time.deltaTime);

		// This only works because the test player model I'm using has their origin set at their feet
		// If i were to change models or rigs, I would need to either make sure the origin also at
		// the ground, or add a collider object for the ground check
		isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
		if (isGrounded && velocity.y < 0)
			velocity.y = -2f;

		velocity.y += gravity * mass * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	/// <summary>
	/// Handles Head and Phone Bob calculations based on movement and speed.
	/// <para>Effectively does a linear interpolation between positive- and negative-bob values
	/// over time multiplied by intensity. This is then applied to a lesser degree to the phone's
	/// translation by way of passing calculated jitter values to the PhoneController ViewShift
	/// methond.</para>
	/// <para>Its a bit janky but it works well enough.</para>
	/// </summary>
	private void HeadBob()
	{
		if (isMoving)
		{
			// Head Bob
			if (currentBobIntensity >= bobRange || currentBobIntensity <= -bobRange)
				bobIntensity = -bobIntensity;

			currentBobIntensity += bobIntensity * Time.deltaTime * (isRunning ? runBobMultiplier : 1);
			currentBobIntensity = Mathf.Clamp(currentBobIntensity, -bobRange, bobRange);
			mainCam.transform.localPosition = new Vector3(0, currentBobIntensity, 0) + cameraBase;

			// Phone Jitter
			if (currentPhoneJitter >= phoneJitterRange || currentPhoneJitter <= -phoneJitterRange)
				phoneJitterIntensity = -phoneJitterIntensity;

			currentPhoneJitter += phoneJitterIntensity * Time.deltaTime * runBobMultiplier;
			currentPhoneJitter = Mathf.Clamp(currentPhoneJitter, -phoneJitterRange, phoneJitterRange);
			phone.ViewShift(xRotation, currentPhoneJitter, currentBobIntensity * phoneBobMultiplier);
		}
		else
			phone.ViewShift(xRotation, 0, 0);
	}

	/// <summary>
	/// Handles mouse look, clamping X roation (up and down) to reasonable values.
	/// <para>TODO: Rework the clamp values, since the placeholder player model
	/// has been removed, and those values were based on that model.</para>
	/// </summary>
	private void Look()
	{
		float mouseX = Input.GetAxis("Mouse X") * viewSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * viewSensitivity * Time.deltaTime;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -45, 30);

		headObject.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

		transform.Rotate(Vector3.up * mouseX);
	}
}
