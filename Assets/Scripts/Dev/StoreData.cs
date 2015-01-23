using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IAP;

[System.Serializable]
public class IAPProductData : IIAPProductData
{
	[SerializeField]
	protected string brainzProductId;
	public string BrainzProductId
	{
		get { return brainzProductId; }
	}

	[SerializeField]
	protected string iapProductId;
	public string IAPProductId
	{
		get { return iapProductId; }
	}
}

[System.Serializable]
public class StoreData
{
	[SerializeField]
	protected List<IAPProductData> iapProducts = new List<IAPProductData>();
	public List<IAPProductData> IAPProducts
	{
		get { return iapProducts; }
	}
}
