using UnityEngine;

[CreateAssetMenu(menuName = "Tracks/TrackName")]
public class Track : ScriptableObject
{
    public Sprite trackIcon;
    public string trackName;
    public int sceneNum;
    public int configNum;
}
