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

    private bool campaignSuccess;
    private int campaignChangeA, campaignChangeB, campaignChangeC;

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
        new QuestData
        {
            question = "다시 대통령에 출마 의사를 공개적으로 발표하시겠습니까?",
            yesResponse = "명확한 비전으로 대중들의 큰 지지 받았다.",
            noResponse = "적당한 모호함 유지하여 논란을 피했다."
        },
        new QuestData
        {
            question = "최근 물가상승에 대한 책임을 직접 대중들에게 설명하시겠습니까?",
            yesResponse = "정면 돌파로 신뢰감을 보여줬다.",
            noResponse = "관료 차원 문제로 일부 유권자가 실망했다."
        },
        new QuestData
        {
            question = "노령 다시 위기에 대응하는 대담한 연금 개혁을 추진하시겠습니까?",
            yesResponse = "연금 개혁으로 선거운동 주도권을 잡았다.",
            noResponse = "보수적 책임을 비판받아 일부 노년층에 실망했다."
        },
        new QuestData
        {
            question = "타 정당의 후보를 비난하는 정치적 네거티브 광고를 시작할까요?",
            yesResponse = "네거티브 광고로 타 후보들의 약점 부각시켰다.",
            noResponse = "품격있는 선거로 중도층의 호감을 얻었다."
        },
        new QuestData
        {
            question = "노동 운동에 적극 협력 자세를 랜드마크 정책을 발표하시겠습니까?",
            yesResponse = "노조 협력 동맹 파트너로 확고한 얻었다.",
            noResponse = "경영 친화에 집중해 재계로부터 지원 끌어냈다."
        },
        new QuestData
        {
            question = "청년들에게 단호한 군복무 혜택을 보장해 발표하시겠습니까?",
            yesResponse = "청년층의 마음을 얻어서 지지층을 확보했다.",
            noResponse = "재원마련 우려 속에서 일부층에 혼란되었다."
        },
        new QuestData
        {
            question = "타 당의 스캔들 정책을 비판을 강조하는 토론회를 진행할까요?",
            yesResponse = "네거티브 전략의 여파 공격에서 우위를 점했다.",
            noResponse = "건설적 대안을 제시해 공정선거 호평을 받았다."
        },
        new QuestData
        {
            question = "환경 단체와 탄소중립 사업 요구에 적극 동참하시겠습니까?",
            yesResponse = "환경 중심 책임에 새로운 이미지를 확보했다.",
            noResponse = "환경 정책 유보로 일부 단체와 논란을 받았다."
        },
        new QuestData
        {
            question = "세금 인상 확대를 위해 빈부 인식을 강조하는 발표를 준비하시겠습니까?",
            yesResponse = "복지국 모토로 저소득층 확대에서 지지받았다.",
            noResponse = "인기영 이슈로 부자 세금 인상을 회피했다."
        },
        new QuestData
        {
            question = "현지 농업 지역을 방문하여 직접들어 복지정책을 홍보하시겠습니까?",
            yesResponse = "현장 책임에 있는 평가를 받아 지역민을 얻었다.",
            noResponse = "전략 방문 없이 농촌과 도시에 불평을 받았다."
        },
        new QuestData
        {
            question = "해외투자 방문을 통해 외국인 투자유치 정책을 추진하시겠습니까?",
            yesResponse = "투자유치 강조되면 이를 선호하는 기업인을 확보했다.",
            noResponse = "투자유치 이슈로 다소 국수주의자 인식을 받았다."
        },
        new QuestData
        {
            question = "큰 혼 스캔들 혐의에 정면 대응해야 할까요?",
            yesResponse = "강력 대응로 논란의 소화를 꺾어냈다.",
            noResponse = "무대응과 회피로 인해 철저하고 대응로 일부로 판단받았다."
        },
        new QuestData
        {
            question = "로컬 커뮤니티와 대화 토론 제안을 받아들이겠습니까?",
            yesResponse = "시민과 소통하여 신뢰를 쌓을 수 있는 연결을 얻었다.",
            noResponse = "정치적 리스크로 기피하면 언론 확산에 이미지 손실."
        },
        new QuestData
        {
            question = "교육개혁 패키지 정책 현장 설명 포럼을 추진하시겠습니까?",
            yesResponse = "개혁적 파트너들이 교육에 이끌어냈다.",
            noResponse = "부담 우려로 학교 그룹으로 반대하는 이슈 확산됐다."
        },
        new QuestData
        {
            question = "우리 정당의 장점을 강조하는 대형 미디어 캠페인을 시작할까요?",
            yesResponse = "브랜드 이미지가 강화되며 긍정적 카드를 보았다.",
            noResponse = "지나치게 캠페인으로 진환하여 카드를 확장했다."
        }
        };
    }

    // "홍보 시작하기" 버튼 클릭 시
    public void OnPromoteButtonClick()
    {
        // 90% 성공, 10% 실패
        bool isSuccess = Random.Range(0, 100) < 90;

        if (isNationalMode) // 전국 선거운동
        {
            ProcessNationalPromotion(isSuccess);
        }
        else // 특정 지역 선거운동
        {
            ProcessRegionalPromotion(isSuccess);
        }

        // 퀘스트 시작
        questPanel.SetActive(true);
        StartQuest();
    }

    // 특정 지역 선거운동 처리
    private void ProcessRegionalPromotion(bool isSuccess)
    {
        campaignSuccess = isSuccess;
        if (isSuccess)
        {
            // 성공: 갑 +30, 을/병 -15
            campaignChangeA = 30; campaignChangeB = -15; campaignChangeC = -15;
            selectedRegion.ChangeSupportRate(30, -15, -15);
            Debug.Log($"{selectedRegion.regionName} 선거운동 성공!");
        }
        else
        {
            // 실패: 갑 -30, 을/병 +15
            campaignChangeA = -30; campaignChangeB = 15; campaignChangeC = 15;
            selectedRegion.ChangeSupportRate(-30, 15, 15);
            Debug.Log($"{selectedRegion.regionName} 선거운동 실패!");
        }
    }

    // 전국 선거운동 처리
    private void ProcessNationalPromotion(bool isSuccess)
    {
        campaignSuccess = isSuccess;
        if (isSuccess)
        {
            // 성공: 모든 지역 갑 +6, 을/병 -3
            campaignChangeA = 6; campaignChangeB = -3; campaignChangeC = -3;
            GameManager.Instance.ChangeAllRegionsSupport(6, -3, -3);
            Debug.Log("전국 선거운동 성공!");
        }
        else
        {
            // 실패: 모든 지역 갑 -6, 을/병 +3
            campaignChangeA = -6; campaignChangeB = 3; campaignChangeC = 3;
            GameManager.Instance.ChangeAllRegionsSupport(-6, 3, 3);
            Debug.Log("전국 선거운동 실패!");
        }
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
        eventText.text = currentQuest.yesResponse;
        questPanel.SetActive(false);
        eventPanel.SetActive(true);
    }

    void OnNoButtonClick()
    {
        eventText.text = currentQuest.noResponse;
        questPanel.SetActive(false);
        eventPanel.SetActive(true);
    }

    // Start Promotion 버튼에서 호출
    public void ShowPromotionPanel()
    {
        // 차트 패널 상태 초기화
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
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
            partyA_PolicyChart.UpdateChart(policyA.Economy, policyA.Education, policyA.Welfare, policyA.Environment, policyA.Industry);
        }

        // 3. 을 지지자들의 정책 선호
        var policyB = GameManager.Instance.GetNationalPolicyDemandForPartyB();
        if (partyB_PolicyChart != null && policyB != null)
        {
            partyB_PolicyChart.UpdateChart(policyB.Economy, policyB.Education, policyB.Welfare, policyB.Environment, policyB.Industry);
        }

        // 4. 병 지지자들의 정책 선호
        var policyC = GameManager.Instance.GetNationalPolicyDemandForPartyC();
        if (partyC_PolicyChart != null && policyC != null)
        {
            partyC_PolicyChart.UpdateChart(policyC.Economy, policyC.Education, policyC.Welfare, policyC.Environment, policyC.Industry);
        }

        partyBarChart.UpdateChart(totalA, totalB, totalC); // 막대 그래프 업데이트 추가
    }

    void UpdateChartsForRegion(Region region)
    {
        if (region == null)
        {
            Debug.LogError("UpdateChartsForRegion: region is null!");
            return;
        }

        // 1. 해당 지역 갑/을/병 지지율
        float rateA = region.partyA.supportRate;
        float rateB = region.partyB.supportRate;
        float rateC = region.partyC.supportRate;

        if (partyChart != null)
        {
            partyChart.UpdateChart(rateA, rateB, rateC);
        }

        // 2. 갑 정책 선호
        var policyA = CalculatePolicyDemandForSupporter(region, rateA);
        if (partyA_PolicyChart != null)
        {
            partyA_PolicyChart.UpdateChart(
                policyA.Economy,
                policyA.Education,
                policyA.Welfare,
                policyA.Environment,
                policyA.Industry
            );
        }

        // 3. 을 정책 선호
        var policyB = CalculatePolicyDemandForSupporter(region, rateB);
        if (partyB_PolicyChart != null)
        {
            partyB_PolicyChart.UpdateChart(
                policyB.Economy,
                policyB.Education,
                policyB.Welfare,
                policyB.Environment,
                policyB.Industry
            );
        }

        // 4. 병 정책 선호
        var policyC = CalculatePolicyDemandForSupporter(region, rateC);
        if (partyC_PolicyChart != null)
        {
            partyC_PolicyChart.UpdateChart(
                policyC.Economy,
                policyC.Education,
                policyC.Welfare,
                policyC.Environment,
                policyC.Industry
            );
        }

        partyBarChart.UpdateChart(rateA, rateB, rateC);
    }

    // 지역 지지자별 정책 선호 계산 함수
    PolicyDemand CalculatePolicyDemandForSupporter(Region region, float supportRate)
    {
        PolicyDemand baseDemand = region.policyDemand;

        float weight = supportRate / 100f;

        PolicyDemand result = new PolicyDemand
        {
            Economy = baseDemand.Economy * weight,
            Education = baseDemand.Education * weight,
            Welfare = baseDemand.Welfare * weight,
            Environment = baseDemand.Environment * weight,
            Industry = baseDemand.Industry * weight
        };

        return result;
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
    void OnStartVoteButtonClick()
    {
        // 카드 분배 로직
        CardDistributor distributor = FindObjectOfType<CardDistributor>();
        if (distributor != null)
        {
            // 0. 플레이어 카드 수집 초기화
            distributor.InitializePlayerCardCollection();

            // 1. 홍보 선택 카드 추가 (0-1장)
            distributor.AddPromotionCard(isNationalMode, selectedRegion);

            // 2. 선거 운동 결과 카드 추가 (2-3장)
            distributor.AddCampaignResultCards(campaignSuccess);

            // 3. 최종 카드 확정 (playerHand에 추가)
            distributor.FinalizePlayerCards();

            Debug.Log($"[PromotionUI] 카드 분배 완료 - 선택: {(isNationalMode ? "전국" : selectedRegion?.regionName)}, 결과: {(campaignSuccess ? "성공" : "실패")}");
        }
        else
        {
            Debug.LogError("CardDistributor를 찾을 수 없습니다!");
        }

        // 카드 배틀 UI로 전환
        UIManager.Instance.ShowCardBattleView();

        // VotingUI는 CardBattleUI 종료 후 자동으로 호출됨
    }

}

[System.Serializable]
public class QuestData
{
    public string question;
    public string yesResponse;
    public string noResponse;
}
