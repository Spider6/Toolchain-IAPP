using UnityEngine;
using System.Collections;
using System;

public interface IIAPManager  
{
	IIAPPlatform IAPPlatform {get;}
	bool AreProductsLoaded {get;}
	bool PurchaseProduct(string brainzProductId);
	void ConsumeProduct(string brainzProductId);
}