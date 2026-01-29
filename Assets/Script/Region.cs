using System;
using UnityEngine;

// 정책 수요 클래스
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

// 지역 클래스
public class Region : MonoBehaviour
{
    [Header("Basic Info")]
    public string regionName;
    public int population; // 5~15
    public string regionDescription;


    [Header("Party Support")]
    public PartyData partyA; // 갑
    public PartyData partyB; // 을
    public PartyData partyC; // 병

    [Header("District Info")]
    public int districtId; // 선거구 ID (0=A, 1=B, 2=C)

    // 게임 시작 시 1회 초기화
    public void InitializeRegion()
    {
        // 인구수 GameManager에서 총합 120 맞춰서 할당

        // 지지율 랜덤 설정 (총 100)
        RandomizeSupportRates();

        // 정책 수요 랜덤 생성
        partyA.policyDemand.RandomizeDemands();
        partyB.policyDemand.RandomizeDemands();
        partyC.policyDemand.RandomizeDemands();


    }


    void RandomizeSupportRates()
    {
        int minSupport = 15;

        // A~F 지역은 갑의 지지율 최대 40%, 나머지 15%, 그외는 최소 15 최대 70
        int maxA = (regionName == "A" || regionName == "B" || regionName == "C" ||
                    regionName == "D" || regionName == "E" || regionName == "F") ? 40 : 100;

        int remaining = 100 - (minSupport * 3); // 55

        // 갑에게 추가 지지율 (제한 적용)
        int maxExtraA = Mathf.Min(remaining, maxA - minSupport);
        int extraA = UnityEngine.Random.Range(0, maxExtraA + 1);

        int remainingAfterA = remaining - extraA;

        // 을에게 (최대 제한없음)
        int maxExtraB = Mathf.Min(remainingAfterA, 100 - minSupport);
        int extraB = UnityEngine.Random.Range(0, maxExtraB + 1);

        int extraC = remaining - extraA - extraB;

        partyA.supportRate = minSupport + extraA;
        partyB.supportRate = minSupport + extraB;
        partyC.supportRate = minSupport + extraC;
    }
    void OnMouseDown()
    {
        // UI 클릭이 아닌 경우 처리
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RegionInfoUI.Instance.ShowRegionInfo(this);
    }

    // 각 정책의 전체 선호도 계산
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

    // 가장 2개 정책 우선 찾기
    public string GetTop2Policies()
    {
        var policies = new System.Collections.Generic.List<(string name, float demand)>
    {
        ("경제", GetEconomyDemand()),
        ("복지", GetWelfareDemand()),
        ("안보", GetSecurityDemand()),
        ("환경", GetEnvironmentDemand())
    };

        // 선호도 높은 순서로 정렬
        policies.Sort((a, b) => b.demand.CompareTo(a.demand));

        // 이름만 반환 (상위 2개)
        return $"{policies[0].name}, {policies[1].name}";
    }

    // 각 정당의 지지율을 동시에 변경
    public void ChangeSupportRate(int amountA, int amountB, int amountC)
    {
        // 변화량 합이 0인지 확인
        if (amountA + amountB + amountC != 0)
        {
            Debug.LogError($"지지율 변화량 합이 0이 아닙니다! (갑:{amountA}, 을:{amountB}, 병:{amountC})");
            return;
        }

        // 임시로 변화량 적용
        float tempA = partyA.supportRate + amountA;
        float tempB = partyB.supportRate + amountB;
        float tempC = partyC.supportRate + amountC;

        // 음수 처리: 0 미만으로 떨어진 값 보정
        float excessA = Mathf.Min(0, tempA); // 음수면 그 값, 0 이상이면 0
        float excessB = Mathf.Min(0, tempB);
        float excessC = Mathf.Min(0, tempC);

        float totalExcess = -(excessA + excessB + excessC); // 음수를 양수로 전환

        // 0으로 보정
        tempA = Mathf.Max(0, tempA);
        tempB = Mathf.Max(0, tempB);
        tempC = Mathf.Max(0, tempC);

        // 초과분을 양수인 정당들에게 비례 배분
        if (totalExcess > 0)
        {
            // 양수인 정당들의 변화량 합계
            float positiveSum = 0;
            if (tempA > partyA.supportRate) positiveSum += (tempA - partyA.supportRate);
            if (tempB > partyB.supportRate) positiveSum += (tempB - partyB.supportRate);
            if (tempC > partyC.supportRate) positiveSum += (tempC - partyC.supportRate);

            // 비례 배분 (양수 변화가 있는 정당들에게만)
            if (positiveSum > 0)
            {
                if (tempA > partyA.supportRate)
                {
                    float ratio = (tempA - partyA.supportRate) / positiveSum;
                    tempA -= totalExcess * ratio;
                }
                if (tempB > partyB.supportRate)
                {
                    float ratio = (tempB - partyB.supportRate) / positiveSum;
                    tempB -= totalExcess * ratio;
                }
                if (tempC > partyC.supportRate)
                {
                    float ratio = (tempC - partyC.supportRate) / positiveSum;
                    tempC -= totalExcess * ratio;
                }
            }
            else
            {
                // 모든 정당이 감소하는 경우: 초과분을 균등 배분
                tempA += totalExcess / 3f;
                tempB += totalExcess / 3f;
                tempC += totalExcess / 3f;
            }
        }

        // 최종 적용 (반올림 후 int로 변환)
        partyA.supportRate = Mathf.RoundToInt(tempA);
        partyB.supportRate = Mathf.RoundToInt(tempB);
        partyC.supportRate = Mathf.RoundToInt(tempC);

        // 반올림 오차 보정 (합이 정확히 100이 되도록)
        int total = partyA.supportRate + partyB.supportRate + partyC.supportRate;
        if (total != 100)
        {
            int diff = 100 - total;
            // 가장 큰 정당에게 오차 할당
            if (partyA.supportRate >= partyB.supportRate && partyA.supportRate >= partyC.supportRate)
                partyA.supportRate += diff;
            else if (partyB.supportRate >= partyC.supportRate)
                partyB.supportRate += diff;
            else
                partyC.supportRate += diff;
        }

        // 최종 0~100 범위 제한
        partyA.supportRate = Mathf.Clamp(partyA.supportRate, 0, 100);
        partyB.supportRate = Mathf.Clamp(partyB.supportRate, 0, 100);
        partyC.supportRate = Mathf.Clamp(partyC.supportRate, 0, 100);

        Debug.Log($"{regionName} 지지율 변경 후: 갑({partyA.supportRate}%), 을({partyB.supportRate}%), 병({partyC.supportRate}%) = {partyA.supportRate + partyB.supportRate + partyC.supportRate}%");
    }
}
