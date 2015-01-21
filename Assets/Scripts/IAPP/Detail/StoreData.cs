using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface IIAPProductData
{
	IAPProductID BrainzProductId { get; }
	string IAPProductId { get; }
    float Price	{ get; }
}


[System.Serializable]
public class IAPProductData : IIAPProductData
{
	[SerializeField]
	protected IAPProductID brainzProductId;
	public IAPProductID BrainzProductId
	{
		get { return brainzProductId; }
	}

	[SerializeField]
	protected string iapProductId;
	public string IAPProductId
	{
		get { return iapProductId; }
	}

	[SerializeField]
	protected float price;
	public float Price
	{
		get { return price; }
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
