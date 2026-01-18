using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDistributor : MonoBehaviour
{
    public PartyCardHand playerHand; // 갑당 (플레이어)
    public PartyCardHand partyBHand; // 을당
    public PartyCardHand partyCHand; // 병당

    // 각 지역의 특성 카드 매핑 (지역명 -> 카드ID)
    private Dictionary<string, string> regionCardMap;

    void Awake()
    {
        InitializeRegionCardMap();
    }

    void InitializeRegionCardMap()
    {
        regionCardMap = new Dictionary<string, string>
        {
            { "지역 A", "M1" }, // 대형 언론 -> 언론 기사
            { "지역 B", "S3" }, // 국립공원 -> (환경 관련, 특수로 가정)
            { "지역 C", "M5" }, // 의회 -> 의회 발언
            { "지역 D", "M2" }, // 거대 방송사 -> TV 인터뷰
            { "지역 E", "D2" }, // 법원 -> 법적 대응
            { "지역 F", "A1" }, // 대기업 -> 캠페인 슬로건
            { "지역 G", "M4" }, // 청년층 -> SNS
            { "지역 H", "M2" }, // 공영방송 -> TV 인터뷰
            { "지역 I", "D1" }, // 노인층 -> 공식 선거운동 자료집
            { "지역 J", "A2" }, // 유동인구 -> 현수막
            { "지역 K", "M3" }, // 농업 -> 토론회
            { "지역 L", "M5" }  // 국경 -> 의회 발언
        };
    }

    // 라운드 시작 시 상대 정당 카드 배분
    public void DistributeCardsForRound(int currentRound, List<Region> wonRegionsB, List<Region> wonRegionsC)
    {
        // 을당 카드 배분
        DistributeCardsForParty(partyBHand, currentRound, wonRegionsB, true);

        // 병당 카드 배분
        DistributeCardsForParty(partyCHand, currentRound, wonRegionsC, false);
    }

    void DistributeCardsForParty(PartyCardHand hand, int currentRound, List<Region> wonRegions, bool isPartyB)
    {
        hand.ClearHand();
        List<string> cards = new List<string>();

        // 2~5라운드일 경우, 이전 턴에 승리한 선거구의 지역 특성 카드 추가
        if (currentRound >= 2 && wonRegions != null && wonRegions.Count > 0)
        {
            foreach (Region region in wonRegions)
            {
                if (regionCardMap.ContainsKey(region.regionName))
                {
                    string cardId = regionCardMap[region.regionName];
                    Card card = CardDatabase.Instance.GetCard(cardId);

                    // 행동카드(M, A, S, D)만 추가
                    if (card != null)
                    {
                        cards.Add(cardId);
                    }
                }
            }
        }

        // 우선순위에 따라 정렬 및 6장 제한
        if (cards.Count > 6)
        {
            cards = SortAndLimitCards(cards, isPartyB);
        }

        // 6장 미만일 경우 기본 카드 추가
        if (cards.Count < 6)
        {
            if (isPartyB)
            {
                // 을당: A1, A2 우선
                while (cards.Count < 6 && cards.Count < 2)
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
                // 병당: D1, D2 우선
                while (cards.Count < 6 && cards.Count < 2)
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

        // 여전히 6장 미만이면 M1~M5 랜덤 추가
        List<string> mCards = new List<string> { "M1", "M2", "M3", "M4", "M5" };
        while (cards.Count < 6)
        {
            string randomM = mCards[Random.Range(0, mCards.Count)];
            cards.Add(randomM);
        }

        // 카드 손에 추가
        foreach (string cardId in cards)
        {
            hand.AddCard(cardId);
        }
    }

    // 우선순위에 따라 카드 정렬 및 6장 제한
    List<string> SortAndLimitCards(List<string> cards, bool isPartyB)
    {
        List<string> result = new List<string>();

        if (isPartyB)
        {
            // 을당 우선순위: M > A > S > D
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
        }
        else
        {
            // 병당 우선순위: S > M > D > A
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
        }

        // 6장으로 제한
        return result.Take(6).ToList();
    }
}