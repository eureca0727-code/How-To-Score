using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardBattleUI : MonoBehaviour
{
    [Header("Game State Display")]
    public TextMeshProUGUI turnInfoText;           // "현재: 갑의 차례"
    public TextMeshProUGUI attackStatusText;       // "을이(가) M1(기자회견)으로 공격 중!"
    public TextMeshProUGUI actionCountText;        // "행동 횟수 - 갑:2/3, 을:1/3, 병:0/3"

    [Header("Player Card Display")]
    public Transform playerCardContainer;          // 플레이어 카드 버튼들의 부모
    public GameObject cardButtonPrefab;            // 카드 버튼 프리팹

    [Header("AI Feedback")]
    public TextMeshProUGUI aiActionText;           // "을이(가) M2(TV 인터뷰)를 사용했습니다!"
    public GameObject aiActionPanel;               // AI 행동 표시 패널

    [Header("Player Actions")]
    public Button passButton;                      // "턴 포기" 버튼

    [Header("Settings")]
    public float aiTurnDelay = 1.5f;              // AI 턴 딜레이
    public float aiActionDisplayTime = 2f;         // AI 행동 표시 시간

    // 내부 상태
    private List<GameObject> currentCardButtons = new List<GameObject>();
    private bool isProcessingTurn = false;         // 중복 클릭 방지

    void Start()
    {
        // 버튼 이벤트 등록
        if (passButton != null)
            passButton.onClick.AddListener(OnPassButtonClick);

        // AI 패널 초기 숨김
        if (aiActionPanel != null)
            aiActionPanel.SetActive(false);
    }

    // 배틀 시작
    public void StartBattle()
    {
        Debug.Log("카드 배틀 시작!");

        // CardGameManager 라운드 시작
        CardGameManager.Instance.StartRound();

        // UI 초기화
        UpdateAllUI();

        // 턴 처리 시작
        StartCoroutine(GameLoop());
    }

    // 게임 루프 (핵심 로직)
    IEnumerator GameLoop()
    {
        while (!CardGameManager.Instance.IsRoundEnd())
        {
            // UI 업데이트
            UpdateAllUI();

            // 현재 턴 확인
            var currentTurn = CardGameManager.Instance.currentTurn;

            if (currentTurn == CardGameManager.Party.Player)
            {
                // 플레이어 턴: 입력 대기
                yield return StartCoroutine(WaitForPlayerAction());
            }
            else
            {
                // AI 턴: 자동 실행
                yield return StartCoroutine(ExecuteAITurn());
            }
        }

        // 라운드 종료
        Debug.Log("카드 배틀 종료!");
        yield return new WaitForSeconds(1f);

        // VotingUI로 전환
        UIManager.Instance.ShowElectionView();
        VotingUI.Instance.StartVoting();
    }

    // 플레이어 턴 처리
    IEnumerator WaitForPlayerAction()
    {
        // 플레이어 카드 버튼 생성
        CreatePlayerCardButtons();

        // 버튼 활성화
        SetCardButtonsInteractable(true);
        if (passButton != null)
            passButton.interactable = true;

        isProcessingTurn = false;

        // 플레이어가 카드 선택하거나 턴 포기할 때까지 대기
        while (!isProcessingTurn)
        {
            yield return null;
        }

        // 버튼 비활성화
        SetCardButtonsInteractable(false);
        if (passButton != null)
            passButton.interactable = false;
    }

    // 카드 버튼 클릭 처리
    void OnCardButtonClick(string cardId)
    {
        if (isProcessingTurn) return;

        isProcessingTurn = true;

        Debug.Log($"플레이어가 {cardId} 카드를 선택했습니다!");

        // CardGameManager를 통해 카드 사용
        CardGameManager.Instance.PlayCard(cardId);

        // 카드 버튼 제거
        ClearPlayerCardButtons();
    }

    // 턴 포기 버튼 클릭
    void OnPassButtonClick()
    {
        if (isProcessingTurn) return;

        isProcessingTurn = true;

        Debug.Log("플레이어가 턴을 포기했습니다.");

        // CardGameManager를 통해 턴 포기
        CardGameManager.Instance.PassTurn();
        CardGameManager.Instance.NextTurn();

        // 카드 버튼 제거
        ClearPlayerCardButtons();
    }

    // AI 턴 처리
    IEnumerator ExecuteAITurn()
    {
        // AI 사고 시간 (시각적 효과)
        yield return new WaitForSeconds(aiTurnDelay);

        // AI 카드 선택 및 실행
        var currentTurn = CardGameManager.Instance.currentTurn;
        string partyName = CardGameManager.Instance.GetPartyName(currentTurn);

        // AI 실행 전 핸드 카드 수 확인
        PartyCardHand hand = CardGameManager.Instance.GetCurrentHand();
        string selectedCardId = null;

        if (currentTurn == CardGameManager.Party.PartyB)
        {
            selectedCardId = PartyAI.SelectCardForPartyB(
                hand,
                CardGameManager.Instance.IsUnderAttack(),
                CardGameManager.Instance.GetAttackCardId()
            );
        }
        else if (currentTurn == CardGameManager.Party.PartyC)
        {
            selectedCardId = PartyAI.SelectCardForPartyC(
                hand,
                CardGameManager.Instance.IsUnderAttack(),
                CardGameManager.Instance.GetAttackCardId()
            );
        }

        // AI 행동 표시
        if (selectedCardId != null)
        {
            Card card = CardDatabase.Instance.GetCard(selectedCardId);
            ShowAIAction($"{partyName}이(가) {card.cardName}을(를) 사용했습니다!");

            // CardGameManager를 통해 카드 사용
            CardGameManager.Instance.PlayCard(selectedCardId);
        }
        else
        {
            ShowAIAction($"{partyName}이(가) 턴을 포기했습니다.");

            // CardGameManager를 통해 턴 포기
            CardGameManager.Instance.PassTurn();
            CardGameManager.Instance.NextTurn();
        }

        // AI 행동 표시 시간
        yield return new WaitForSeconds(aiActionDisplayTime);

        // AI 패널 숨김
        HideAIAction();
    }

    // AI 행동 표시
    void ShowAIAction(string message)
    {
        if (aiActionPanel != null)
            aiActionPanel.SetActive(true);

        if (aiActionText != null)
            aiActionText.text = message;
    }

    // AI 패널 숨김
    void HideAIAction()
    {
        if (aiActionPanel != null)
            aiActionPanel.SetActive(false);
    }

    // UI 업데이트
    void UpdateAllUI()
    {
        UpdateTurnInfo();
        UpdateAttackStatus();
        UpdateActionCount();
    }

    // 턴 정보 업데이트
    void UpdateTurnInfo()
    {
        if (turnInfoText == null) return;

        var currentTurn = CardGameManager.Instance.currentTurn;
        string partyName = CardGameManager.Instance.GetPartyName(currentTurn);
        turnInfoText.text = $"현재: {partyName}의 차례";
    }

    // 공격 상태 업데이트
    void UpdateAttackStatus()
    {
        if (attackStatusText == null) return;

        if (CardGameManager.Instance.IsUnderAttack())
        {
            string attackCardId = CardGameManager.Instance.GetAttackCardId();
            Card attackCard = CardDatabase.Instance.GetCard(attackCardId);

            var currentTurn = CardGameManager.Instance.currentTurn;
            string defenderName = CardGameManager.Instance.GetPartyName(currentTurn);

            attackStatusText.text = $"{defenderName}이(가) {attackCard.cardName}(으)로 공격받는 중!";
            attackStatusText.color = Color.red;
        }
        else
        {
            attackStatusText.text = "공격 없음";
            attackStatusText.color = Color.gray;
        }
    }

    // 행동 횟수 업데이트
    void UpdateActionCount()
    {
        if (actionCountText == null) return;

        int playerCount = CardGameManager.Instance.playerActionCount;
        int partyBCount = CardGameManager.Instance.partyBActionCount;
        int partyCCount = CardGameManager.Instance.partyCActionCount;

        actionCountText.text = $"행동 횟수 - 갑:{playerCount}/3, 을:{partyBCount}/3, 병:{partyCCount}/3";
    }

    // 플레이어 카드 버튼 생성
    void CreatePlayerCardButtons()
    {
        // 기존 버튼 제거
        ClearPlayerCardButtons();

        // 플레이어 핸드에서 카드 가져오기
        PartyCardHand playerHand = CardGameManager.Instance.playerHand;

        if (playerHand.GetCardCount() == 0)
        {
            // 카드 없음: "턴 포기"만 활성화
            Debug.Log("플레이어 카드 없음. 턴 포기만 가능");
            if (passButton != null)
                passButton.interactable = true;
            return;
        }

        foreach (string cardId in playerHand.cardsInHand)
        {
            // 카드 버튼 생성
            GameObject cardButton = Instantiate(cardButtonPrefab, playerCardContainer);

            // 카드 정보 설정
            CardButtonUI cardButtonUI = cardButton.GetComponent<CardButtonUI>();
            if (cardButtonUI != null)
            {
                cardButtonUI.SetCard(cardId);
                cardButtonUI.onCardClick = OnCardButtonClick;
            }

            currentCardButtons.Add(cardButton);
        }
    }

    // 카드 버튼 제거
    void ClearPlayerCardButtons()
    {
        foreach (GameObject button in currentCardButtons)
        {
            Destroy(button);
        }
        currentCardButtons.Clear();
    }

    // 카드 버튼 활성화/비활성화
    void SetCardButtonsInteractable(bool interactable)
    {
        foreach (GameObject button in currentCardButtons)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null)
                btn.interactable = interactable;
        }
    }
}
