using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mapContainer; // Inspector에서 MapContainer 연결
    private Region[] allRegions;
    private int currentRound = 1;

    void Awake()
    {
        Instance = this;

        // MapContainer의 자식들에서 Region 컴포넌트 가져오기
        allRegions = mapContainer.GetComponentsInChildren<Region>();
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        DistributePopulation();

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

        Debug.Log("라운드 " + currentRound + " 시작");
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
}