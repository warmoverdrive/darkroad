using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	// Parameters ----------------------
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


	void Start()
	{
		mainCam = GetComponentInChildren<Camera>();
		controller = GetComponent<CharacterController>();
		phone = GetComponentInChildren<PhoneController>();
		cameraBase = mainCam.transform.localPosition;

		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		Move();
		Look();
		HeadBob();
		Zoom();
	}

	private void Zoom()
	{
		if (Input.GetButtonDown("Right Click"))
			isZoomed = true;
		else if (Input.GetButtonUp("Right Click"))
			isZoomed = false;

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

			currentPhoneJitter += phoneJitterIntensity * Time.deltaTime * (isRunning ? runBobMultiplier : 1);
			currentPhoneJitter = Mathf.Clamp(currentPhoneJitter, -phoneJitterRange, phoneJitterRange);
			phone.ViewShift(xRotation, currentPhoneJitter, currentBobIntensity * phoneBobMultiplier);
		}
		else
			phone.ViewShift(xRotation, 0, 0);
	}

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
