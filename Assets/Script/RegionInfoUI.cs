using TMPro;
using UnityEngine;

public class RegionInfoUI : MonoBehaviour
{
    public static RegionInfoUI Instance;

    public GameObject panel;
    public TextMeshProUGUI infoText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowRegionInfo(Region region)
    {
        panel.SetActive(true);

        string info = $"Region: {region.regionName}\n";
        info += $"Population: {region.population}\n\n";

        // 정당 지지도
        info += "Party Support\n";
        info += $"Party A: {region.partyA.supportRate}%\n";
        info += $"Party B: {region.partyB.supportRate}%\n";
        info += $"Party C: {region.partyC.supportRate}%\n\n";

        // 전체 정책 수요도
        info += "Policy Demands \n(PartySupport*Demands) \n";
        info += $"Economy: {region.GetEconomyDemand():F1}\n";
        info += $"Welfare: {region.GetWelfareDemand():F1}\n";
        info += $"Security: {region.GetSecurityDemand():F1}\n";
        info += $"Environment: {region.GetEnvironmentDemand():F1}\n\n";

        // 상위 2개 정책
        info += "--- Top 2 Policies ---\n";
        info += region.GetTop2Policies();

        infoText.text = info;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}