using UnityEngine;
using System;

public class FBManager
{
	private static FBManager instance;
	
	public static FBManager Instance
	{
		get
		{
			if (instance == null)
				instance = new FBManager ();
			
			return instance;
		}
	}

	public void Init ()
	{
		FB.Init (OnInitComplete, OnHideUnity, null);
	}
	
	private void OnInitComplete ()
	{
		Debug.Log("Facebook initialize Complete");
	}
	
	private void OnHideUnity (bool isGameShown)
	{
		Debug.Log("Facebook is game showing? " + isGameShown);
	}

	public void Login()
	{
		if(!FB.IsLoggedIn)
			FB.Login("", OnResponseLogin);

		else
			FB.Logout();
	}

	private void OnResponseLogin(FBResult result)
	{
		if(FB.IsLoggedIn)
			Debug.Log("Login susccess: " + FB.UserId);
		else
			Debug.Log("Login fail error: " + result.Error + " text: " + result.Text);
	}
}

