using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class ProductData
{
	[SerializeField]
	protected IAPProductID brainzProductID;
	public IAPProductID BrainzProductID
	{
		get { return brainzProductID; }
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
	protected List<ProductData> products = new List<ProductData>();
	public List<ProductData> Products
	{
		get { return products; }
	}

	public int GetProductAmount(IAPProductID productId)
	{
		foreach (ProductData product in products)
		{
			if (product.BrainzProductID == productId)
				return product.Amount;
		}
		
		return 0;
	}
}
