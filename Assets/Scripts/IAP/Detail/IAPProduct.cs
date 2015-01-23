using UnityEngine;
using System.Collections;
using System;

namespace IAP
{
	public enum IAPPlatformID
	{
		Dummy = 0,
		StoreKit = 1,
		GoogleIAB = 2
	}

	public class IAPProductInfo
	{
		public string BrainzProductId {get; private set;}
		public string Price {get; private set;}
		public string CurrencyPrice {get; set;}
		public IAPProductInfo (string brainzProductId, string price)
		{
			this.BrainzProductId = brainzProductId;
			this.Price = price;
		}
	}

	[System.Serializable]
	public class IAPProduct
	{
		public string brainzProductId;
		public string title;
		public string description;
		public string price;
		public string formattedPrice;
		public string currencySymbol;
		public string currencyCode;
		
		public override string ToString()
		{	
			return System.String.Format( "<Product>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nPrice Formatted: {4}\nCurrency Symbol: {5}\nCurrency Code: {6}",
			                            brainzProductId, title, description, price, formattedPrice, currencySymbol, currencyCode );
		}
	}
}