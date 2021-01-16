using UnityEngine;
using UnityEngine.UI;

public class PhoneOS : MonoBehaviour
{
	// Config Params ---------------
	[Header("OS Screen Objects")]
	[SerializeField]
	Image offScreen;

	public void ToggleLight()
	{
		// TODO, make this check for activity, IE script a toggle for turning the screen off
		// so that we don't get out of sync calls
		offScreen.gameObject.SetActive(!offScreen.gameObject.activeSelf);
	}
}
