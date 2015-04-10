using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IAP;

public class IAPProductInput : MonoBehaviour
{
	[SerializeField]
	private InputField idProductInput;

	[SerializeField]
	private string brainzProductId;

	public string IdProduct
	{
		get { return idProductInput.text; }
	}

	public string BrainzProductId
	{
		get { return brainzProductId; }
	}
	
	public void Purchase()
	{
		IAPDataManager.Instance.IAPManager.PurchaseProduct(brainzProductId);
	}


}
