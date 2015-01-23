using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IAP
{
	public interface IDebugStoreData
	{
		List<IAPProduct> DebugProducts {get;}
		float StoreDebugDelayInSeconds {get;}
	}

	public interface IStoreSettings
	{
		string GooglePublicKey {get;}
		float TimeOutToStore {get;}
	}
}
