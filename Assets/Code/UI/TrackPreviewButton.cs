using UnityEngine;
using UnityEngine.UI;

public class TrackPreviewButton : MonoBehaviour
{
    [SerializeField] private GameObject _isSelected;
    [SerializeField] private Text _name;
    [SerializeField] private Image _icon;

    public void FillButtonInfo(string name, Sprite icon)
    {
        _name.text = name;
        _icon.sprite = icon;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _isSelected.SetActive(selected);
    }
}
