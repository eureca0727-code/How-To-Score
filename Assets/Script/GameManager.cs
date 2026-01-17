using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mapContainer; // Inspector에서 MapContainer 연결
    private Region[] allRegions;
    private int currentRound = 1; //아마 10라운드 까지?

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

    public void StartNewRound()
    {
        currentRound++;


        Debug.Log("라운드 " + currentRound);
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
}
