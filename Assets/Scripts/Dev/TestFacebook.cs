using UnityEngine;
using System.Collections;

public class TestFacebook : MonoBehaviour
{
	private void Awake ()
	{
		FBManager.Instance.Init();
	}

	public void Login()
	{
		FBManager.Instance.Login();
	}
}
