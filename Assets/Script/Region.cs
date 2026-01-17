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
    public int districtId; // 선거구 ID (0=A, 1=B, 2=C)

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
        int minSupport = 15;

        // A~F 지역은 갑당 지지도 상한 40%, 최저 15%, 그외는 최저 15 최대 70
        int maxA = (regionName == "A" || regionName == "B" || regionName == "C" ||
                    regionName == "D" || regionName == "E" || regionName == "F") ? 40 : 100;

        int remaining = 100 - (minSupport * 3); // 55

        // 갑당의 추가 지지율 (상한 고려)
        int maxExtraA = Mathf.Min(remaining, maxA - minSupport);
        int extraA = UnityEngine.Random.Range(0, maxExtraA + 1);

        int remainingAfterA = remaining - extraA;

        // 을당 (남은 범위에서)
        int maxExtraB = Mathf.Min(remainingAfterA, 100 - minSupport);
        int extraB = UnityEngine.Random.Range(0, maxExtraB + 1);

        int extraC = remaining - extraA - extraB;

        partyA.supportRate = minSupport + extraA;
        partyB.supportRate = minSupport + extraB;
        partyC.supportRate = minSupport + extraC;
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
        ("경제", GetEconomyDemand()),
        ("복지", GetWelfareDemand()),
        ("안보", GetSecurityDemand()),
        ("환경", GetEnvironmentDemand())
    };

        // 수요도 높은 순으로 정렬
        policies.Sort((a, b) => b.demand.CompareTo(a.demand));

        // 이름만 반환 (값 제외)
        return $"{policies[0].name}, {policies[1].name}";
    }

    // 세 정당의 지지도를 직접 변경 
public void ChangeSupportRate(int amountA, int amountB, int amountC)
    {
        // 변화량 합이 0인지 확인
        if (amountA + amountB + amountC != 0)
        {
            Debug.LogError($"지지도 변화량 합이 0이 아닙니다! (갑:{amountA}, 을:{amountB}, 병:{amountC})");
            return;
        }

        partyA.supportRate += amountA;
        partyB.supportRate += amountB;
        partyC.supportRate += amountC;

        // 0~100 범위 제한
        partyA.supportRate = Mathf.Clamp(partyA.supportRate, 0, 100);
        partyB.supportRate = Mathf.Clamp(partyB.supportRate, 0, 100);
        partyC.supportRate = Mathf.Clamp(partyC.supportRate, 0, 100);
    }


}