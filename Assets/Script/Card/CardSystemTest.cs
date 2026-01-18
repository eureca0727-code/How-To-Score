using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestCardSystem());
    }

    IEnumerator TestCardSystem()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("=== 카드 시스템 테스트 시작 ===");

        // 1. CardDatabase 테스트
        Debug.Log("\n[1단계] CardDatabase 테스트");
        TestCardDatabase();

        yield return new WaitForSeconds(1f);

        // 2. PartyCardHand 테스트
        Debug.Log("\n[2단계] PartyCardHand 테스트");
        TestPartyCardHand();

        yield return new WaitForSeconds(1f);

        // 3. CardDistributor 테스트
        Debug.Log("\n[3단계] CardDistributor 테스트");
        TestCardDistributor();

        yield return new WaitForSeconds(1f);

        // 4. CardGameManager 테스트
        Debug.Log("\n[4단계] CardGameManager 테스트");
        TestCardGameManager();

        Debug.Log("\n=== 카드 시스템 테스트 완료 ===");
    }

    void TestCardDatabase()
    {
        // M1 카드 가져오기
        Card m1 = CardDatabase.Instance.GetCard("M1");
        if (m1 != null)
        {
            Debug.Log($"M1 카드: {m1.cardName}, 공격력: {m1.attackValue}, 타입: {m1.cardType}");
            Debug.Log($"  완전 방어 카드: {string.Join(", ", m1.fullDefenseCards)}");
        }

        // A1 카드 가져오기
        Card a1 = CardDatabase.Instance.GetCard("A1");
        if (a1 != null)
        {
            Debug.Log($"A1 카드: {a1.cardName}, 공격력: {a1.attackValue}, 타입: {a1.cardType}");
        }

        // S1 카드 가져오기
        Card s1 = CardDatabase.Instance.GetCard("S1");
        if (s1 != null)
        {
            Debug.Log($"S1 카드: {s1.cardName}, 특수효과: {s1.specialEffect}");
        }

        // 모든 카드 개수 확인
        List<string> allCards = CardDatabase.Instance.GetAllCardIds();
        Debug.Log($"전체 카드 개수: {allCards.Count}개");
    }

    void TestPartyCardHand()
    {
        // CardDistributor에서 손패 참조 가져오기
        CardDistributor distributor = FindObjectOfType<CardDistributor>();
        if (distributor == null || distributor.partyBHand == null)
        {
            Debug.LogError("CardDistributor 또는 PartyHand를 찾을 수 없습니다!");
            return;
        }

        PartyCardHand testHand = distributor.partyBHand;

        // 카드 추가 테스트
        testHand.ClearHand();
        testHand.AddCard("M1");
        testHand.AddCard("M2");
        testHand.AddCard("A1");

        Debug.Log($"카드 추가 후 개수: {testHand.GetCardCount()}개");
        Debug.Log($"보유 카드: {string.Join(", ", testHand.cardsInHand)}");

        // 카드 확인 테스트
        Debug.Log($"M1 보유 여부: {testHand.HasCard("M1")}");
        Debug.Log($"M 타입 보유 여부: {testHand.HasCardOfType(CardType.M)}");

        // 랜덤 선택 테스트
        string randomM = testHand.GetRandomCardOfType(CardType.M);
        Debug.Log($"M 타입 랜덤 선택: {randomM}");

        // 카드 제거 테스트
        testHand.RemoveCard("M1");
        Debug.Log($"M1 제거 후 개수: {testHand.GetCardCount()}개");
    }

    void TestCardDistributor()
    {
        CardDistributor distributor = FindObjectOfType<CardDistributor>();
        if (distributor == null)
        {
            Debug.LogError("CardDistributor를 찾을 수 없습니다!");
            return;
        }

        // 1라운드 카드 배분 테스트 (이전 승리 지역 없음)
        distributor.DistributeCardsForRound(1, null, null);

        Debug.Log($"을당 카드 ({distributor.partyBHand.GetCardCount()}장): {string.Join(", ", distributor.partyBHand.cardsInHand)}");
        Debug.Log($"병당 카드 ({distributor.partyCHand.GetCardCount()}장): {string.Join(", ", distributor.partyCHand.cardsInHand)}");
    }

    void TestCardGameManager()
    {
        if (CardGameManager.Instance == null)
        {
            Debug.LogError("CardGameManager를 찾을 수 없습니다!");
            return;
        }

        // 라운드 시작 테스트
        CardGameManager.Instance.StartRound();
        Debug.Log($"현재 턴: {CardGameManager.Instance.GetPartyName(CardGameManager.Instance.currentTurn)}");
        Debug.Log($"진행 방향: {(CardGameManager.Instance.IsClockwise() ? "갑→을→병" : "병→을→갑")}");

        // 턴 진행 테스트
        var startTurn = CardGameManager.Instance.currentTurn;
        CardGameManager.Instance.NextTurn();
        Debug.Log($"다음 턴: {CardGameManager.Instance.GetPartyName(CardGameManager.Instance.currentTurn)}");

        // 순서 반전 테스트
        CardGameManager.Instance.ReverseOrder();
        Debug.Log($"순서 반전 후: {(CardGameManager.Instance.IsClockwise() ? "갑→을→병" : "병→을→갑")}");

        // 공격 테스트
        CardGameManager.Instance.PlayAttackCard("M1", CardGameManager.Party.Player);
        Debug.Log($"공격 상태: {CardGameManager.Instance.IsUnderAttack()}");
        Debug.Log($"공격 카드: {CardGameManager.Instance.GetAttackCardId()}");

        // 방어 테스트 (완전 방어)
        bool defenseSuccess = CardGameManager.Instance.PlayDefenseCard("M2");
        Debug.Log($"방어 성공 여부: {defenseSuccess}");
        Debug.Log($"방어 후 공격 상태: {CardGameManager.Instance.IsUnderAttack()}");

        // 종료 조건 테스트
        CardGameManager.Instance.playerActionCount = 3;
        CardGameManager.Instance.partyBActionCount = 3;
        CardGameManager.Instance.partyCActionCount = 3;
        Debug.Log($"라운드 종료 여부: {CardGameManager.Instance.IsRoundEnd()}");
    }
}