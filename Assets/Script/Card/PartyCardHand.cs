using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyCardHand : MonoBehaviour
{
    public List<string> cardsInHand; // 보유 중인 카드 ID 리스트
    public string partyName; // "갑", "을", "병"

    void Awake()
    {
        cardsInHand = new List<string>();
    }

    // 카드 추가
    public void AddCard(string cardId)
    {
        cardsInHand.Add(cardId);
    }

    // 카드 제거
    public void RemoveCard(string cardId)
    {
        cardsInHand.Remove(cardId);
    }

    // 특정 타입의 카드가 있는지 확인
    public bool HasCardOfType(CardType type)
    {
        foreach (string cardId in cardsInHand)
        {
            Card card = CardDatabase.Instance.GetCard(cardId);
            if (card != null && card.cardType == type)
            {
                return true;
            }
        }
        return false;
    }

    // 특정 카드가 있는지 확인
    public bool HasCard(string cardId)
    {
        return cardsInHand.Contains(cardId);
    }

    // 특정 타입의 카드 중 랜덤하게 하나 선택
    public string GetRandomCardOfType(CardType type)
    {
        List<string> matchingCards = new List<string>();

        foreach (string cardId in cardsInHand)
        {
            Card card = CardDatabase.Instance.GetCard(cardId);
            if (card != null && card.cardType == type)
            {
                matchingCards.Add(cardId);
            }
        }

        if (matchingCards.Count > 0)
        {
            return matchingCards[Random.Range(0, matchingCards.Count)];
        }

        return null;
    }

    // 보유 카드 개수
    public int GetCardCount()
    {
        return cardsInHand.Count;
    }

    // 모든 카드 제거
    public void ClearHand()
    {
        cardsInHand.Clear();
    }
}