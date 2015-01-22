using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDebugStoreData
{
	List<IAPProduct> DebugProducts {get;}
	float StoreDebugDelayInSeconds {get;}
}
