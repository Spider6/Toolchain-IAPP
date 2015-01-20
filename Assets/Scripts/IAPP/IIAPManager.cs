using UnityEngine;
using System.Collections;
using System;

public interface IIAPManager  
{
	IIAPPlatform IAPPlatform {get;}
	bool AreProductsLoaded {get;}
	bool PurchaseProduct(IAPProductID id);
	void ConsumeProduct(IAPProductID id);
}