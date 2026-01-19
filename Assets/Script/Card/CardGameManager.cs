using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance { get; private set; }

    // Á¤´ç ¼ÕÆĞ ÂüÁ¶
    public PartyCardHand playerHand; // °©´ç
    public PartyCardHand partyBHand; // À»´ç
    public PartyCardHand partyCHand; // º´´ç

    // ÅÏ °ü¸®
    public enum Party { Player, PartyB, PartyC }
    public Party currentTurn;
    private bool isClockwise = true; // true: °©¡æÀ»¡æº´, false: º´¡æÀ»¡æ°©

    // °ø°İ »óÅÂ
    private bool isUnderAttack = false;
    private string attackCardId = null;
    private Party attacker;

    // Æ¯¼ö È¿°ú
    private bool isAmplified = false; // S3 °ø°İ ÁõÆø È¿°ú

    // ÅÏ Ä«¿îÆ® (Á¾·á Á¶°Ç)
    public int playerActionCount = 0;
    public int partyBActionCount = 0;
    public int partyCActionCount = 0;

    // ÇöÀç ¶ó¿îµå
    public int currentRound = 1;

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

    // ¶ó¿îµå ½ÃÀÛ (¼±°ø ·£´ı °áÁ¤)
    public void StartRound()
    {
        // ÅÏ Ä«¿îÆ® ÃÊ±âÈ­
        playerActionCount = 0;
        partyBActionCount = 0;
        partyCActionCount = 0;

        // °ø°İ »óÅÂ ÃÊ±âÈ­
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false;

        // ·£´ı ¼±°ø °áÁ¤
        int random = Random.Range(0, 3);
        currentTurn = (Party)random;

        Debug.Log($"¶ó¿îµå {currentRound} ½ÃÀÛ! ¼±°ø: {GetPartyName(currentTurn)}");
    }

    // ´ÙÀ½ ÅÏÀ¸·Î ÁøÇà
    public void NextTurn()
    {
        if (isClockwise)
        {
            // °© ¡æ À» ¡æ º´
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyB;
            else if (currentTurn == Party.PartyB)
                currentTurn = Party.PartyC;
            else
                currentTurn = Party.Player;
        }
        else
        {
            // º´ ¡æ À» ¡æ °©
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyC;
            else if (currentTurn == Party.PartyC)
                currentTurn = Party.PartyB;
            else
                currentTurn = Party.Player;
        }
    }

    // ¼ø¼­ ¹İÀü (S2 Ä«µå È¿°ú)
    public void ReverseOrder()
    {
        isClockwise = !isClockwise;
        Debug.Log($"¼ø¼­°¡ ¹İÀüµÇ¾ú½À´Ï´Ù! ÇöÀç ¹æÇâ: {(isClockwise ? "°©¡æÀ»¡æº´" : "º´¡æÀ»¡æ°©")}");
    }

    // °ø°İ Ä«µå Á¦½Ã
    public void PlayAttackCard(string cardId, Party attackingParty)
    {
        isUnderAttack = true;
        attackCardId = cardId;
        attacker = attackingParty;

        Card card = CardDatabase.Instance.GetCard(cardId);
        Debug.Log($"{GetPartyName(attackingParty)}ÀÌ(°¡) {card.cardName} °ø°İ!");
    }

    // ¹æ¾î Ä«µå Á¦½Ã (¿ÏÀü ¹æ¾î ¼º°ø ¿©ºÎ ¹İÈ¯)
    public bool PlayDefenseCard(string defenseCardId)
    {
        if (!isUnderAttack || attackCardId == null)
        {
            Debug.LogWarning("ÇöÀç °ø°İ¹Ş°í ÀÖÁö ¾Ê½À´Ï´Ù.");
            return false;
        }

        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        Card defenseCard = CardDatabase.Instance.GetCard(defenseCardId);

        // ¿ÏÀü ¹æ¾î ¼º°ø
        if (attackCard.fullDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}(À¸)·Î ¿ÏÀü ¹æ¾î ¼º°ø!");
            ClearAttackState();
            return true;
        }

        // ÀÏºÎ ¹æ¾î ¼º°ø
        if (attackCard.partialDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}(À¸)·Î ÀÏºÎ ¹æ¾î ¼º°ø!");
            int damage = attackCard.attackValue / 3;
            if (isAmplified) damage *= 2;

            ApplyDamage(currentTurn, damage, attackCard.attackerGain / 3);
            ClearAttackState();
            return true;
        }

        Debug.LogWarning("ë°©ì–´ ì‹¤íŒ¨! ì „ì²´ ë°ë¯¸ì§€ë¥¼ ë°›ìŠµë‹ˆë‹¤.");
        ApplyFullDamage();
        return false;

    // í†µí•© ì¹´ë“œ ì‚¬ìš© í•¨ìˆ˜ (Mì¹´ë“œ ìë™ ì „í™˜ ì²˜ë¦¬)
    public void PlayCard(string cardId)
    {
        Card card = CardDatabase.Instance.GetCard(cardId);
        if (card == null)
        {
            Debug.LogError($"ì¹´ë“œ {cardId}ë¥¼ ì°¾ì„ ìˆ˜ ì—†ìŠµë‹ˆë‹¤!");
            return;
        }

        // í–‰ë™ ì¹´ìš´íŠ¸ ì¦ê°€
        IncrementActionCount(currentTurn);

        // ì¹´ë“œ íƒ€ì…ë³„ ì²˜ë¦¬
        switch (card.cardType)
        {
            case CardType.M:
                // Mì¹´ë“œ: ê³µê²©ë°›ëŠ” ì¤‘ì´ë©´ ë°©ì–´, ì•„ë‹ˆë©´ ê³µê²©
                if (isUnderAttack)
                {
                    PlayDefenseCard(cardId);
                }
                else
                {
                    PlayAttackCard(cardId, currentTurn);
                    NextTurn();  // ê³µê²© í›„ ë‹¤ìŒ í„´ìœ¼ë¡œ
                }
                break;

            case CardType.A:
                // Aì¹´ë“œ: ë¬´ì¡°ê±´ ê³µê²©
                if (isUnderAttack)
                {
                    // ê³µê²©ë°›ëŠ” ì¤‘ì— ê³µê²©ì¹´ë“œ ì œì‹œ -> ë¨¼ì € í”¼í•´ ë°›ê³  ê³µê²©
                    Debug.Log($"{GetPartyName(currentTurn)}ì´(ê°€) ë°©ì–´ë¥¼ í¬ê¸°í•˜ê³  ë°˜ê²©í•©ë‹ˆë‹¤!");
                    ApplyFullDamage();
                }
                PlayAttackCard(cardId, currentTurn);
                NextTurn();
                break;

            case CardType.D:
                // Dì¹´ë“œ: ë¬´ì¡°ê±´ ë°©ì–´
                if (!isUnderAttack)
                {
                    Debug.LogWarning("í˜„ì¬ ê³µê²©ë°›ê³  ìˆì§€ ì•Šì•„ ë°©ì–´ ì¹´ë“œë¥¼ ì‚¬ìš©í•  ìˆ˜ ì—†ìŠµë‹ˆë‹¤!");
                    return;
                }
                PlayDefenseCard(cardId);
                if (!isUnderAttack)  // ë°©ì–´ ì„±ê³µ ì‹œì—ë§Œ ë‹¤ìŒ í„´
                {
                    NextTurn();
                }
                break;

            case CardType.S:
                // Sì¹´ë“œ: íŠ¹ìˆ˜ íš¨ê³¼
                switch (card.specialEffect)
                {
                    case SpecialEffect.PassAttack:
                        PassAttack();
                        // PassAttack ë‚´ë¶€ì—ì„œ NextTurn() í˜¸ì¶œë¨
                        break;

                    case SpecialEffect.ReverseOrder:
                        ReverseOrder();
                        NextTurn();
                        break;

                    case SpecialEffect.AmplifyAttack:
                        AmplifyNextAttack();
                        NextTurn();
                        break;
                }
                break;
        }

        // ì‚¬ìš©í•œ ì¹´ë“œëŠ” í•¸ë“œì—ì„œ ì œê±°
        GetCurrentHand().RemoveCard(cardId);

    // AI í„´ ì‹¤í–‰ (ì„/ë³‘ ì „ìš©)
    public void ExecuteAITurn()
    {
        if (currentTurn == Party.Player)
        {
            Debug.LogWarning("í”Œë ˆì´ì–´ í„´ì—ëŠ” AIë¥¼ ì‹¤í–‰í•  ìˆ˜ ì—†ìŠµë‹ˆë‹¤!");
            return;
        }

        PartyCardHand hand = GetCurrentHand();
        string selectedCardId = null;

        // AI ì¹´ë“œ ì„ íƒ
        if (currentTurn == Party.PartyB)
        {
            selectedCardId = PartyAI.SelectCardForPartyB(hand, isUnderAttack, attackCardId);
        }
        else if (currentTurn == Party.PartyC)
        {
            selectedCardId = PartyAI.SelectCardForPartyC(hand, isUnderAttack, attackCardId);
        }

        // ì¹´ë“œ ì„ íƒë¨ -> ì‚¬ìš©
        if (selectedCardId != null)
        {
            Debug.Log($"{GetPartyName(currentTurn)}ì´(ê°€) {CardDatabase.Instance.GetCard(selectedCardId).cardName} ì¹´ë“œë¥¼ ì œì‹œí•©ë‹ˆë‹¤!");
            PlayCard(selectedCardId);
        }
        else
        {
            // ì¹´ë“œ ì—†ìŒ -> í„´ í¬ê¸°
            Debug.Log($"{GetPartyName(currentTurn)}ì´(ê°€) ì¹´ë“œ ì œì‹œë¥¼ í¬ê¸°í•©ë‹ˆë‹¤.");
            PassTurn();
            NextTurn();
        }
    }
    }
    }

    // S1: °ø°İ ³Ñ±â±â
    public void PassAttack()
    {
        if (!isUnderAttack)
        {
            Debug.LogWarning("ÇöÀç °ø°İ¹Ş°í ÀÖÁö ¾Ê½À´Ï´Ù.");
            return;
        }

        Debug.Log($"{GetPartyName(currentTurn)}ÀÌ(°¡) °ø°İÀ» ´ÙÀ½ Á¤´ç¿¡°Ô ³Ñ°å½À´Ï´Ù!");

        // ë‹¤ìŒ í„´ìœ¼ë¡œ ì´ë™ (ê³µê²© ìƒíƒœëŠ” ìœ ì§€)
        NextTurn();

        Debug.Log($"ê³µê²©ì´ {GetPartyName(currentTurn)}ì—ê²Œ ë„˜ì–´ê°”ìŠµë‹ˆë‹¤!");
    }

    // S3: °ø°İ ÁõÆø
    public void AmplifyNextAttack()
    {
        isAmplified = true;
        Debug.Log("´ÙÀ½ °ø°İÀÌ 2¹è·Î ÁõÆøµË´Ï´Ù!");
    }

    // ¹æ¾î ½ÇÆĞ ½Ã ÇÇÇØ Àû¿ë
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

    // ÁöÁöÀ² º¯µ¿ Àû¿ë
    void ApplyDamage(Party defender, int damage, int attackerGain)
    {
        // ê° ì •ë‹¹ì˜ ì§€ì§€ìœ¨ ë³€í™”ëŸ‰ ê³„ì‚°
        int changeA = 0, changeB = 0, changeC = 0;

        // ë°©ì–´ì ì§€ì§€ìœ¨ ê°ì†Œ
        if (defender == Party.Player) changeA = -damage;
        else if (defender == Party.PartyB) changeB = -damage;
        else changeC = -damage;

        // ê³µê²©ì ì§€ì§€ìœ¨ ì¦ê°€
        if (attacker == Party.Player) changeA += attackerGain;
        else if (attacker == Party.PartyB) changeB += attackerGain;
        else changeC += attackerGain;

        // ë‚˜ë¨¸ì§€ ì •ë‹¹ ì§€ì§€ìœ¨ ì¦ê°€ (ì „ì²´ ë°©ì–´ ì‹¤íŒ¨ ì‹œì—ë§Œ)
        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        if (damage == attackCard.attackValue)
        {
            int remainingGain = damage - attackerGain;
            Party otherParty = GetOtherParty(defender, attacker);

            if (otherParty == Party.Player) changeA += remainingGain;
            else if (otherParty == Party.PartyB) changeB += remainingGain;
            else changeC += remainingGain;
        }

        // GameManagerë¥¼ í†µí•´ ì‹¤ì œ ì§€ì§€ìœ¨ ë³€ê²½
        GameManager.Instance.ChangeAllRegionsSupport(changeA, changeB, changeC);

        Debug.Log($"ì§€ì§€ìœ¨ ë³€ê²½: ê°‘({changeA:+#;-#;0}), ì„({changeB:+#;-#;0}), ë³‘({changeC:+#;-#;0})");
    }

    // °ø°İ »óÅÂ ÃÊ±âÈ­
    void ClearAttackState()
    {
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false; // ¹æ¾î ¼º°ø ½Ã¿¡µµ ÁõÆø È¿°ú ¼Ò¸ê
    }

    // ÅÏ Æ÷±â
    public void PassTurn()
    {
        IncrementActionCount(currentTurn);

        // °ø°İ¹Ş°í ÀÖ´Â »óÅÂ¿¡¼­ ÅÏ Æ÷±â ½Ã ÀüÃ¼ ÇÇÇØ
        if (isUnderAttack)
        {
            ApplyFullDamage();
        }

        Debug.Log($"{GetPartyName(currentTurn)}ÀÌ(°¡) ÅÏÀ» Æ÷±âÇß½À´Ï´Ù.");
        // NextTurn()ì€ í˜¸ì¶œí•˜ëŠ” ìª½ì—ì„œ ì²˜ë¦¬ (ExecuteAITurn ë“±)
    }

    // Çàµ¿ Ä«¿îÆ® Áõ°¡
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

    // Á¾·á Á¶°Ç Ã¼Å©
    public bool IsRoundEnd()
    {
        return playerActionCount >= 3 && partyBActionCount >= 3 && partyCActionCount >= 3;
    }

    // ÇïÆÛ ¸Ş¼­µåµé
    // í˜„ì¬ í„´ì˜ ì¹´ë“œ í•¸ë“œ ë°˜í™˜
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
            case Party.Player: return "°©";
            case Party.PartyB: return "À»";
            case Party.PartyC: return "º´";
            default: return "¾Ë ¼ö ¾øÀ½";
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
}