using System;
using UnityEngine;

// 정책 수요 데이터
[Serializable]
public class PolicyDemand
{
    public int economy;    // 경제
    public int welfare;    // 복지
    public int security;   // 안보
    public int environment; // 환경


    public void RandomizeDemands()
    {
        economy = UnityEngine.Random.Range(0, 11);
        welfare = UnityEngine.Random.Range(0, 11);
        security = UnityEngine.Random.Range(0, 11);
        environment = UnityEngine.Random.Range(0, 11);
    }
}

// 정당별 데이터
[Serializable]
public class PartyData
{
    public int supportRate;        // 지지율 (%)
    public PolicyDemand policyDemand; // 정책 수요

}

// 지역 데이터
public class Region : MonoBehaviour
{
    [Header("Basic Info")]
    public string regionName;
    public int population; // 5~15

    [Header("Party Support")]
    public PartyData partyA; // 갑
    public PartyData partyB; // 을
    public PartyData partyC; // 병

    [Header("District Info")]
    public int districtId; // 선거구 ID (0=A, 1=B, ...)

    // 게임 시작 시 1회 초기화
    public void InitializeRegion()
    {
        // 인구는 GameManager에서 총합 120 맞춰서 할당

        // 지지율 랜덤 생성 (합 100)
        RandomizeSupportRates();

        // 정책 수요 랜덤 생성
        partyA.policyDemand.RandomizeDemands();
        partyB.policyDemand.RandomizeDemands();
        partyC.policyDemand.RandomizeDemands();


    }


    void RandomizeSupportRates()
    {
        // 합이 100이 되도록
        partyA.supportRate = UnityEngine.Random.Range(0, 101);
        partyB.supportRate = UnityEngine.Random.Range(0, 101 - partyA.supportRate);
        partyC.supportRate = 100 - partyA.supportRate - partyB.supportRate;
    }

    void OnMouseDown()
    {
        // UI 클릭이 아닐 때만 실행
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RegionInfoUI.Instance.ShowRegionInfo(this);
    }

    // 각 정책의 전체 수요도 계산
    public float GetEconomyDemand()
    {
        return (partyA.policyDemand.economy * partyA.supportRate +
                partyB.policyDemand.economy * partyB.supportRate +
                partyC.policyDemand.economy * partyC.supportRate) / 100f;
    }
    
    public float GetWelfareDemand()
    {
        return (partyA.policyDemand.welfare * partyA.supportRate +
                partyB.policyDemand.welfare * partyB.supportRate +
                partyC.policyDemand.welfare * partyC.supportRate) / 100f;
    }
    
    public float GetSecurityDemand()
    {
        return (partyA.policyDemand.security * partyA.supportRate +
                partyB.policyDemand.security * partyB.supportRate +
                partyC.policyDemand.security * partyC.supportRate) / 100f;
    }
    
    public float GetEnvironmentDemand()
    {
        return (partyA.policyDemand.environment * partyA.supportRate +
                partyB.policyDemand.environment * partyB.supportRate +
                partyC.policyDemand.environment * partyC.supportRate) / 100f;
    }

    // 상위 2개 정책 분야 찾기
    public string GetTop2Policies()
    {
        var policies = new System.Collections.Generic.List<(string name, float demand)>
    {
        ("Economy", GetEconomyDemand()),
        ("Welfare", GetWelfareDemand()),
        ("Security", GetSecurityDemand()),
        ("Environment", GetEnvironmentDemand())
    };

        // 수요도 높은 순으로 정렬
        policies.Sort((a, b) => b.demand.CompareTo(a.demand));

        // 이름만 반환 (값 제외)
        return $"{policies[0].name}, {policies[1].name}";
    }


}