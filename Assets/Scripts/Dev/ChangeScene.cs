using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour 
{
	[SerializeField]
	private string sceneName;

	public void LoadScene()
	{
		Application.LoadLevel(sceneName);
	}

}
