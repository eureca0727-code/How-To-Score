using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mapContainer; // Inspector에서 MapContainer 연결
    private Region[] allRegions;
    private int currentRound = 1;
    private const int MAX_ROUNDS = 5; // 최대 라운드

    void Awake()
    {
        Instance = this;

        allRegions = mapContainer.GetComponentsInChildren<Region>();

        // 이름순 정렬(구역나누기용도)
        System.Array.Sort(allRegions, (a, b) =>
            string.Compare(a.regionName, b.regionName));
    }

    void Start()
    {
        InitializeGame();
        // 지역 초기화 후 UI 업데이트
        if (RegionInfoUI.Instance != null)
        {
            RegionInfoUI.Instance.ShowOverallStats();
        }


    }

    void InitializeGame()
    {
        DistributePopulation();

        // 선거구 랜덤 할당
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
            // 최종 결과 화면으로
        }
    }


    void DistributePopulation()
    {
        // 각 지역에 최소 5씩
        foreach (var region in allRegions)
        {
            region.population = 5;
        }
        // 남은 60을 랜덤 배분
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
    // 전체 정당 지지자 수 계산
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

    // 전국 모든 지역의 지지도를 변경
    public void ChangeAllRegionsSupport(int amountA, int amountB, int amountC)
    {
        // 변화량 합이 0인지 확인
        if (amountA + amountB + amountC != 0)
        {
            Debug.LogError($"전국 지지도 변화량 합이 0이 아닙니다! (갑:{amountA}, 을:{amountB}, 병:{amountC})");
            return;
        }

        // 모든 지역에 동일한 변화 적용
        foreach (var region in allRegions)
        {
            region.ChangeSupportRate(amountA, amountB, amountC);
        }

        Debug.Log($"전국 지지도 변경: 갑({amountA:+#;-#;0}), 을({amountB:+#;-#;0}), 병({amountC:+#;-#;0})");
    }
    // 특정 선거구의 정당별 지지율 계산
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

            // 각 지역의 정당별 지지자 수 = 인구수 * 지지율 / 100
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
}
