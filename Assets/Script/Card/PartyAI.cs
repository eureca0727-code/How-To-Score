using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PartyAI
{
    // 을(PartyB) 카드 선택 AI
    public static string SelectCardForPartyB(PartyCardHand hand, bool isUnderAttack, string attackCardId)
    {
        if (isUnderAttack)
        {
            // 1. 전체 방어 카드 체크
            List<string> fullDefenseCards = GetFullDefenseCards(hand, attackCardId);
            if (fullDefenseCards.Count > 0)
            {
                return GetRandomCard(fullDefenseCards);
            }

            // 2. S1 (공격 넘기기) 체크
            if (hand.HasCard("S1"))
            {
                return "S1";
            }

            // 3. A카드 체크 (공격받는 중에도 공격 가능)
            string aCard = hand.GetRandomCardOfType(CardType.A);
            if (aCard != null)
            {
                return aCard;
            }

            // 4. M카드 체크 (방어로 사용)
            string mCard = hand.GetRandomCardOfType(CardType.M);
            if (mCard != null)
            {
                return mCard;
            }

            // 5. 일부 방어 카드 체크
            List<string> partialDefenseCards = GetPartialDefenseCards(hand, attackCardId);
            if (partialDefenseCards.Count > 0)
            {
                return GetRandomCard(partialDefenseCards);
            }

            // 방어 실패 -> 아래 공격 규칙으로 fallthrough
        }

        // 공격 안 받는 중 (또는 방어 불가능)
        // 1. A카드 우선
        string attackA = hand.GetRandomCardOfType(CardType.A);
        if (attackA != null)
        {
            return attackA;
        }

        // 2. M카드
        string attackM = hand.GetRandomCardOfType(CardType.M);
        if (attackM != null)
        {
            return attackM;
        }

        // 3. S카드
        string specialCard = hand.GetRandomCardOfType(CardType.S);
        if (specialCard != null)
        {
            return specialCard;
        }

        // 카드 없음 -> null 반환 (턴 포기)
        return null;
    }

    // 병(PartyC) 카드 선택 AI
    public static string SelectCardForPartyC(PartyCardHand hand, bool isUnderAttack, string attackCardId)
    {
        if (isUnderAttack)
        {
            // 1. 전체 방어 카드 체크
            List<string> fullDefenseCards = GetFullDefenseCards(hand, attackCardId);
            if (fullDefenseCards.Count > 0)
            {
                return GetRandomCard(fullDefenseCards);
            }

            // 2. S1 (공격 넘기기) 체크
            if (hand.HasCard("S1"))
            {
                return "S1";
            }

            // 3. 일부 방어 카드 체크 (병은 A/M 안 씀)
            List<string> partialDefenseCards = GetPartialDefenseCards(hand, attackCardId);
            if (partialDefenseCards.Count > 0)
            {
                return GetRandomCard(partialDefenseCards);
            }

            // 방어 실패 -> 아래 규칙으로 fallthrough
        }

        // 공격 안 받는 중 (또는 방어 불가능)
        // 1. S카드 우선
        string specialCard = hand.GetRandomCardOfType(CardType.S);
        if (specialCard != null)
        {
            return specialCard;
        }

        // 2. A카드
        string attackA = hand.GetRandomCardOfType(CardType.A);
        if (attackA != null)
        {
            return attackA;
        }

        // 3. M카드
        string attackM = hand.GetRandomCardOfType(CardType.M);
        if (attackM != null)
        {
            return attackM;
        }

        // 카드 없음 -> null 반환 (턴 포기)
        return null;
    }

    // === 헬퍼 함수들 ===

    // 전체 방어 가능한 카드 찾기
    private static List<string> GetFullDefenseCards(PartyCardHand hand, string attackCardId)
    {
        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        if (attackCard == null) return new List<string>();

        List<string> defenseCards = new List<string>();
        foreach (string cardId in hand.cardsInHand)
        {
            if (attackCard.fullDefenseCards.Contains(cardId))
            {
                defenseCards.Add(cardId);
            }
        }

        return defenseCards;
    }

    // 일부 방어 가능한 카드 찾기
    private static List<string> GetPartialDefenseCards(PartyCardHand hand, string attackCardId)
    {
        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        if (attackCard == null) return new List<string>();

        List<string> defenseCards = new List<string>();
        foreach (string cardId in hand.cardsInHand)
        {
            if (attackCard.partialDefenseCards.Contains(cardId))
            {
                defenseCards.Add(cardId);
            }
        }

        return defenseCards;
    }

    // 리스트에서 랜덤 카드 선택
    private static string GetRandomCard(List<string> cards)
    {
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }
}
