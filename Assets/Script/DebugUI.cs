using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public static DebugUI Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        // 켜질 때 Instance 재설정 (비활성화 상태에서 켜진 경우 대비)
        Instance = this;
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdateDebugInfo();
        }
    }

    void UpdateDebugInfo()
    {
        string info = "<b>=== 전체 지역 지지도(F12 On/Off) ===</b>\n\n";

        Region[] allRegions = GameManager.Instance.mapContainer.GetComponentsInChildren<Region>();
        System.Array.Sort(allRegions, (a, b) => string.Compare(a.regionName, b.regionName));

        foreach (var region in allRegions)
        {
            info += $"<b>{region.regionName}</b> (구역{region.districtId})\n";
            info += $"  갑:{region.partyA.supportRate}% 을:{region.partyB.supportRate}% 병:{region.partyC.supportRate}%\n\n";
        }

        debugText.text = info;
    }
}