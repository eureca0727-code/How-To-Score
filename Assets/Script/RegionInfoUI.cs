using TMPro;
using UnityEngine;

public class RegionInfoUI : MonoBehaviour
{
    public static RegionInfoUI Instance;
    public GameObject panel;
    public TextMeshProUGUI infoText;

    [Header("Bar Chart")]
    public PartyBarChart barChart; 

    void Awake()
    {
        Instance = this;
        panel.SetActive(true); // 시작할 때 켜둠
    }

    void Start()
    {
        ShowOverallStats(); // 게임 시작 시 전체 통계 표시
    }

    // 전체 통계 표시
    public void ShowOverallStats()
    {
        panel.SetActive(true);

        int totalA = GameManager.Instance.GetTotalPartyASupport();
        int totalB = GameManager.Instance.GetTotalPartyBSupport();
        int totalC = GameManager.Instance.GetTotalPartyCSupport();

        barChart.UpdateChart(totalA, totalB, totalC); //전국 지지도 막대 비율용

        int totalPopulation = 120; // 전체 인구

        string info = "=== Overall Statistics ===\n\n";
        info += $"Total Population: {totalPopulation}\n\n";
        info += "Total Supporters:\n";
        info += $"Party A: {totalA} people\n";
        info += $"Party B: {totalB} people\n";
        info += $"Party C: {totalC} people\n";

        infoText.text = info;
    }

    // 특정 지역 정보 표시
    public void ShowRegionInfo(Region region)
    {
        panel.SetActive(true);

        barChart.UpdateChartPercent(
            region.partyA.supportRate,
            region.partyB.supportRate,
            region.partyC.supportRate);
                                        
        string info = $"Region: {region.regionName}\n";
        info += $"Population: {region.population}\n\n";


        // 전체 정책 수요도
        info += "Policy Demands\n(PartySupport*Demands)\n";
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