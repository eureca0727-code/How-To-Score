using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoUI : MonoBehaviour
{
    public static RegionInfoUI Instance;
    public GameObject panel;
    public TextMeshProUGUI infoText;

    [Header("Bar Chart")]
    public PartyBarChart barChart;

    [Header("Buttons")]
    public Button promoteButton; // 홍보 버튼

    private Region currentRegion; // 현재 선택된 지역 (null이면 전국)

    void Awake()
    {
        Instance = this;
        panel.SetActive(true);

        // 홍보 버튼 클릭 이벤트 연결
        if (promoteButton != null)
        {
            // 강제로 켜기
            promoteButton.gameObject.SetActive(true);

            promoteButton.onClick.AddListener(OnPromoteButtonClick);
        }
    }

    void Start()
    {
        ShowOverallStats();
    }

    // 전체 통계 표시
    public void ShowOverallStats()
    {
        panel.SetActive(true);
        currentRegion = null; // 전국 모드
        
        int totalA = GameManager.Instance.GetTotalPartyASupport();
        int totalB = GameManager.Instance.GetTotalPartyBSupport();
        int totalC = GameManager.Instance.GetTotalPartyCSupport();

        barChart.UpdateChart(totalA, totalB, totalC);

        int totalPopulation = 120;
        string info = "=== Overall Statistics ===\n\n";
        info += $"Total Population: {totalPopulation} M \n\n";
        info += "Total Supporters:\n";
        info += $"Party A: {totalA} M people\n";
        info += $"Party B: {totalB} M people\n";
        info += $"Party C: {totalC} M people\n";

        infoText.text = info;

        // 버튼 텍스트 변경
        if (promoteButton != null)
        {
            promoteButton.GetComponentInChildren<TextMeshProUGUI>().text = "Promote Nationwide";
        }
    }

    // 특정 지역 정보 표시
    public void ShowRegionInfo(Region region)
    {
        panel.SetActive(true);
        currentRegion = region; // 현재 지역 저장

        barChart.UpdateChartPercent(
            region.partyA.supportRate,
            region.partyB.supportRate,
            region.partyC.supportRate
        );

        string info = $"Region: {region.regionName}\n";
        info += $"Population: {region.population} M \n\n";

        info += "Policy Demands\n(PartySupport*Demands)\n";
        info += $"Economy: {region.GetEconomyDemand():F1}\n";
        info += $"Welfare: {region.GetWelfareDemand():F1}\n";
        info += $"Security: {region.GetSecurityDemand():F1}\n";
        info += $"Environment: {region.GetEnvironmentDemand():F1}\n\n";

        info += "--- Top 2 Policies ---\n";
        info += region.GetTop2Policies();

        infoText.text = info;

        // 버튼 텍스트 변경
        if (promoteButton != null)
        {
            promoteButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Promote in {region.regionName}";
        }
    }

    // 홍보 버튼 클릭 시
    void OnPromoteButtonClick()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPromotionView(currentRegion);
        }
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}