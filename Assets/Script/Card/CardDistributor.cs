using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDistributor : MonoBehaviour
{
    public PartyCardHand playerHand; // ���� (�÷��̾�)
    public PartyCardHand partyBHand; // ����
    public PartyCardHand partyCHand; // ����

    // �� ������ Ư�� ī�� ���� (������ -> ī��ID)
    private Dictionary<string, string> regionCardMap;

    void Awake()
    {
        InitializeRegionCardMap();
    }

    void InitializeRegionCardMap()
    {
        regionCardMap = new Dictionary<string, string>
        {
            { "���� A", "M1" }, // ���� ��� -> ��� ���
            { "���� B", "S3" }, // �������� -> (ȯ�� ����, Ư���� ����)
            { "���� C", "M5" }, // ��ȸ -> ��ȸ �߾�
            { "���� D", "M2" }, // �Ŵ� ��ۻ� -> TV ���ͺ�
            { "���� E", "D2" }, // ���� -> ���� ����
            { "���� F", "A1" }, // ���� -> ķ���� ���ΰ�
            { "���� G", "M4" }, // û���� -> SNS
            { "���� H", "M2" }, // ������� -> TV ���ͺ�
            { "���� I", "D1" }, // ������ -> ���� ���ſ �ڷ���
            { "���� J", "A2" }, // �����α� -> ������
            { "���� K", "M3" }, // ��� -> ���ȸ
            { "���� L", "M5" }  // ���� -> ��ȸ �߾�
        };
    }

    // ���� ���� �� ��� ���� ī�� ���
    public void DistributeCardsForRound(int currentRound, List<Region> wonRegionsB, List<Region> wonRegionsC)
    {
        // ���� ī�� ���
        DistributeCardsForParty(partyBHand, currentRound, wonRegionsB, true);

        // ���� ī�� ���
        DistributeCardsForParty(partyCHand, currentRound, wonRegionsC, false);
    }

    void DistributeCardsForParty(PartyCardHand hand, int currentRound, List<Region> wonRegions, bool isPartyB)
    {
        hand.ClearHand();
        List<string> cards = new List<string>();

        // 2~5������ ���, ���� �Ͽ� �¸��� ���ű��� ���� Ư�� ī�� �߰�
        if (currentRound >= 2 && wonRegions != null && wonRegions.Count > 0)
        {
            foreach (Region region in wonRegions)
            {
                if (regionCardMap.ContainsKey(region.regionName))
                {
                    string cardId = regionCardMap[region.regionName];
                    Card card = CardDatabase.Instance.GetCard(cardId);

                    // �ൿī��(M, A, S, D)�� �߰�, �ߺ� üũ
                    if (card != null && !cards.Contains(cardId))
                    {
                        cards.Add(cardId);
                    }
                }
            }
        }

        // �켱������ ���� ���� �� 6�� ����
        if (cards.Count > 6)
        {
            cards = SortAndLimitCards(cards, isPartyB);
        }

        // 6�� �̸��� ��� �⺻ ī�� �߰�
        if (cards.Count < 6)
        {
            if (isPartyB)
            {
                // ����: A1, A2 �켱
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
                // ����: D1, D2 �켱
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

        // ������ 6�� �̸��̸� M1~M5 ���� �߰�
        List<string> mCards = new List<string> { "M1", "M2", "M3", "M4", "M5" };
        while (cards.Count < 6)
        {
            string randomM = mCards[Random.Range(0, mCards.Count)];
            cards.Add(randomM);
        }

        // ī�� �տ� �߰�
        foreach (string cardId in cards)
        {
            hand.AddCard(cardId);
        }
    }

    // �켱������ ���� ī�� ���� �� 6�� ����
    List<string> SortAndLimitCards(List<string> cards, bool isPartyB)
    {
        List<string> result = new List<string>();

        if (isPartyB)
        {
            // ���� �켱����: M > A > S > D
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
        }
        else
        {
            // ���� �켱����: S > M > D > A
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.S).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.M).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.D).OrderBy(c => c));
            result.AddRange(cards.Where(c => CardDatabase.Instance.GetCard(c).cardType == CardType.A).OrderBy(c => c));
        }

        // 6������ ����
        return result.Take(6).ToList();
    }
}