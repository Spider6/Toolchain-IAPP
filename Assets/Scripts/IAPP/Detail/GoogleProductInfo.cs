using UnityEngine;
using System.Collections;

public class GoogleProductInfo : IGoogleProductInfo
{
	public GoogleSkuInfo ProductInfo
	{
		get;
		set;
	}

	public string Title 
	{
		get {return ProductInfo.title;}
	}

	public string Price 
	{
		get {return ProductInfo.price;}
	}

	public string Description 
	{
		get {return ProductInfo.description;}
	}

	public string ProductId 
	{
		get {return ProductInfo.productId;}
	}

	public string PriceCurrencyCode 
	{
		get {return ProductInfo.priceCurrencyCode;}
	}
}
