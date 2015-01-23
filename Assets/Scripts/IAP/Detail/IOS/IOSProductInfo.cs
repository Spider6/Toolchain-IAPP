using UnityEngine;
using System.Collections;
using IAP;

namespace IAP.Detail
{
	#if UNITY_IOS
	public class IOSProductInfo : IIOSProductInfo
	{
		public StoreKitProduct ProductInfo;

		public string ProductId 
		{
			get {return ProductInfo.productIdentifier;}
		}

		public string Title 
		{
			get {return ProductInfo.title;} 
		}

		public string Description 
		{
			get {return ProductInfo.description;}
		}

		public string Price 
		{
			get {return ProductInfo.price;}
		}

		public string CurrencySymbol
		{
			get {return ProductInfo.currencySymbol;}
		}

		public string CurrencyCode 
		{
			get {return ProductInfo.currencyCode;}
		}

		public string FormattedPrice 
		{
			get {return ProductInfo.formattedPrice;}
		}
	}
	#endif
}