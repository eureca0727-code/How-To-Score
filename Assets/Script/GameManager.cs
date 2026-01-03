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

        // 이름순 정렬
        System.Array.Sort(allRegions, (a, b) =>
            string.Compare(a.regionName, b.regionName));
    }

    void Start()
    {
        InitializeGame();

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

        foreach (var region in allRegions)
        {
            region.UpdateRoundDemands();
        }

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
    public int GetTotalPartyASupport()
    {
        int total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyA.supportRate) / 100;
        }
        return total;
    }

    public int GetTotalPartyBSupport()
    {
        int total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyB.supportRate) / 100;
        }
        return total;
    }

    public int GetTotalPartyCSupport()
    {
        int total = 0;
        foreach (var region in allRegions)
        {
            total += (region.population * region.partyC.supportRate) / 100;
        }
        return total;
    }

}