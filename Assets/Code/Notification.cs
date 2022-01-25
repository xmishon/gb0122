using UnityEngine;
using UnityEngine.UI;

public class Notification
{
    private GameObject _notificationGameObject;
    private Button _button;

    public Notification()
    {
        _notificationGameObject = Object.Instantiate(Resources.Load<GameObject>("Notification")); ;
        if (_notificationGameObject == null)
            return;
        _notificationGameObject.gameObject.SetActive(false);        
    }

    public void Show(string message)
    {
        Show(message, Color.white);
    }

    public void ShowError(string message)
    {
        Show(message, Color.red);
    }

    private void Show(string message, Color textColor)
    {
        _notificationGameObject.gameObject.SetActive(true);
        Text text = _notificationGameObject.GetComponentInChildren<Text>();
        text.text = message;
        text.color = textColor;
        _button = _notificationGameObject.GetComponentInChildren<Button>();
        _button.onClick.AddListener(DestroyGameObject);
    }

    private void DestroyGameObject()
    {
        _button.onClick.RemoveAllListeners();
        Object.Destroy(_notificationGameObject);
    }
}
