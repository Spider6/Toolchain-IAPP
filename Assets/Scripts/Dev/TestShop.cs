using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestShop : MonoBehaviour 
{
	[SerializeField]
	private Text textBox;

	[SerializeField]
	private IAPManager iapManager;


	private IIAPPlatform CurrentIAPPlatform
	{
		get{ return iapManager.IAPPlatform; }
	}

	private void Awake()
	{
		CurrentIAPPlatform.ProductListReceivedDelegate += OnProductListReceived;
		CurrentIAPPlatform.ProductListRequestFailedDelegate += OnProductListRequestFiled;
	}

	public void OnProductListReceived(IAPPlatformID platformId)
	{
		textBox.text = "Product list received \n";
		foreach(IAPProduct product in CurrentIAPPlatform.Products)
		{
			textBox.text += "BrainzProductId: " + product.brainzProductId + "\n";
			textBox.text += "CurrencyCode: " + product.currencyCode + "\n";
			textBox.text += "CurrencySymbol: " + product.currencySymbol + "\n";
			textBox.text += "Description: " + product.description + "\n";
			textBox.text += "FormattedPrice: " + product.formattedPrice + "\n";
			textBox.text += "Price: " + product.price + "\n";
			textBox.text += "Title: " + product.title + "\n";
			textBox.text += "=========================================================================\n";
		}
	}

	public void OnProductListRequestFiled(IAPPlatformID platformId, string error)
	{
		textBox.text = "Product list request filed: Platform: " + platformId.ToString() + " Error: " + error;
	}

	public void RequestProducts()
	{
		CurrentIAPPlatform.RequestAllProductData(this);
	}


}
