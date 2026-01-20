using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardButtonUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI cardNameText;       // 카드 이름
    public TextMeshProUGUI cardTypeText;       // 카드 타입 (M/A/D/S)
    public TextMeshProUGUI attackValueText;    // 공격력 (M/A 카드만)
    public TextMeshProUGUI defenseInfoText;    // 방어 정보
    public Image backgroundImage;              // 배경 이미지

    [Header("Type Colors")]
    public Color mCardColor = new Color(1f, 0.8f, 0.4f);  // 주황
    public Color aCardColor = new Color(1f, 0.3f, 0.3f);  // 빨강
    public Color dCardColor = new Color(0.4f, 0.8f, 1f);  // 파랑
    public Color sCardColor = new Color(0.8f, 0.4f, 1f);  // 보라

    private string cardId;
    public Action<string> onCardClick;

    void Start()
    {
        // 버튼 클릭 이벤트
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetCard(string cardId)
    {
        this.cardId = cardId;

        Card card = CardDatabase.Instance.GetCard(cardId);
        if (card == null)
        {
            Debug.LogError($"카드 {cardId}를 찾을 수 없습니다!");
            return;
        }

        // 카드 이름
        if (cardNameText != null)
            cardNameText.text = card.cardName;

        // 카드 타입
        if (cardTypeText != null)
            cardTypeText.text = $"[{card.cardType}]";

        // 배경색 설정
        if (backgroundImage != null)
        {
            switch (card.cardType)
            {
                case CardType.M:
                    backgroundImage.color = mCardColor;
                    break;
                case CardType.A:
                    backgroundImage.color = aCardColor;
                    break;
                case CardType.D:
                    backgroundImage.color = dCardColor;
                    break;
                case CardType.S:
                    backgroundImage.color = sCardColor;
                    break;
            }
        }

        // 공격력 표시 (M/A 카드만)
        if (attackValueText != null)
        {
            if (card.cardType == CardType.M || card.cardType == CardType.A)
            {
                attackValueText.text = $"공격: {card.attackValue}\n이득: {card.attackerGain}";
                attackValueText.gameObject.SetActive(true);
            }
            else
            {
                attackValueText.gameObject.SetActive(false);
            }
        }

        // 방어 정보 표시
        UpdateDefenseInfo(card);
    }

    void UpdateDefenseInfo(Card card)
    {
        if (defenseInfoText == null) return;

        // 현재 공격받고 있는지 확인
        if (CardGameManager.Instance.IsUnderAttack())
        {
            string attackCardId = CardGameManager.Instance.GetAttackCardId();
            Card attackCard = CardDatabase.Instance.GetCard(attackCardId);

            if (attackCard != null)
            {
                if (attackCard.fullDefenseCards.Contains(cardId))
                {
                    defenseInfoText.text = " 완전 방어";
                    defenseInfoText.color = Color.green;
                    defenseInfoText.gameObject.SetActive(true);
                }
                else if (attackCard.partialDefenseCards.Contains(cardId))
                {
                    defenseInfoText.text = " 부분 방어";
                    defenseInfoText.color = Color.yellow;
                    defenseInfoText.gameObject.SetActive(true);
                }
                else
                {
                    defenseInfoText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // 공격받지 않는 경우
            if (card.cardType == CardType.S)
            {
                // 특수 효과 표시
                switch (card.specialEffect)
                {
                    case SpecialEffect.PassAttack:
                        defenseInfoText.text = "공격 넘기기";
                        break;
                    case SpecialEffect.ReverseOrder:
                        defenseInfoText.text = "순서 반전";
                        break;
                    case SpecialEffect.AmplifyAttack:
                        defenseInfoText.text = "공격 증폭";
                        break;
                    default:
                        defenseInfoText.gameObject.SetActive(false);
                        return;
                }
                defenseInfoText.color = Color.magenta;
                defenseInfoText.gameObject.SetActive(true);
            }
            else
            {
                defenseInfoText.gameObject.SetActive(false);
            }
        }
    }

    void OnClick()
    {
        onCardClick?.Invoke(cardId);
    }
}
