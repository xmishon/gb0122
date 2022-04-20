using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrackChoose : MonoBehaviour
{
    private const string TRACK_BUTTON_NAME = "TrackButton";

    [SerializeField] private List<Track> _tracks;
    [SerializeField] private GameObject _trackListPanel;

    private readonly List<TrackPreviewButton> _trackButtons = new List<TrackPreviewButton>();

    public void FillTracksArray()
    {
        int childNums = _trackListPanel.transform.childCount;
        for (int i = 0; i < childNums; i++)
        {
            Destroy(_trackListPanel.transform.GetChild(i).gameObject);
        }
        _trackButtons.Clear();
        foreach (var track in _tracks)
        {
            GameObject trackButton = Instantiate(Resources.Load<GameObject>(TRACK_BUTTON_NAME));
            TrackPreviewButton trackPreviewButton = trackButton.GetComponent<TrackPreviewButton>();
            _trackButtons.Add(trackPreviewButton);
            trackPreviewButton.FillButtonInfo(track.trackName, track.trackIcon);
            trackPreviewButton.GetComponent<Button>().onClick.AddListener(delegate
            {
                foreach(var button in _trackButtons)
                {
                    button.SetSelected(false);
                }
                trackPreviewButton.SetSelected(true);
                if (GameCore.instance != null)
                {
                    GameCore.instance.trackSetup.trackConfigNum = track.configNum;
                }
                else
                {
                    new GameCore().trackSetup.trackConfigNum = track.configNum;
                }
                GameCore.instance.trackSetup.trackSceneNum = track.sceneNum;
            });
            trackPreviewButton.transform.SetParent(_trackListPanel.transform);
        }
    }
}
