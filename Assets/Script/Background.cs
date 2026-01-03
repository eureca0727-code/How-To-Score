using UnityEngine;

public class MapBackground : MonoBehaviour
{
    void OnMouseDown()
    {
        RegionInfoUI.Instance.ShowOverallStats();
    }
}