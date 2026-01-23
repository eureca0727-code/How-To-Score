using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;

    [Header("Charts")]
    public MultiSectionPieChart partyChart;        // 갑/을/병 지지율
    public PartyBarChart partyBarChart;            // 갑/을/병 막대 그래프 (추가)
    public MultiSectionPieChart partyA_PolicyChart; // 갑 정당의 정책 선호
    public MultiSectionPieChart partyB_PolicyChart; // 을 정당의 정책 선호
    public MultiSectionPieChart partyC_PolicyChart; // 병 정당의 정책 선호

    [Header("Quest System")]
    public GameObject questPanel;
    public TextMeshProUGUI questionText;
    public Button yesButton;
    public Button noButton;

    [Header("Event System")]
    public GameObject eventPanel;
    public TextMeshProUGUI supportChangeText; // 지지율 변화량 표시
    public TextMeshProUGUI eventText; // 이벤트 내용
    public Button startvoteButton; // 투표 시작 버튼


    private bool isNationalMode = true;
    private Region selectedRegion = null;
    private QuestData[] allQuests;
    public Button promoteButton;
    private QuestData currentQuest;

    private string questCardReward = ""; // 획득한 카드 ID


    void Start()
    {
        InitializeQuests();

        if (yesButton != null) yesButton.onClick.AddListener(OnYesButtonClick);
        if (noButton != null) noButton.onClick.AddListener(OnNoButtonClick);
        if (promoteButton != null) promoteButton.onClick.AddListener(OnPromoteButtonClick);
        if (startvoteButton != null) startvoteButton.onClick.AddListener(OnStartVoteButtonClick);


        // Quest Panel 초기에는 숨김
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
    }
    void InitializeQuests()
    {
        allQuests = new QuestData[]
        {
            // Quest 1
            new QuestData
            {
                question = "핵심 지지층을 위한 강경한 개혁안을 발표하시겠습니까?",
                yesResponse = "지지층의 결집으로 지지도가 크게 상승했다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 20, yesSupportChangeB = -10, yesSupportChangeC = -10,
                noResponse = "온건한 태도를 유지하여 현상을 유지했다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 2
            new QuestData
            {
                question = "지역 유지들과의 밀착 행보를 통해 조직력을 강화하시겠습니까?",
                yesResponse = "지역 조직을 장악하여 선거구 정보를 확보했다.",
                yesRewardType = RewardType.Card,
                yesCardId = "S1",
                yesSupportChangeA = 0, yesSupportChangeB = 0, yesSupportChangeC = 0,
                noResponse = "조직 관리 소홀로 일부 지지자가 이탈했다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = true,
                failSupportChangeA = -10, failSupportChangeB = 5, failSupportChangeC = 5
            },
            
            // Quest 3
            new QuestData
            {
                question = "당의 핵심 공약을 재확인하는 대규모 집회를 개최하시겠습니까?",
                yesResponse = "집회 성공으로 선거운동 주도권을 잡았다.",
                yesRewardType = RewardType.Card,
                yesCardId = "M1",
                yesSupportChangeA = 0, yesSupportChangeB = 0, yesSupportChangeC = 0,
                noResponse = "조용한 행보를 택하며 상대 공격에 대비했다.",
                noRewardType = RewardType.Card,
                noCardId = "D1",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 4
            new QuestData
            {
                question = "타 정당의 실정을 비판하는 강력한 네거티브 공세를 시작할까요?",
                yesResponse = "상대 정당의 신뢰도를 깎아내리는 데 성공했다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 10, yesSupportChangeB = -5, yesSupportChangeC = -5,
                noResponse = "신사적인 대응으로 중도층의 호감을 얻었다.",
                noRewardType = RewardType.Support,
                noCardId = "",
                noSupportChangeA = 5, noSupportChangeB = -2, noSupportChangeC = -3,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 5
            new QuestData
            {
                question = "지지 지역의 오랜 숙원 사업인 랜드마크 건설을 약속하시겠습니까?",
                yesResponse = "지역 개발 기대로 압도적인 지지를 얻었다.",
                yesRewardType = RewardType.Support,
                yesCardId = "S3",
                yesSupportChangeA = 10, yesSupportChangeB = -5, yesSupportChangeC = -5,
                noResponse = "공약 부재에 실망한 주민들이 등을 돌렸다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = true,
                failSupportChangeA = -10, failSupportChangeB = 5, failSupportChangeC = 5
            },
            
            // Quest 6
            new QuestData
            {
                question = "청년층을 겨냥한 맞춤형 일자리 공약을 발표하시겠습니까?",
                yesResponse = "청년들의 열렬한 지지로 지지도가 상승했다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 15, yesSupportChangeB = -7, yesSupportChangeC = -8,
                noResponse = "별다른 반응 없이 일상이 유지되었다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 7
            new QuestData
            {
                question = "타 정당 후보의 정책적 허점을 지적하는 토론회를 제안할까요?",
                yesResponse = "토론 제안을 통해 공세적인 위치를 선점했다.",
                yesRewardType = RewardType.Card,
                yesCardId = "A1",
                yesSupportChangeA = 0, yesSupportChangeB = 0, yesSupportChangeC = 0,
                noResponse = "내실을 다지며 상대의 공세에 대비했다.",
                noRewardType = RewardType.Card,
                noCardId = "D2",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 8
            new QuestData
            {
                question = "환경 단체의 탄소세 도입 요구를 전격 수용하시겠습니까?",
                yesResponse = "환경 중시 행보로 새로운 지지층을 확보했다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 10, yesSupportChangeB = -5, yesSupportChangeC = -5,
                noResponse = "환경 정책 부재로 시민 단체의 비판을 받았다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = true,
                failSupportChangeA = -5, failSupportChangeB = 5, failSupportChangeC = 0
            },
            
            // Quest 9
            new QuestData
            {
                question = "복지 예산 확대를 위해 세금 인상이 필요하다는 진실을 밝히겠습니까?",
                yesResponse = "정직한 태도에 지지율이 올랐으나 반발도 생겼다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 5, yesSupportChangeB = 5, yesSupportChangeC = -10,
                noResponse = "민감한 이슈를 피해 가며 안정을 택했다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 10
            new QuestData
            {
                question = "지역 전통 시장을 방문하여 소상공인 지원책을 약속하시겠습니까?",
                yesResponse = "민생 행보가 좋은 평가를 받아 지지율이 올랐다.",
                yesRewardType = RewardType.Support,
                yesCardId = "M2",
                yesSupportChangeA = 10, yesSupportChangeB = -10, yesSupportChangeC = 0,
                noResponse = "현장 방문 없이 서류상 정책에 집중했다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 11
            new QuestData
            {
                question = "군부대 방문을 통해 강력한 안보 정책을 강조하시겠습니까?",
                yesResponse = "안보 의지가 강조되며 이를 선호하는 지지층이 결집했다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 12, yesSupportChangeB = 0, yesSupportChangeC = -12,
                noResponse = "안보 이슈에서 다소 소극적인 인상을 남겼다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 12
            new QuestData
            {
                question = "당 내 스캔들 의혹에 대해 당대표가 직접 사과해야 할까요?",
                yesResponse = "빠른 사과로 여론의 악화를 막아냈다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 10, yesSupportChangeB = -10, yesSupportChangeC = 0,
                noResponse = "섣부르게 대처하지 않고 철저하게 진상을 조사하며 역공을 준비했다.",
                noRewardType = RewardType.Card,
                noCardId = "D1",
                noSupportChangeA = -10, noSupportChangeB = 10, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 13
            new QuestData
            {
                question = "인기 연예인의 지원 유세 요청을 받아들이겠습니까?",
                yesResponse = "대중적 인지도를 끌어올릴 수 있는 기회가 생겼다.",
                yesRewardType = RewardType.Card,
                yesCardId = "S2",
                yesSupportChangeA = 0, yesSupportChangeB = 0, yesSupportChangeC = 0,
                noResponse = "정치적 전문성을 강조하며 외연 확장을 미뤘다.",
                noRewardType = RewardType.None,
                noCardId = "",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 14
            new QuestData
            {
                question = "노년층을 위한 연금 인상 안을 긴급 공약으로 내거시겠습니까?",
                yesResponse = "노년층의 압도적인 지지를 이끌어냈다.",
                yesRewardType = RewardType.Support,
                yesCardId = "",
                yesSupportChangeA = 10, yesSupportChangeB = -5, yesSupportChangeC = -5,
                noResponse = "경쟁 정당이 해당 지지층을 공략하는 것을 허용했다.",
                noRewardType = RewardType.Support,
                noCardId = "",
                noSupportChangeA = -5, noSupportChangeB = 10, noSupportChangeC = -5,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            },
            
            // Quest 15
            new QuestData
            {
                question = "우리 정당의 상징색을 강조하는 전국 단위 캠페인을 벌일까요?",
                yesResponse = "브랜드 이미지가 강화되며 유용한 카드를 얻었다.",
                yesRewardType = RewardType.Card,
                yesCardId = "M3",
                yesSupportChangeA = 0, yesSupportChangeB = 0, yesSupportChangeC = 0,
                noResponse = "실리적인 캠페인으로 전환하여 카드를 확보했다.",
                noRewardType = RewardType.Card,
                noCardId = "M4",
                noSupportChangeA = 0, noSupportChangeB = 0, noSupportChangeC = 0,
                hasFail = false,
                failSupportChangeA = 0, failSupportChangeB = 0, failSupportChangeC = 0
            }
        };
    }


    // "홍보 시작하기" 버튼 클릭 시
    public void OnPromoteButtonClick()
    {
        // Quest 바로 시작
        questPanel.SetActive(true);
        StartQuest();
    }



    // 선거 지지도 자연 증가
    public float regionalSupportBonus = 0;
    public float nationalSupportBonus = 0;

    void StartQuest()
    {
        currentQuest = allQuests[Random.Range(0, allQuests.Length)];
        questionText.text = currentQuest.question;
    }

    void OnYesButtonClick()
    {
        ProcessQuestChoice(true);
    }

    void OnNoButtonClick()
    {
        ProcessQuestChoice(false);
    }
    void ProcessQuestChoice(bool isYes)
    {
        // 90% 성공, 10% 실패
        bool questSuccess = Random.Range(0, 100) < 90;

        Debug.Log($"선택: {(isYes ? "Yes" : "No")}, 성공 여부: {questSuccess}");

        if (questSuccess)
        {
            // 성공
            string responseText = isYes ? currentQuest.yesResponse : currentQuest.noResponse;
            eventText.text = $"<b>선거운동 성공!</b>\n\n{responseText}";

            if (isYes)
            {
                ProcessReward(
                    currentQuest.yesRewardType,
                    currentQuest.yesCardId,
                    currentQuest.yesSupportChangeA,
                    currentQuest.yesSupportChangeB,
                    currentQuest.yesSupportChangeC
                );
            }
            else
            {
                ProcessReward(
                    currentQuest.noRewardType,
                    currentQuest.noCardId,
                    currentQuest.noSupportChangeA,
                    currentQuest.noSupportChangeB,
                    currentQuest.noSupportChangeC
                );
            }

            Debug.Log("ProcessRegionalPromotion(true) 호출 직전");
            ProcessRegionalPromotion(true);
            Debug.Log("ProcessRegionalPromotion(true) 호출 완료");
        }
        else
        {
            Debug.Log("Quest 실패 처리 시작");

            // 실패
            if (currentQuest.hasFail)
            {
                eventText.text = "선거 운동 실패! 여론이 악화되었습니다.";
                questCardReward = "";

                // 실패 패널티 (지지율 감소)
                if (isNationalMode)
                {
                    GameManager.Instance.ChangeAllRegionsSupport(
                        currentQuest.failSupportChangeA,
                        currentQuest.failSupportChangeB,
                        currentQuest.failSupportChangeC
                    );
                }
                else
                {
                    selectedRegion.ChangeSupportRate(
                        currentQuest.failSupportChangeA,
                        currentQuest.failSupportChangeB,
                        currentQuest.failSupportChangeC
                    );
                }

                supportChangeText.text = $"지지율 하락!\n갑당 {currentQuest.failSupportChangeA}%, 을당 {currentQuest.failSupportChangeB:+#;-#;0}%, 병당 {currentQuest.failSupportChangeC:+#;-#;0}%";
                Debug.Log($"Quest 실패 - 지지율 패널티: 갑{currentQuest.failSupportChangeA}, 을{currentQuest.failSupportChangeB}, 병{currentQuest.failSupportChangeC}");
            }
            else
            {
                eventText.text = "선거 운동 실패! 아무 일도 일어나지 않았습니다.";
                questCardReward = "";
                supportChangeText.text = "변화 없음";
                Debug.Log("Quest 실패 - 패널티 없음");
            }

            ProcessRegionalPromotion(false);
        }


        questPanel.SetActive(false);
        eventPanel.SetActive(true);
    }
    void ProcessReward(RewardType type, string cardId, int changeA, int changeB, int changeC)
    {
        if (type == RewardType.Card)
        {
            // 카드 보상
            questCardReward = cardId;
            supportChangeText.text = $"카드 획득!\n{GetCardName(cardId)}";
            Debug.Log($"Quest 성공 - 카드 보상: {cardId}");
        }
        else if (type == RewardType.Support)
        {
            // 지지율 보상
            questCardReward = "";

            if (isNationalMode)
            {
                GameManager.Instance.ChangeAllRegionsSupport(changeA, changeB, changeC);
            }
            else
            {
                selectedRegion.ChangeSupportRate(changeA, changeB, changeC);
            }

            supportChangeText.text = $"지지율 변화\n갑당 {changeA:+#;-#;0}%, 을당 {changeB:+#;-#;0}%, 병당 {changeC:+#;-#;0}%";
            Debug.Log($"Quest 성공 - 지지율 보상: 갑{changeA}, 을{changeB}, 병{changeC}");
        }
        else // RewardType.None
        {
            // 보상 없음
            questCardReward = "";
            supportChangeText.text = "변화 없음";
            Debug.Log("Quest 성공 - 보상 없음");
        }
    }
    string GetCardName(string cardId)
    {
        Card card = CardDatabase.Instance.GetCard(cardId);
        return card != null ? card.cardName : cardId;
    }
    // Start Promotion 버튼에서 호출
    public void ShowPromotionPanel()
    {
        // 차트 패널 상태 초기화
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
        UpdateTitleText();

        UpdateChartsForMode();
    }

    // 전국 모드 설정
    public void SetNationalMode()
    {
        isNationalMode = true;
        selectedRegion = null;
    }

    // 지역 모드 설정
    public void SetRegionalMode(Region region)
    {
        isNationalMode = false;
        selectedRegion = region;
    }
    // titleText 업데이트
    void UpdateTitleText()
    {
        if (titleText == null) return;

        if (isNationalMode)
        {
            titleText.text = "전국 홍보";
        }
        else
        {
            if (selectedRegion != null)
            {
                titleText.text = $"{selectedRegion.regionName} 지역 홍보";
            }
            else
            {
                titleText.text = "지역 홍보";
            }
        }
    }
    void UpdateChartsForMode()
    {
        if (isNationalMode)
        {
            UpdateChartsForNational();
        }
        else
        {
            UpdateChartsForRegion(selectedRegion);
        }
    }

    void UpdateChartsForNational()
    {
        // 1. 전국 갑/을/병 지지율
        float totalA = GameManager.Instance.GetNationalSupportRateA();
        float totalB = GameManager.Instance.GetNationalSupportRateB();
        float totalC = GameManager.Instance.GetNationalSupportRateC();

        partyChart.UpdateChart(totalA, totalB, totalC);

        // 2. 갑 지지자들의 정책 선호 (전국 평균)
        var policyA = GameManager.Instance.GetNationalPolicyDemandForPartyA();
        if (partyA_PolicyChart != null && policyA != null)
        {
            partyA_PolicyChart.UpdateChart(
                policyA.economy,      // Economy → economy
                policyA.security,     // Security → security
                policyA.welfare,      // Welfare → welfare
                policyA.environment   // Environment → environment
            );
        }

        // 3. 을 지지자들의 정책 선호
        var policyB = GameManager.Instance.GetNationalPolicyDemandForPartyB();
        if (partyB_PolicyChart != null && policyB != null)
        {
            partyB_PolicyChart.UpdateChart(
                policyB.economy,      // Economy → economy
                policyB.security,     // Security → security
                policyB.welfare,      // Welfare → welfare
                policyB.environment   // Environment → environment
            );
        }

        // 4. 병 지지자들의 정책 선호
        var policyC = GameManager.Instance.GetNationalPolicyDemandForPartyC();
        if (partyC_PolicyChart != null && policyC != null)
        {
            partyC_PolicyChart.UpdateChart(
                policyC.economy,      // Economy → economy
                policyC.security,     // Security → security
                policyC.welfare,      // Welfare → welfare
                policyC.environment   // Environment → environment
            );
        }

        partyBarChart.UpdateChart(totalA, totalB, totalC);
    }

    void UpdateChartsForRegion(Region region)
    {
        if (region == null)
        {
            Debug.LogError("UpdateChartsForRegion: region is null!");
            return;
        }

        // 해당 지역 갑 을 병 지지율
        float rateA = region.partyA.supportRate;
        float rateB = region.partyB.supportRate;
        float rateC = region.partyC.supportRate;

        if (partyChart != null)
        {
            partyChart.UpdateChart(rateA, rateB, rateC);
        }

        // 갑 지지자들의 정책 선호
        if (partyA_PolicyChart != null)
        {
            var policyA = region.partyA.policyDemand;
            partyA_PolicyChart.UpdateChart(
                policyA.economy,
                policyA.security,
                policyA.welfare,
                policyA.environment
            );
        }

        // 을 지지자들의 정책 선호
        if (partyB_PolicyChart != null)
        {
            var policyB = region.partyB.policyDemand;
            partyB_PolicyChart.UpdateChart(
                policyB.economy,
                policyB.security,
                policyB.welfare,
                policyB.environment
            );
        }

        // 병 지지자들의 정책 선호
        if (partyC_PolicyChart != null)
        {
            var policyC = region.partyC.policyDemand;
            partyC_PolicyChart.UpdateChart(
                policyC.economy,
                policyC.security,
                policyC.welfare,
                policyC.environment
            );
        }

        partyBarChart.UpdateChart(rateA, rateB, rateC);
    }

    public void ClosePromotionPanel()
    {
        gameObject.SetActive(false); // promotionPanel 자체 gameObject
    }

    // 선거운동 결과를 EventPanel에 표시
    public void ShowEventResult(bool isSuccess, int amountA, int amountB, int amountC)
    {
        string result = isSuccess ? "선거운동 성공!" : "선거운동 실패!";
        supportChangeText.text = $"{result}\n갑당 {amountA:+#;-#;0}%, 을당 {amountB:+#;-#;0}%, 병당 {amountC:+#;-#;0}%";
    }
    // 선거운동 결과에 따라 카드 추가
    void ProcessRegionalPromotion(bool isSuccess)
    {
        Debug.Log($">>>>>> ProcessRegionalPromotion 진입: isSuccess={isSuccess}");

        CardDistributor distributor = FindAnyObjectByType<CardDistributor>();
        if (distributor == null)
        {
            Debug.LogError("CardDistributor를 찾을 수 없습니다!");
            return;
        }

        Debug.Log("CardDistributor 찾음, AddCampaignResultCards 호출 직전");

        distributor.AddCampaignResultCards(isSuccess);

        Debug.Log($"<<<<<<< ProcessRegionalPromotion 완료: {(isSuccess ? 3 : 2)}장 추가됨");
    }
    void OnStartVoteButtonClick()
    {
        CardDistributor distributor = FindAnyObjectByType<CardDistributor>();
        if (distributor != null)
        {

            // Quest 보상 카드 추가
            if (!string.IsNullOrEmpty(questCardReward))
            {
                distributor.AddSpecificCard(questCardReward);
                Debug.Log($"[PromotionUI] Quest 보상 카드: {questCardReward}");
            }

            // 의석 보상 카드 추가 (TODO: 나중에 구현)
            // int partyASeats = VotingUI.Instance.GetLastPartyASeats();
            // distributor.AddSeatRewardCards(partyASeats);

            distributor.FinalizePlayerCards();
        }
        else
        {
            Debug.LogError("CardDistributor를 찾을 수 없습니다!");
        }

        UIManager.Instance.ShowCardBattleView();
    }
}
