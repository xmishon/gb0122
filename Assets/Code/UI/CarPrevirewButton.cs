using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarPrevirewButton : MonoBehaviour
{
    [SerializeField] Text _carNameLabel;
    [SerializeField] Image _carImage;
    [SerializeField] Text _carPrice;
    [SerializeField] GameObject _isBoughtIcon;

    private bool _isBought;

    public void FillButtonInfo(string carId, string displayName, string carPrice, bool isBought)
    {
        _carNameLabel.text = displayName;
        if (isBought)
        {
            _isBoughtIcon.SetActive(true);
        }
        else
        {
            _carPrice.text = carPrice;
            _isBoughtIcon.SetActive(false);
        }
        var carImage = Resources.Load<Sprite>("icon_" + carId);
        _carImage.sprite = carImage;
    }
}
