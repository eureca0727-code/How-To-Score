using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance { get; private set; }

    // 각 정당 핸드 참조
    public PartyCardHand playerHand; // 플레이어
    public PartyCardHand partyBHand; // 을
    public PartyCardHand partyCHand; // 병

    // 턴 관리
    public enum Party { Player, PartyB, PartyC }
    public Party currentTurn;
    private bool isClockwise = true; // true: 시계방향, false: 반시계방향

    // 공격 상태
    private bool isUnderAttack = false;
    private string attackCardId = null;
    private Party attacker;

    // 특수 효과
    private bool isAmplified = false; // S3 카드 증폭 효과

    // 턴 카운트 (라운드 종료)
    public int playerActionCount = 0;
    public int partyBActionCount = 0;
    public int partyCActionCount = 0;

    // 라운드 번호
    public int currentRound = 1;

    // 라운드 동안의 누적 지지율 변화 (변수 추가)
    public int totalChangeA = 0;
    public int totalChangeB = 0;
    public int totalChangeC = 0;

    // 연속 턴포기 카운터 
    private int consecutivePassCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 라운드 시작 (라운드 시작 처리)
    public void StartRound()
    {
        // 턴 카운트 초기화
        playerActionCount = 0;
        partyBActionCount = 0;
        partyCActionCount = 0;
        partyCActionCount = 0;

        // 공격 상태 초기화
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false;

        // 지지율 변화 초기화 
        totalChangeA = 0;
        totalChangeB = 0;
        totalChangeC = 0;

        // 연속 턴포기 초기화 
        consecutivePassCount = 0;

        // 시작 정당 결정
        int random = Random.Range(0, 3);
        currentTurn = (Party)random;

        Debug.Log($"라운드 {currentRound} 시작! 선공: {GetPartyName(currentTurn)}");
    }

    // 다음 차례로 이동
    public void NextTurn()
    {
        if (isClockwise)
        {
            // 갑 -> 을 -> 병 -> 갑
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyB;
            else if (currentTurn == Party.PartyB)
                currentTurn = Party.PartyC;
            else
                currentTurn = Party.Player;
        }
        else
        {
            // 갑 -> 병 -> 을 -> 갑
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyC;
            else if (currentTurn == Party.PartyC)
                currentTurn = Party.PartyB;
            else
                currentTurn = Party.Player;
        }
    }

    // 순서 반전 (S2 카드 효과)
    public void ReverseOrder()
    {
        isClockwise = !isClockwise;
        Debug.Log($"순서가 반전되었습니다! 현재 방향: {(isClockwise ? "시계방향" : "반시계방향")}");
    }

    // 공격 카드 제시
    public void PlayAttackCard(string cardId, Party attackingParty)
    {
        isUnderAttack = true;
        attackCardId = cardId;
        attacker = attackingParty;

        Card card = CardDatabase.Instance.GetCard(cardId);
        string attackerName = GetPartyName(attackingParty);

        Debug.Log($"{attackerName}이(가) {card.cardName} 공격!");

        // UI에 알림
        if (CardBattleUI.Instance != null)
        {
            Party targetParty = GetNextParty(attackingParty);
            string targetName = GetPartyName(targetParty);
            CardBattleUI.Instance.ShowWarning($"{attackerName}당이 {targetName}당을 {card.cardName}(으)로 공격!");
        }
    }

    // 방어 카드 제시 (방어 성공 여부 반환)
    public bool PlayDefenseCard(string defenseCardId)
    {
        if (!isUnderAttack || attackCardId == null)
        {
            Debug.LogWarning("현재 공격받고 있지 않습니다.");
            return false;
        }

        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        Card defenseCard = CardDatabase.Instance.GetCard(defenseCardId);
        string partyName = GetPartyName(currentTurn);

        // 전체 방어 성공
        if (attackCard.fullDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}으로 완전 방어 성공!");
            if (CardBattleUI.Instance != null)
            {
                CardBattleUI.Instance.ShowWarning($"{partyName}당이 {defenseCard.cardName}(으)로 완전 방어 성공!");
            }

            ClearAttackState();
            return true;
        }

        // 부분 방어 성공
        if (attackCard.partialDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}으로 부분 방어 성공!");
            int damage = attackCard.attackValue / 3;
            if (isAmplified) damage *= 2;
            
            // UI에 알림
            if (CardBattleUI.Instance != null)
            {
                CardBattleUI.Instance.ShowWarning($"{partyName}당이 {defenseCard.cardName}(으)로 부분 방어! 일부 데미지 받음");
            }

            ApplyDamage(currentTurn, damage, damage);
            ClearAttackState();
            return true;
        }

        // 방어 실패
        Debug.LogWarning("방어 실패! 전체 데미지를 받습니다.");

        // UI에 경고 표시
        if (CardBattleUI.Instance != null)
        {
            CardBattleUI.Instance.ShowWarning($"{partyName}당의 방어 실패! {defenseCard.cardName}(으)로 {attackCard.cardName}을(를) 막을 수 없습니다.");
        }
        ApplyFullDamage();
        return false;
    }

    // 통합 카드 사용 함수 (M카드 자동 전환 처리)
    public void PlayCard(string cardId)
    {
        Card card = CardDatabase.Instance.GetCard(cardId);

        Debug.Log($"[PlayCard 시작] {GetPartyName(currentTurn)}당 - 카드: {cardId}");

        // 행동 카운트 증가
        IncrementActionCount(currentTurn);

        // 연속 턴포기 초기화 
        consecutivePassCount = 0;

        // 카드 타입별 처리
        switch (card.cardType)
        {
            case CardType.M:
                if (isUnderAttack)
                {
                    PlayDefenseCard(cardId);
                }
                else
                {
                    PlayAttackCard(cardId, currentTurn);
                }
                break;

            case CardType.A:
                if (isUnderAttack)
                {
                    Debug.Log($"{GetPartyName(currentTurn)}이(가) 방어를 포기하고 반격합니다!");

                    // UI에 알림 추가
                    if (CardBattleUI.Instance != null)
                    {
                        CardBattleUI.Instance.ShowWarning($"{GetPartyName(currentTurn)}당이 방어를 포기하고 {card.cardName}(으)로 반격!");
                    }

                    ApplyFullDamage();
                }
                PlayAttackCard(cardId, currentTurn);
                break;

            case CardType.D:
                PlayDefenseCard(cardId);
                break;

            case CardType.S:
                switch (card.specialEffect)
                {
                    case SpecialEffect.PassAttack:
                        PassAttack();
                        // PassAttack 내부에서 NextTurn() 호출되므로 여기서는 제거만
                        GetCurrentHand().RemoveCard(cardId);
                        Debug.Log($"[PlayCard 끝] {GetPartyName(currentTurn)}당 - 핸드 카드 수: {GetCurrentHand().GetCardCount()}");
                        return; // 여기서 종료

                    case SpecialEffect.ReverseOrder:
                        ReverseOrder();
                        break;

                    case SpecialEffect.AmplifyAttack:
                        AmplifyNextAttack();
                        break;
                }
                break;
        }

        // ===== 카드 제거를 NextTurn() 전에! =====
        GetCurrentHand().RemoveCard(cardId);
        Debug.Log($"[PlayCard 끝] {GetPartyName(currentTurn)}당 - 핸드 카드 수: {GetCurrentHand().GetCardCount()}");

        // 이제 턴 넘김
        NextTurn();
    }    // AI 턴 실행 (을/병 전용)
    public void ExecuteAITurn()
    {
        if (currentTurn == Party.Player)
        {
            Debug.LogWarning("플레이어 턴에는 AI를 실행할 수 없습니다!");
            return;
        }

        PartyCardHand hand = GetCurrentHand();
        string selectedCardId = null;

        // AI 카드 선택
        if (currentTurn == Party.PartyB)
        {
            selectedCardId = PartyAI.SelectCardForPartyB(hand, isUnderAttack, attackCardId);
        }
        else if (currentTurn == Party.PartyC)
        {
            selectedCardId = PartyAI.SelectCardForPartyC(hand, isUnderAttack, attackCardId);
        }

        // 카드 선택됨 -> 사용
        if (selectedCardId != null)
        {
            Debug.Log($"{GetPartyName(currentTurn)}이(가) {CardDatabase.Instance.GetCard(selectedCardId).cardName} 카드를 제시합니다!");
            PlayCard(selectedCardId);
        }
        else
        {
            // 카드 없음 -> 턴 포기
            Debug.Log($"{GetPartyName(currentTurn)}이(가) 카드 제시를 포기합니다.");
            PassTurn();
            NextTurn();
        }
    }

    // S1: 공격 넘기기
    public void PassAttack()
    {
        if (!isUnderAttack)
        {
            Debug.LogWarning("현재 공격받고 있지 않습니다.");
            return;
        }

        Debug.Log($"{GetPartyName(currentTurn)}이(가) 공격을 다음 순서에게 넘깁니다!");

        // 다음 턴으로 이동 (공격 상태는 유지)
        NextTurn();

        Debug.Log($"공격이 {GetPartyName(currentTurn)}에게 넘어갔습니다!");
    }

    // S3: 공격 증폭
    public void AmplifyNextAttack()
    {
        isAmplified = true;
        Debug.Log("다음 공격이 2배로 증폭됩니다!");
    }

    // 방어 실패 시 전체 데미지
    public void ApplyFullDamage()
    {
        if (!isUnderAttack || attackCardId == null)
            return;

        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        int damage = attackCard.attackValue;
        if (isAmplified) damage *= 2;

        ApplyDamage(currentTurn, damage, attackCard.attackerGain);
        ClearAttackState();
    }

    // 데미지 적용 로직
    void ApplyDamage(Party defender, int damage, int attackerGain)
    {
        // 각 정당의 지지율 변화량 계산
        int changeA = 0, changeB = 0, changeC = 0;

        // 방어자 지지율 감소
        if (defender == Party.Player) changeA = -damage;
        else if (defender == Party.PartyB) changeB = -damage;
        else changeC = -damage;

        // 공격자 지지율 증가
        if (attacker == Party.Player) changeA += attackerGain;
        else if (attacker == Party.PartyB) changeB += attackerGain;
        else changeC += attackerGain;

        // 나머지 정당 지지율 증가 (전체 방어 실패 시에만)
        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        if (damage == attackCard.attackValue)
        {
            int remainingGain = damage - attackerGain;
            Party otherParty = GetOtherParty(defender, attacker);

            if (otherParty == Party.Player) changeA += remainingGain;
            else if (otherParty == Party.PartyB) changeB += remainingGain;
            else changeC += remainingGain;
        }

        // 누적 변화량 기록 
        totalChangeA += changeA;
        totalChangeB += changeB;
        totalChangeC += changeC;

        // GameManager를 통해 실제 지지율 변경
        GameManager.Instance.ChangeAllRegionsSupport(changeA, changeB, changeC);

        Debug.Log($"지지율 변경: 갑({changeA:+#;-#;0}), 을({changeB:+#;-#;0}), 병({changeC:+#;-#;0})");
        Debug.Log($"누적 변화: 갑({totalChangeA:+#;-#;0}), 을({totalChangeB:+#;-#;0}), 병({totalChangeC:+#;-#;0})");
    }

    public (int, int, int) GetTotalSupportChange()
    {
        return (totalChangeA, totalChangeB, totalChangeC);
    }


    // 공격 상태 초기화
    void ClearAttackState()
    {
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false; // 방어 성공 또는 증폭 효과 소멸
    }

    // 턴 포기
    public void PassTurn()
    {
        IncrementActionCount(currentTurn);

        // 연속 턴포기 증가 (추가)
        consecutivePassCount++;

        // 공격받고 있는 상태에서 턴 포기 시 전체 데미지
        if (isUnderAttack)
        {
            ApplyFullDamage();
        }

        Debug.Log($"{GetPartyName(currentTurn)}이(가) 턴을 포기했습니다. (연속 {consecutivePassCount}번)");
    }

    // 행동 카운트 증가
    public void IncrementActionCount(Party party)
    {
        switch (party)
        {
            case Party.Player:
                playerActionCount++;
                break;
            case Party.PartyB:
                partyBActionCount++;
                break;
            case Party.PartyC:
                partyCActionCount++;
                break;
        }
    }

    // 라운드 종료 체크 
    public bool IsRoundEnd()
    {
        // 3명 연속 턴포기
        if (consecutivePassCount >= 3)
        {
            Debug.Log("3명 모두 연속 턴포기! 라운드 종료!");
            return true;
        }

        // 모든 정당의 카드 소진
        bool allEmpty = playerHand.GetCardCount() == 0
                     && partyBHand.GetCardCount() == 0
                     && partyCHand.GetCardCount() == 0;

        if (allEmpty)
        {
            Debug.Log("모든 카드 소진! 라운드 종료!");
            return true;
        }

        return false;
    }

    // 헬퍼 메소드들
    // 현재 턴의 카드 핸드 반환
    public PartyCardHand GetCurrentHand()
    {
        switch (currentTurn)
        {
            case Party.Player:
                return playerHand;
            case Party.PartyB:
                return partyBHand;
            case Party.PartyC:
                return partyCHand;
            default:
                return null;
        }
    }

    public string GetPartyName(Party party)
    {
        switch (party)
        {
            case Party.Player: return "갑";
            case Party.PartyB: return "을";
            case Party.PartyC: return "병";
            default: return "알 수 없음";
        }
    }

    Party GetOtherParty(Party p1, Party p2)
    {
        if (p1 != Party.Player && p2 != Party.Player) return Party.Player;
        if (p1 != Party.PartyB && p2 != Party.PartyB) return Party.PartyB;
        return Party.PartyC;
    }

    public bool IsUnderAttack()
    {
        return isUnderAttack;
    }

    public string GetAttackCardId()
    {
        return attackCardId;
    }

    public bool IsClockwise()
    {
        return isClockwise;
    }
    // 카드 사용 가능 여부 검증 (새 메서드 추가)
    public string CanUseCard(string cardId)
    {
        Card card = CardDatabase.Instance.GetCard(cardId);
        if (card == null)
        {
            return "카드 데이터를 찾을 수 없습니다.";
        }

        PartyCardHand hand = GetCurrentHand();
        if (!hand.HasCard(cardId))
        {
            return "보유하지 않은 카드입니다.";
        }

        // D카드는 공격받을 때만 사용 가능
        if (card.cardType == CardType.D && !isUnderAttack)
        {
            return "공격받고 있지 않아 방어 카드를 사용할 수 없습니다.";
        }

        // S1(공격 넘기기)는 공격받을 때만 사용 가능
        if (card.cardType == CardType.S && card.specialEffect == SpecialEffect.PassAttack && !isUnderAttack)
        {
            return "공격받고 있지 않아 공격 넘기기를 사용할 수 없습니다.";
        }

        return null; // 사용 가능
    }

    // 플레이어 카드 사용 (통합 메서드)
    public bool ExecutePlayerAction(string cardId, out string errorMessage)
    {
        errorMessage = null;

        if (currentTurn != Party.Player)
        {
            errorMessage = "플레이어의 턴이 아닙니다.";
            return false;
        }

        // 카드 사용 가능 여부 검증
        errorMessage = CanUseCard(cardId);
        if (errorMessage != null)
        {
            return false;
        }

        // 카드 사용
        PlayCard(cardId);
        return true;
    }

    // 플레이어 턴 포기 (통합 메서드)
    public bool ExecutePlayerPass(out string errorMessage)
    {
        errorMessage = null;

        if (currentTurn != Party.Player)
        {
            errorMessage = "플레이어의 턴이 아닙니다.";
            return false;
        }

        // 턴 포기 처리
        PassTurn();
        NextTurn();
        return true;
    }
    // 다음 턴 정당 계산 (currentTurn 변경 없이)
    Party GetNextParty(Party current)
    {
        if (isClockwise)
        {
            if (current == Party.Player) return Party.PartyB;
            else if (current == Party.PartyB) return Party.PartyC;
            else return Party.Player;
        }
        else
        {
            if (current == Party.Player) return Party.PartyC;
            else if (current == Party.PartyC) return Party.PartyB;
            else return Party.Player;
        }
    }
}
