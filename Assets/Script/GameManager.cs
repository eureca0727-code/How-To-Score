using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mapContainer; // Inspector에서 MapContainer 설정
    private Region[] allRegions;
    private int currentRound = 1;
    private const int MAX_ROUNDS = 5; // 최대 라운드

    void Awake()
    {
        Instance = this;

        allRegions = mapContainer.GetComponentsInChildren<Region>();

        // 이름별 정렬(가나다라마바사아자차카타파하 순)
        System.Array.Sort(allRegions, (a, b) =>
            string.Compare(a.regionName, b.regionName));
    }

    void Start()
    {
        InitializeGame();
        // 게임 초기화 후 UI 업데이트
        if (RegionInfoUI.Instance != null)
        {
            RegionInfoUI.Instance.ShowOverallStats();
        }


    }

    void InitializeGame()
    {
        DistributePopulation();

        // 지역별 선거구 할당
        DistrictManager.Instance.AssignRandomDistricts(allRegions);


        foreach (var region in allRegions)
        {
            region.InitializeRegion();
        }
    }
    public int GetCurrentRound()
    {
        return currentRound;
    }

    public int GetMaxRounds()
    {
        return MAX_ROUNDS;
    }

    public bool IsGameOver()
    {
        return currentRound >= MAX_ROUNDS;
    }

    public void StartNewRound()
    {
        if (currentRound < MAX_ROUNDS)
        {
            currentRound++;
            Debug.Log($"라운드 {currentRound} 시작!");
        }
        else
        {
            Debug.Log("게임 종료!");
            // 게임 종료 화면으로
        }
    }


    void DistributePopulation()
    {
        // 각 지역에 최소 5명
        foreach (var region in allRegions)
        {
            region.population = 5;
        }
        // 나머지 60명 랜덤 분배
        int remaining = 60;
        while (remaining > 0)
        {
            Region randomRegion = allRegions[Random.Range(0, 12)];
            if (randomRegion.population < 15)
            {
                randomRegion.population++;
                remaining--;
            }
        }
    }
    // 전체 정당 지지율 총합 계산
    public float GetTotalPartyASupport()
    {
        float total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyA.supportRate) / 100f;
        }
        return total;
    }

    public float GetTotalPartyBSupport()
    {
        float total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyB.supportRate) / 100f;
        }
        return total;
    }

    public float GetTotalPartyCSupport()
    {
        float total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyC.supportRate) / 100f;
        }
        return total;
    }
    // PromotionUI에서 사용하는 전국 지지율 Getter
    public float GetNationalSupportRateA()
    {
        return GetTotalPartyASupport();
    }

    public float GetNationalSupportRateB()
    {
        return GetTotalPartyBSupport();
    }

    public float GetNationalSupportRateC()
    {
        return GetTotalPartyCSupport();
    }


    // 모든 지역 지지율을 동시에 변경
    public void ChangeAllRegionsSupport(int amountA, int amountB, int amountC)
    {
        // 변화량 합이 0인지 확인
        if (amountA + amountB + amountC != 0)
        {
            Debug.LogError($"지지율 변화량 합이 0이 아닙니다! (갑:{amountA}, 을:{amountB}, 병:{amountC})");
            return;
        }

        // 모든 지역에 지지율 변화 적용
        foreach (var region in allRegions)
        {
            region.ChangeSupportRate(amountA, amountB, amountC);
        }

        Debug.Log($"전체 지지율 변경: 갑({amountA:+#;-#;0}), 을({amountB:+#;-#;0}), 병({amountC:+#;-#;0})");
    }
    // 특정 선거구의 가중평균 지지율 계산
    public (float partyA, float partyB, float partyC) GetDistrictSupport(int districtId)
    {
        // 해당 선거구에 속한 지역들 찾기
        Region[] districtRegions = System.Array.FindAll(allRegions,
            region => region.districtId == districtId);

        if (districtRegions.Length == 0)
        {
            Debug.LogError($"선거구 {districtId}에 지역이 없습니다!");
            return (0, 0, 0);
        }

        // 총 인구수
        int totalPopulation = 0;

        // 각 정당의 지지자 수
        float totalSupportA = 0;
        float totalSupportB = 0;
        float totalSupportC = 0;

        foreach (var region in districtRegions)
        {
            totalPopulation += region.population;

            // 각 지역의 가중된 지지자 수 = 인구수 * 지지율 / 100
            totalSupportA += region.population * region.partyA.supportRate / 100f;
            totalSupportB += region.population * region.partyB.supportRate / 100f;
            totalSupportC += region.population * region.partyC.supportRate / 100f;
        }

        // 선거구 전체 지지율 = 지지자 수 / 총 인구수 * 100
        float districtSupportA = (totalSupportA / totalPopulation) * 100f;
        float districtSupportB = (totalSupportB / totalPopulation) * 100f;
        float districtSupportC = (totalSupportC / totalPopulation) * 100f;

        Debug.Log($"선거구 {districtId}: 갑 {districtSupportA:F1}%, 을 {districtSupportB:F1}%, 병 {districtSupportC:F1}%");

        return (districtSupportA, districtSupportB, districtSupportC);
    }
    // 전국 정책 수요 계산 (정당별 지지율 가중 평균)
    public PolicyDemand GetNationalPolicyDemandForPartyA()
    {
        PolicyDemand total = new PolicyDemand();
        float totalSupport = 0;

        foreach (var region in allRegions)
        {
            float weight = region.partyA.supportRate;
            totalSupport += weight;

            total.economy += region.partyA.policyDemand.economy * (int)weight;
            total.welfare += region.partyA.policyDemand.welfare * (int)weight;
            total.security += region.partyA.policyDemand.security * (int)weight;
            total.environment += region.partyA.policyDemand.environment * (int)weight;
        }

        if (totalSupport > 0)
        {
            total.economy = (int)(total.economy / totalSupport);
            total.welfare = (int)(total.welfare / totalSupport);
            total.security = (int)(total.security / totalSupport);
            total.environment = (int)(total.environment / totalSupport);
        }

        return total;
    }

    public PolicyDemand GetNationalPolicyDemandForPartyB()
    {
        PolicyDemand total = new PolicyDemand();
        float totalSupport = 0;

        foreach (var region in allRegions)
        {
            float weight = region.partyB.supportRate;
            totalSupport += weight;

            total.economy += region.partyB.policyDemand.economy * (int)weight;
            total.welfare += region.partyB.policyDemand.welfare * (int)weight;
            total.security += region.partyB.policyDemand.security * (int)weight;
            total.environment += region.partyB.policyDemand.environment * (int)weight;
        }

        if (totalSupport > 0)
        {
            total.economy = (int)(total.economy / totalSupport);
            total.welfare = (int)(total.welfare / totalSupport);
            total.security = (int)(total.security / totalSupport);
            total.environment = (int)(total.environment / totalSupport);
        }

        return total;
    }

    public PolicyDemand GetNationalPolicyDemandForPartyC()
    {
        PolicyDemand total = new PolicyDemand();
        float totalSupport = 0;

        foreach (var region in allRegions)
        {
            float weight = region.partyC.supportRate;
            totalSupport += weight;

            total.economy += region.partyC.policyDemand.economy * (int)weight;
            total.welfare += region.partyC.policyDemand.welfare * (int)weight;
            total.security += region.partyC.policyDemand.security * (int)weight;
            total.environment += region.partyC.policyDemand.environment * (int)weight;
        }

        if (totalSupport > 0)
        {
            total.economy = (int)(total.economy / totalSupport);
            total.welfare = (int)(total.welfare / totalSupport);
            total.security = (int)(total.security / totalSupport);
            total.environment = (int)(total.environment / totalSupport);
        }

        return total;
    }
}

