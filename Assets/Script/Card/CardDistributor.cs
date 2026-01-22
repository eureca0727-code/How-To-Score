using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDistributor : MonoBehaviour
{
    public PartyCardHand playerHand; // 플레이어
    public PartyCardHand partyBHand; // 을당
    public PartyCardHand partyCHand; // 병당

    // 각 지역별 특정 카드 매핑 (지역명 -> 카드ID)
    private Dictionary<string, string> regionCardMap;

    // 플레이어 카드 임시 저장소
    private List<string> pendingPlayerCards = new List<string>();

    void Awake()
    {
        InitializeRegionCardMap();
    }

    void InitializeRegionCardMap()
    {
        regionCardMap = new Dictionary<string, string>
        {
            { "지역 A", "M1" }, // 기자 회견 -> 기자 회견
            // { "지역 B", "S3" }, // 카드 없음
            { "지역 C", "M5" }, // 국회 -> 국회 중계
            { "지역 D", "M2" }, // 민단 선거사무 -> TV 인터뷰
            { "지역 E", "D2" }, // 감사 -> 경력 검증
            // { "지역 F", "A1" }, // 카드 없음
            { "지역 G", "M4" }, // 청소년 -> SNS
            { "지역 H", "M2" }, // 산업지구 -> TV 인터뷰
            // { "지역 I", "D1" }, // 카드 없음
            { "지역 J", "A2" }, // 팟캐스트 -> 비방전
            // { "지역 K", "M3" }, // 카드 없음
            // { "지역 L", "M5" }  // 카드 없음
        };
    }

    // 라운드 시작 시 양당 행동 카드 분배 (기존 카드에 추가)
    public void DistributeCardsForRound(int currentRound, List<Region> wonRegionsB, List<Region> wonRegionsC)
    {
        // 을당 카드 분배
        DistributeCardsForParty(partyBHand, currentRound, wonRegionsB, true);

        // 병당 카드 분배
        DistributeCardsForParty(partyCHand, currentRound, wonRegionsC, false);
    }

    void DistributeCardsForParty(PartyCardHand hand, int currentRound, List<Region> wonRegions, bool isPartyB)
    {
        // ★ ClearHand() 제거 - 기존 카드 유지
        List<string> newCards = new List<string>();

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
                    if (card != null && !newCards.Contains(cardId))
                    {
                        newCards.Add(cardId);
                    }
                }
            }
        }

        // 우선순위에 따라 정렬 후 6장 선택
        if (newCards.Count > 6)
        {
            newCards = SortAndLimitCards(newCards, isPartyB);
        }

        // 6장 미만일 경우 기본 카드 추가
        if (newCards.Count < 6)
        {
            if (isPartyB)
            {
                // 을당: A1, A2 우선
                while (newCards.Count < 6)
                {
                    if (!newCards.Contains("A1"))
                        newCards.Add("A1");
                    else if (!newCards.Contains("A2"))
                        newCards.Add("A2");
                    else
                        break;
                }
            }
            else
            {
                // 병당: D1, D2 우선
                while (newCards.Count < 6)
                {
                    if (!newCards.Contains("D1"))
                        newCards.Add("D1");
                    else if (!newCards.Contains("D2"))
                        newCards.Add("D2");
                    else
                        break;
                }
            }
        }

        // 그래도 6장 미만이면 M1~M5 랜덤 추가
        List<string> mCards = new List<string> { "M1", "M2", "M3", "M4", "M5" };
        while (newCards.Count < 6)
        {
            string randomM = mCards[Random.Range(0, mCards.Count)];
            newCards.Add(randomM);
        }

        // ★ 새로운 카드만 기존 핸드에 추가
        foreach (string cardId in newCards)
        {
            hand.AddCard(cardId);
        }
    }

    // ===== 플레이어 카드 수집 시스템 =====

    // 플레이어 카드 수집 초기화 (라운드 시작 시)
    public void InitializePlayerCardCollection()
    {
        if (pendingPlayerCards == null)
            pendingPlayerCards = new List<string>();
        else
            pendingPlayerCards.Clear();

        Debug.Log("플레이어 카드 수집 시작");
    }

    // 홍보 선택 카드 추가 (1장 또는 0장)
    public void AddPromotionCard(bool isNational, Region selectedRegion)
    {
        if (isNational)
        {
            // 전국 홍보: 전체 12장 중 랜덤
            string randomCard = GetRandomActionCard();
            pendingPlayerCards.Add(randomCard);
            Debug.Log($"전국 홍보 카드 추가: {randomCard}");
        }
        else
        {
            // 지역 홍보: regionCardMap에서 가져오기
            if (selectedRegion != null && regionCardMap.ContainsKey(selectedRegion.regionName))
            {
                string cardId = regionCardMap[selectedRegion.regionName];
                pendingPlayerCards.Add(cardId);
                Debug.Log($"지역 홍보 카드 추가: {selectedRegion.regionName} -> {cardId}");
            }
            else
            {
                // 카드 매핑이 없는 지역: 카드를 얻지 못함
                Debug.Log($"지역 '{selectedRegion?.regionName}'은 카드를 제공하지 않습니다.");
            }
        }
    }

    // 선거 운동 결과 카드 추가 (2-3장)
    public void AddCampaignResultCards(bool isSuccess)
    {
        int cardCount = isSuccess ? 3 : 2;
        Debug.Log($"선거 운동 {(isSuccess ? "성공" : "실패")} - 카드 {cardCount}장 추가");

        for (int i = 0; i < cardCount; i++)
        {
            string randomCard = GetRandomActionCard();
            pendingPlayerCards.Add(randomCard);
            Debug.Log($"  - {randomCard} 추가");
        }
    }

    // 의석 결과 카드 추가 (0-2장)
    public void AddSeatRewardCards(int partyASeats)
    {
        int cardCount = 0;

        if (partyASeats == 0)
            cardCount = 0;
        else if (partyASeats >= 1 && partyASeats <= 3)
            cardCount = 1;
        else if (partyASeats >= 4 && partyASeats <= 6)
            cardCount = 2;

        Debug.Log($"의석 결과: {partyASeats}석 -> 카드 {cardCount}장 추가");

        for (int i = 0; i < cardCount; i++)
        {
            string randomCard = GetRandomActionCard();
            pendingPlayerCards.Add(randomCard);
            Debug.Log($"  - {randomCard} 추가");
        }
    }

    // 최종 카드 확정 (기존 카드에 추가)
    public void FinalizePlayerCards()
    {
        if (playerHand == null)
        {
            Debug.LogError("playerHand가 null입니다!");
            return;
        }

        Debug.Log($"최종 카드 확정: 임시 저장소에 {pendingPlayerCards.Count}장");

        // playerHand에 새로운 카드만 추가
        foreach (string cardId in pendingPlayerCards)
        {
            playerHand.AddCard(cardId);
        }

        Debug.Log($"플레이어 신규 카드 ({pendingPlayerCards.Count}장): {string.Join(", ", pendingPlayerCards)}");

        // 임시 저장소 초기화
        pendingPlayerCards.Clear();
    }

    // 모든 행동 카드 중 랜덤 선택 (전국 홍보, 선거 운동, 의석 보상용)
    string GetRandomActionCard()
    {
        List<string> allActionCards = new List<string>
        {
            "M1", "M2", "M3", "M4", "M5",
            "A1", "A2",
            "D1", "D2",
            "S1", "S2", "S3"
        };
        return allActionCards[Random.Range(0, allActionCards.Count)];
    }

    // 우선순위에 따라 카드 정렬 후 6장 선택
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

        // 6장까지만 선택
        return result.Take(6).ToList();
    }
}