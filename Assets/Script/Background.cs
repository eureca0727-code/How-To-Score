using UnityEngine;

public class MapBackground : MonoBehaviour
{
    void OnMouseDown()
    {
        // UI 클릭이 아닐 때만 실행
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return; // UI 위에 있으면 무시
        }

        RegionInfoUI.Instance.ShowOverallStats();
    }
}