using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElement : MonoBehaviour
{
    [SerializeField]
    private Text _playerName;

    public void SetItem(Player p)
    {
        _playerName.text = p.UserId;
    }
}
