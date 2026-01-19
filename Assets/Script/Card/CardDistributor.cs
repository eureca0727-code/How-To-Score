using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDistributor : MonoBehaviour
{
    public PartyCardHand playerHand; // 플레이어
    public PartyCardHand partyBHand; // 을
    public PartyCardHand partyCHand; // 병

    // 각 지역별 특정 카드 매핑 (지역명 -> 카드ID)
    private Dictionary<string, string> regionCardMap;

    void Awake()
    {
        InitializeRegionCardMap();
    }

    void InitializeRegionCardMap()
    {
        regionCardMap = new Dictionary<string, string> //특정 지역이 카드 획득 없을시 리스트에서 지울것
        {
            { "지역 A", "M1" }, // 기자 회견 -> 기자 회견
            { "지역 B", "S3" }, // 환경전망 -> (환경 이슈, 특정적 이슈)
            { "지역 C", "M5" }, // 국회 -> 국회 중계
            { "지역 D", "M2" }, // 민단 선거사무 -> TV 인터뷰
            { "지역 E", "D2" }, // 감사 -> 경력 검증
            { "지역 F", "A1" }, // 교육 -> 캠페인 네거티브
            { "지역 G", "M4" }, // 청소년 -> SNS
            { "지역 H", "M2" }, // 산업지구 -> TV 인터뷰
            { "지역 I", "D1" }, // 은퇴자 -> 정책 공약서 자료집
            { "지역 J", "A2" }, // 팟캐스트 -> 비방전
            { "지역 K", "M3" }, // 상업 -> 토론회
            { "지역 L", "M5" }  // 의료 -> 국회 중계
        };
    }

    // 라운드 시작 시 양당 행동 카드 분배
    public void DistributeCardsForRound(int currentRound, List<Region> wonRegionsB, List<Region> wonRegionsC)
    {
        // 을 카드 분배
        DistributeCardsForParty(partyBHand, currentRound, wonRegionsB, true);

        // 병 카드 분배
        DistributeCardsForParty(partyCHand, currentRound, wonRegionsC, false);
    }

    void DistributeCardsForParty(PartyCardHand hand, int currentRound, List<Region> wonRegions, bool isPartyB)
    {
        hand.ClearHand();
        List<string> cards = new List<string>();

        // 2~5라운드 시, 직전 라운드에서 승리한 지역마다 해당 특정 카드 추가
        if (currentRound >= 2 && wonRegions != null && wonRegions.Count > 0)
        {
            foreach (Region region in wonRegions)
            {
                if (regionCardMap.ContainsKey(region.regionName))
                {
                    string cardId = regionCardMap[region.regionName];
                    Card card = CardDatabase.Instance.GetCard(cardId);

                    // 행동카드(M, A, S, D)만 추가, 중복 체크
                    if (card != null && !cards.Contains(cardId))
                    {
                        cards.Add(cardId);
                    }
                }
            }
        }

        // 우선순위에 따라 정렬 후 6장 선택
        if (cards.Count > 6)
        {
            cards = SortAndLimitCards(cards, isPartyB);
        }

        // 6장 미만일 경우 기본 카드 추가
        if (cards.Count < 6)
        {
            if (isPartyB)
            {
                // 을: A1, A2 우선
                while (cards.Count < 6)
                {
                    if (!cards.Contains("A1"))
                        cards.Add("A1");
                    else if (!cards.Contains("A2"))
                        cards.Add("A2");
                    else
                        break;
                }
            }
            else
            {
                // 병: D1, D2 우선
                while (cards.Count < 6)
                {
                    if (!cards.Contains("D1"))
                        cards.Add("D1");
                    else if (!cards.Contains("D2"))
                        cards.Add("D2");
                    else
                        break;
                }
            }
        }

        // 그래도 6장 미만이면 M1~M5 랜덤 추가
        List<string> mCards = new List<string> { "M1", "M2", "M3", "M4", "M5" };
        while (cards.Count < 6)
        {
            string randomM = mCards[Random.Range(0, mCards.Count)];
            cards.Add(randomM);
        }

        // 카드 핸드에 추가
        foreach (string cardId in cards)
        {
            hand.AddCard(cardId);
        }
    }

    // 우선순위에 따라 카드 정렬 후 6장 선택
    List<string> SortAndLimitCards(List<string> cards, bool isPartyB)
    {
        List<string> result = new List<string>();

        if (isPartyB)
        {
            // 을 우선순위: M > A > S > D
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
        }
        else
        {
            // 병 우선순위: S > M > D > A
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
        }

        // 6장까지만 선택
        return result.Take(6).ToList();
    }
}
