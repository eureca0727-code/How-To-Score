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

        float totalA = GameManager.Instance.GetTotalPartyASupport();
        float totalB = GameManager.Instance.GetTotalPartyBSupport();
        float totalC = GameManager.Instance.GetTotalPartyCSupport();

        int totalPopulation = 120;

        // 퍼센트로 변환 후 int로 변환
        int percentA = Mathf.RoundToInt((totalA / totalPopulation) * 100f);
        int percentB = Mathf.RoundToInt((totalB / totalPopulation) * 100f);
        int percentC = Mathf.RoundToInt((totalC / totalPopulation) * 100f);

        // 반올림 오차 보정 (합이 100이 되도록)
        int total = percentA + percentB + percentC;
        if (total != 100)
        {
            int diff = 100 - total;
            // 가장 큰 정당에게 오차 할당
            if (percentA >= percentB && percentA >= percentC)
                percentA += diff;
            else if (percentB >= percentC)
                percentB += diff;
            else
                percentC += diff;
        }

        // 바 차트에 퍼센트 전달
        barChart.UpdateChartPercent(percentA, percentB, percentC);

        string info = "<size=28><b>전국 통계</b></size>\n\n";
        info += $"총 인구: {totalPopulation}백만 명\n\n";
        info += "정당별 지지자 수:\n\n\n";
        info += $"정당 갑: {totalA:F1}백만 명\n\n";
        info += $"정당 을: {totalB:F1}백만 명\n\n";
        info += $"정당 병: {totalC:F1}백만 명\n";

        infoText.text = info;

        // 버튼 텍스트 변경
        if (promoteButton != null)
        {
            promoteButton.GetComponentInChildren<TextMeshProUGUI>().text = "전국 홍보";
        }
    }    // 특정 지역 정보 표시
    public void ShowRegionInfo(Region region)
    {
        panel.SetActive(true);
        currentRegion = region; // 현재 지역 저장

        barChart.UpdateChartPercent(
            region.partyA.supportRate,
            region.partyB.supportRate,
            region.partyC.supportRate
        );

        string info = $"<size=28><b>{region.regionName}</b></size>\n\n";
        // 지역 특성 문구 추가
        if (!string.IsNullOrEmpty(region.regionDescription))
        {
            info += $"{region.regionDescription}\n\n";
        }

        info += $"인구: {region.population}백만 명\n\n";
        info += "정책 수요도\n(정당지지율 × 수요도)\n";
        info += $"경제: {region.GetEconomyDemand():F1}\n";
        info += $"복지: {region.GetWelfareDemand():F1}\n";
        info += $"안보: {region.GetSecurityDemand():F1}\n";
        info += $"환경: {region.GetEnvironmentDemand():F1}\n\n";
        info += "-상위 2개 정책-\n\n";
        info += region.GetTop2Policies();

        infoText.text = info;

        // 버튼 텍스트 변경
        if (promoteButton != null)
        {
            promoteButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{region.regionName} 홍보";
        }
    }

    // 홍보 버튼 클릭 시
    void OnPromoteButtonClick()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPromotionView(currentRegion);
            Debug.Log
                ("홍보 버튼2 클릭됨");
        }
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}