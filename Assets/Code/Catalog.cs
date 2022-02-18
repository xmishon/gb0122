using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Catalog : MonoBehaviour
{
    private const string CAR_CLASS_NAME = "Car";
    private const string CAR_BUTTON_NAME = "CarButton";

    [SerializeField] GameObject _carListPanel;

    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();
    private readonly List<CarPreviewButton> _carPreviewButtons = new List<CarPreviewButton>();

    public void GetCatalog()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(),
            result => 
            {
                HandleCatalog(result.Catalog);
            }, error => 
            {
                Debug.LogError("Couldn't get Catalog from the Server!");
            });
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        foreach(CatalogItem item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Catalog item {item.DisplayName} ({item.ItemClass}) successfully loaded!");
        }
        FillButtonsArray();
    }

    private void FillButtonsArray()
    {
        foreach (var item in _catalog)
        {
            if(item.Value.ItemClass == CAR_CLASS_NAME)
            {
                GameObject carButton = GameObject.Instantiate(Resources.Load<GameObject>(CAR_BUTTON_NAME));
                CarPreviewButton carPreviewButton = carButton.GetComponent<CarPreviewButton>();
                _carPreviewButtons.Add(carPreviewButton);
                uint price;
                if (item.Value.VirtualCurrencyPrices.TryGetValue("DG", out price))
                {
                    carPreviewButton.FillButtonInfo(item.Value.ItemId, item.Value.DisplayName, price.ToString(), /*Temporary*/false);
                }
                else
                {
                    carPreviewButton.FillButtonInfo(item.Value.ItemId, item.Value.DisplayName, "0", /*Temporary*/true);
                }
                carButton.GetComponent<Button>().onClick.AddListener(delegate {
                    if(GameCore.instance != null)
                    {
                        GameCore.instance.carSetup.currentCarId = item.Value.ItemId;
                    }
                    else
                    {
                        new GameCore().carSetup.currentCarId = item.Value.ItemId;
                    }
                    foreach (var button in _carPreviewButtons)
                    {
                        button.SetIsBought(false);
                    }
                    carPreviewButton.SetIsBought(true);
                });
                carButton.transform.SetParent(_carListPanel.transform);
            }
        }
    }
}
