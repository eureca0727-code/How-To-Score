using UnityEngine;

public class Background : MonoBehaviour
{
    void OnMouseDown()
    {
        RegionInfoUI.Instance.ShowOverallStats();
    }
}