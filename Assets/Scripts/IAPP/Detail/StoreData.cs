using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IAPProductData
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

	[SerializeField]
	protected int amount;
	public int Amount
	{
		get { return amount; }
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

	public int GetProductAmount(IAPProductID brainzProductId)
	{
		IAPProductData iapProduct = IAPProducts.Find(p => p.BrainzProductId == brainzProductId);
		if(iapProduct == null)
			return 0;

		return iapProduct.Amount;
	}
}
