using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;

    [Header("Charts")]
    public MultiSectionPieChart partyChart;        // 갑/을/병 지지도
    public MultiSectionPieChart partyA_PolicyChart; // 갑 지지자 정책 수요
    public MultiSectionPieChart partyB_PolicyChart; // 을 지지자 정책 수요
    public MultiSectionPieChart partyC_PolicyChart; // 병 지지자 정책 수요

    private bool isNationalMode = true;
    private Region selectedRegion = null;

    // Start Promotion 버튼에서 호출
    public void ShowPromotionPanel()
    {
        // promotionPanel.SetActive(true); // 이미 UIManager에서 활성화했으니 필요없음
        UpdateCharts();
    }

    // 전국 모드로 설정 
    public void SetNationalMode()
    {
        isNationalMode = true;
        selectedRegion = null;
    }

    // 지역 모드로 설정 
    public void SetRegionalMode(Region region)
    {
        isNationalMode = false;
        selectedRegion = region;
    }

    void UpdateCharts()
    {
        if (isNationalMode)
        {
            ShowNationalData();
        }
        else
        {
            ShowRegionalData();
        }
    }

    void ShowNationalData()
    {
        titleText.text = "Nation Rating";

        // 1. 전국 갑/을/병 지지도
        int totalA = GameManager.Instance.GetTotalPartyASupport();
        int totalB = GameManager.Instance.GetTotalPartyBSupport();
        int totalC = GameManager.Instance.GetTotalPartyCSupport();
        partyChart.SetData3(totalA, totalB, totalC);

        // 2. 갑 지지자들의 정책 수요 (전국 평균)
        var partyA_Demand = CalculateNationalPolicyDemand("A");
        partyA_PolicyChart.SetData4(
            partyA_Demand.economy,
            partyA_Demand.welfare,
            partyA_Demand.security,
            partyA_Demand.environment
        );

        // 3. 을 지지자들의 정책 수요
        var partyB_Demand = CalculateNationalPolicyDemand("B");
        partyB_PolicyChart.SetData4(
            partyB_Demand.economy,
            partyB_Demand.welfare,
            partyB_Demand.security,
            partyB_Demand.environment
        );

        // 4. 병 지지자들의 정책 수요
        var partyC_Demand = CalculateNationalPolicyDemand("C");
        partyC_PolicyChart.SetData4(
            partyC_Demand.economy,
            partyC_Demand.welfare,
            partyC_Demand.security,
            partyC_Demand.environment
        );
    }

    void ShowRegionalData()
    {
        if (selectedRegion == null)
        {
            Debug.LogError("선택된 지역이 없습니다!");
            return;
        }

        titleText.text = "Regional Rating";

        // 1. 해당 지역 갑/을/병 지지도
        Debug.Log($"=== {selectedRegion.regionName} 지지도 ===");
        Debug.Log($"갑: {selectedRegion.partyA.supportRate}%");
        Debug.Log($"을: {selectedRegion.partyB.supportRate}%");
        Debug.Log($"병: {selectedRegion.partyC.supportRate}%");

        partyChart.SetData3(
            selectedRegion.partyA.supportRate,
            selectedRegion.partyB.supportRate,
            selectedRegion.partyC.supportRate
        );

        // 2. 갑 정책 수요
        Debug.Log($"=== 갑 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyA.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyA.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyA.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyA.policyDemand.environment}");

        partyA_PolicyChart.SetData4(
            selectedRegion.partyA.policyDemand.economy,
            selectedRegion.partyA.policyDemand.welfare,
            selectedRegion.partyA.policyDemand.security,
            selectedRegion.partyA.policyDemand.environment
        );

        // 3. 을 정책 수요
        Debug.Log($"=== 을 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyB.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyB.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyB.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyB.policyDemand.environment}");

        partyB_PolicyChart.SetData4(
            selectedRegion.partyB.policyDemand.economy,
            selectedRegion.partyB.policyDemand.welfare,
            selectedRegion.partyB.policyDemand.security,
            selectedRegion.partyB.policyDemand.environment
        );

        // 4. 병 정책 수요
        Debug.Log($"=== 병 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyC.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyC.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyC.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyC.policyDemand.environment}");

        partyC_PolicyChart.SetData4(
            selectedRegion.partyC.policyDemand.economy,
            selectedRegion.partyC.policyDemand.welfare,
            selectedRegion.partyC.policyDemand.security,
            selectedRegion.partyC.policyDemand.environment
        );
    }



    // 전국 정당별 정책 수요 평균 계산
    PolicyDemand CalculateNationalPolicyDemand(string party)
    {
        Region[] allRegions = GameManager.Instance.mapContainer.GetComponentsInChildren<Region>();

        int totalEconomy = 0;
        int totalWelfare = 0;
        int totalSecurity = 0;
        int totalEnvironment = 0;
        int totalSupport = 0;

        foreach (var region in allRegions)
        {
            int support = 0;
            PolicyDemand demand = null;

            switch (party)
            {
                case "A":
                    support = (region.population * region.partyA.supportRate) / 100;
                    demand = region.partyA.policyDemand;
                    break;
                case "B":
                    support = (region.population * region.partyB.supportRate) / 100;
                    demand = region.partyB.policyDemand;
                    break;
                case "C":
                    support = (region.population * region.partyC.supportRate) / 100;
                    demand = region.partyC.policyDemand;
                    break;
            }

            if (support > 0 && demand != null)
            {
                totalEconomy += demand.economy * support;
                totalWelfare += demand.welfare * support;
                totalSecurity += demand.security * support;
                totalEnvironment += demand.environment * support;
                totalSupport += support;
            }
        }

        PolicyDemand avgDemand = new PolicyDemand();
        if (totalSupport > 0)
        {
            avgDemand.economy = totalEconomy / totalSupport;
            avgDemand.welfare = totalWelfare / totalSupport;
            avgDemand.security = totalSecurity / totalSupport;
            avgDemand.environment = totalEnvironment / totalSupport;
        }

        return avgDemand;
    }

    public void ClosePromotionPanel()
    {
        gameObject.SetActive(false); // promotionPanel 대신 gameObject
    }
}