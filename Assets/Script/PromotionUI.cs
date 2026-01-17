using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;   

    [Header("Charts")]
    public MultiSectionPieChart partyChart;        // 갑/을/병 지지도
    public PartyBarChart partyBarChart;            // 갑/을/병 막대 그래프 (추가)
    public MultiSectionPieChart partyA_PolicyChart; // 갑 지지자 정책 수요
    public MultiSectionPieChart partyB_PolicyChart; // 을 지지자 정책 수요
    public MultiSectionPieChart partyC_PolicyChart; // 병 지지자 정책 수요

    [Header("Quest System")]
    public GameObject questPanel;
    public TextMeshProUGUI questionText;
    public Button yesButton;
    public Button noButton;

    [Header("Event System")]
    public GameObject eventPanel;
    public TextMeshProUGUI supportChangeText; // 지지도 변화율 표시
    public TextMeshProUGUI eventText; // 이벤트 대사
    public Button startvoteButton; // 선거 시작 버튼


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
        if (promoteButton != null) promoteButton.onClick.AddListener(OnPromoteButtonClick);  // 추가
        if (startvoteButton != null) startvoteButton.onClick.AddListener(OnStartVoteButtonClick); // ← 추가


        // Quest Panel 초기에는 꺼두기
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
    }

    void InitializeQuests()
    {
        allQuests = new QuestData[]
        {
        new QuestData
        {
            question = "핵심 지지층을 위한 강경한 개혁안을 발표하시겠습니까?",
            yesResponse = "지지층의 결집으로 지지도가 크게 상승했다.",
            noResponse = "온건한 태도를 유지하여 현상을 유지했다."
        },
        new QuestData
        {
            question = "지역 유지들과의 밀착 행보를 통해 조직력을 강화하시겠습니까?",
            yesResponse = "지역 조직을 장악하여 선거구 정보를 확보했다.",
            noResponse = "조직 관리 소홀로 일부 지지자가 이탈했다."
        },
        new QuestData
        {
            question = "당의 핵심 공약을 재확인하는 대규모 집회를 개최하시겠습니까?",
            yesResponse = "집회 성공으로 선거운동 주도권을 잡았다.",
            noResponse = "조용한 행보를 택하며 상대 공격에 대비했다."
        },
        new QuestData
        {
            question = "타 정당의 실정을 비판하는 강력한 네거티브 공세를 시작할까요?",
            yesResponse = "상대 정당의 신뢰도를 깎아내리는 데 성공했다.",
            noResponse = "신사적인 대응으로 중도층의 호감을 얻었다."
        },
        new QuestData
        {
            question = "지지 지역의 오랜 숙원 사업인 랜드마크 건설을 약속하시겠습니까?",
            yesResponse = "지역 개발 기대로 압도적인 지지를 얻었다.",
            noResponse = "공약 부재에 실망한 주민들이 등을 돌렸다."
        },
        new QuestData
        {
            question = "청년층을 겨냥한 맞춤형 일자리 공약을 발표하시겠습니까?",
            yesResponse = "청년들의 열렬한 지지로 지지도가 상승했다.",
            noResponse = "별다른 반응 없이 일상이 유지되었다."
        },
        new QuestData
        {
            question = "타 정당 후보의 정책적 허점을 지적하는 토론회를 제안할까요?",
            yesResponse = "토론 제안을 통해 공세적인 위치를 선점했다.",
            noResponse = "내실을 다지며 상대의 공세에 대비했다."
        },
        new QuestData
        {
            question = "환경 단체의 탄소세 도입 요구를 전격 수용하시겠습니까?",
            yesResponse = "환경 중시 행보로 새로운 지지층을 확보했다.",
            noResponse = "환경 정책 부재로 시민 단체의 비판을 받았다."
        },
        new QuestData
        {
            question = "복지 예산 확대를 위해 세금 인상이 필요하다는 진실을 밝히겠습니까?",
            yesResponse = "정직한 태도에 지지율이 올랐으나 반발도 생겼다.",
            noResponse = "민감한 이슈를 피해 가며 안정을 택했다."
        },
        new QuestData
        {
            question = "지역 전통 시장을 방문하여 소상공인 지원책을 약속하시겠습니까?",
            yesResponse = "민생 행보가 좋은 평가를 받아 지지율이 올랐다.",
            noResponse = "현장 방문 없이 서류상 정책에 집중했다."
        },
        new QuestData
        {
            question = "군부대 방문을 통해 강력한 안보 정책을 강조하시겠습니까?",
            yesResponse = "안보 의지가 강조되며 이를 선호하는 지지층이 결집했다.",
            noResponse = "안보 이슈에서 다소 소극적인 인상을 남겼다."
        },
        new QuestData
        {
            question = "당 내 스캔들 의혹에 대해 당대표가 직접 사과해야 할까요?",
            yesResponse = "빠른 사과로 여론의 악화를 막아냈다.",
            noResponse = "섣부르게 대처하지 않고 철저하게 진상을 조사하며 역공을 준비했다."
        },
        new QuestData
        {
            question = "인기 연예인의 지원 유세 요청을 받아들이겠습니까?",
            yesResponse = "대중적 인지도를 끌어올릴 수 있는 기회가 생겼다.",
            noResponse = "정치적 전문성을 강조하며 외연 확장을 미뤘다."
        },
        new QuestData
        {
            question = "노년층을 위한 연금 인상 안을 긴급 공약으로 내거시겠습니까?",
            yesResponse = "노년층의 압도적인 지지를 이끌어냈다.",
            noResponse = "경쟁 정당이 해당 지지층을 공략하는 것을 허용했다."
        },
        new QuestData
        {
            question = "우리 정당의 상징색을 강조하는 전국 단위 캠페인을 벌일까요?",
            yesResponse = "브랜드 이미지가 강화되며 유용한 카드를 얻었다.",
            noResponse = "실리적인 캠페인으로 전환하여 카드를 확보했다."
        }
        };
    }

    // "홍보 진행하기" 버튼 클릭 시
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
            campaignChangeA = 30; campaignChangeB = -15; campaignChangeC = -15; //eventPanel에 표시할 값 저장
            selectedRegion.ChangeSupportRate(30, -15, -15);
            Debug.Log($"{selectedRegion.regionName} 선거운동 성공!");
        }
        else
        {
            // 실패: 갑 -30, 을/병 +15
            campaignChangeA = -30; campaignChangeB = 15; campaignChangeC = 15; //eventPanel에 표시할 값 저장
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
            campaignChangeA = 6; campaignChangeB = -3; campaignChangeC = -3; //eventPanel에 표시할 값 저장

            GameManager.Instance.ChangeAllRegionsSupport(6, -3, -3);
            Debug.Log("전국 선거운동 성공!");
        }
        else
        {
            // 실패: 모든 지역 갑 -6, 을/병 +3
            campaignChangeA = -6; campaignChangeB = 3; campaignChangeC = 3; //eventPanel에 표시할 값 저장
            GameManager.Instance.ChangeAllRegionsSupport(-6, 3, 3);
            Debug.Log("전국 선거운동 실패!");
        }
    }
    public void StartQuest()
    {
        float regionSupportA = selectedRegion.partyA.supportRate;
        Debug.Log($"지역 {selectedRegion.regionName}의 갑당 지지도: {regionSupportA}%");

        int randomIndex;
        // 갑당 지지도가 50 이상이면 0~5 (지지자 전용), 미만이면 5~14 (일반 질문만)
        if (regionSupportA >= 50)
        {
            randomIndex = Random.Range(0, 5); // 0~4
        }
        else
        {
            randomIndex = Random.Range(5, allQuests.Length); // 5~14
        }

        currentQuest = allQuests[randomIndex];
        ShowQuest(randomIndex);
    }

    void ShowQuest(int index)
    {
        questionText.text = currentQuest.question;
    }

    void OnYesButtonClick()
    {
        questPanel.SetActive(false);
        eventPanel.SetActive(true);
        eventText.text = currentQuest.yesResponse;
        ShowEventResult(campaignSuccess, campaignChangeA, campaignChangeB, campaignChangeC);
    }

    void OnNoButtonClick()
    {
        questPanel.SetActive(false);
        eventPanel.SetActive(true);
        eventText.text = currentQuest.noResponse;
        ShowEventResult(campaignSuccess, campaignChangeA, campaignChangeB, campaignChangeC);
    }

    // Start Promotion 버튼에서 호출
    public void ShowPromotionPanel()
    {
        // 하위 패널 전부 초기화
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
        UpdateCharts();
    }

    // 전국 모드로 설정 
    public void SetNationalMode()
    {
        isNationalMode = true;
        selectedRegion = null;
    }

    // 지역 모드로 설정 
    public void SetRegionalMode(Region region)
    {
        isNationalMode = false;
        selectedRegion = region;
    }

    void UpdateCharts()
    {
        if (isNationalMode)
        {
            ShowNationalData();
        }
        else
        {
            ShowRegionalData();
        }
    }

    void ShowNationalData()
    {
        titleText.text = "전국 지지도";

        // 1. 전국 갑/을/병 지지도
        float totalA = GameManager.Instance.GetTotalPartyASupport();
        float totalB = GameManager.Instance.GetTotalPartyBSupport();
        float totalC = GameManager.Instance.GetTotalPartyCSupport();
        partyChart.SetData3(totalA, totalB, totalC);

        // 2. 갑 지지자들의 정책 수요 (전국 평균)
        var partyA_Demand = CalculateNationalPolicyDemand("A");
        partyA_PolicyChart.SetData4(
            partyA_Demand.economy,
            partyA_Demand.welfare,
            partyA_Demand.security,
            partyA_Demand.environment
        );

        // 3. 을 지지자들의 정책 수요
        var partyB_Demand = CalculateNationalPolicyDemand("B");
        partyB_PolicyChart.SetData4(
            partyB_Demand.economy,
            partyB_Demand.welfare,
            partyB_Demand.security,
            partyB_Demand.environment
        );

        // 4. 병 지지자들의 정책 수요
        var partyC_Demand = CalculateNationalPolicyDemand("C");
        partyC_PolicyChart.SetData4(
            partyC_Demand.economy,
            partyC_Demand.welfare,
            partyC_Demand.security,
            partyC_Demand.environment
        );
        partyBarChart.UpdateChart(totalA, totalB, totalC); // 막대 그래프 업데이트 추가
    }

    void ShowRegionalData()
    {
        if (selectedRegion == null)
        {
            Debug.LogError("선택된 지역이 없습니다!");
            return;
        }

        titleText.text = $"{selectedRegion.regionName} 지지도";

        // 1. 해당 지역 갑/을/병 지지도 , debug 로그 나중에 지울것
        Debug.Log($"=== {selectedRegion.regionName} 지지도 ===");
        Debug.Log($"갑: {selectedRegion.partyA.supportRate}%");
        Debug.Log($"을: {selectedRegion.partyB.supportRate}%");
        Debug.Log($"병: {selectedRegion.partyC.supportRate}%");

        partyChart.SetData3(
            selectedRegion.partyA.supportRate,
            selectedRegion.partyB.supportRate,
            selectedRegion.partyC.supportRate
        );

        // 2. 갑 정책 수요
        Debug.Log($"=== 갑 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyA.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyA.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyA.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyA.policyDemand.environment}");

        partyA_PolicyChart.SetData4(
            selectedRegion.partyA.policyDemand.economy,
            selectedRegion.partyA.policyDemand.welfare,
            selectedRegion.partyA.policyDemand.security,
            selectedRegion.partyA.policyDemand.environment
        );

        // 3. 을 정책 수요
        Debug.Log($"=== 을 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyB.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyB.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyB.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyB.policyDemand.environment}");

        partyB_PolicyChart.SetData4(
            selectedRegion.partyB.policyDemand.economy,
            selectedRegion.partyB.policyDemand.welfare,
            selectedRegion.partyB.policyDemand.security,
            selectedRegion.partyB.policyDemand.environment
        );

        // 4. 병 정책 수요
        Debug.Log($"=== 병 지지자 정책 수요 ===");
        Debug.Log($"경제: {selectedRegion.partyC.policyDemand.economy}");
        Debug.Log($"복지: {selectedRegion.partyC.policyDemand.welfare}");
        Debug.Log($"안보: {selectedRegion.partyC.policyDemand.security}");
        Debug.Log($"환경: {selectedRegion.partyC.policyDemand.environment}");

        partyC_PolicyChart.SetData4(
            selectedRegion.partyC.policyDemand.economy,
            selectedRegion.partyC.policyDemand.welfare,
            selectedRegion.partyC.policyDemand.security,
            selectedRegion.partyC.policyDemand.environment
        );
        partyBarChart.UpdateChartPercent(
            selectedRegion.partyA.supportRate,
            selectedRegion.partyB.supportRate,
            selectedRegion.partyC.supportRate
        );
    }

    // 전국 정당별 정책 수요 평균 계산
    PolicyDemand CalculateNationalPolicyDemand(string party)
    {
        Region[] allRegions = GameManager.Instance.mapContainer.GetComponentsInChildren<Region>();

        int totalEconomy = 0;
        int totalWelfare = 0;
        int totalSecurity = 0;
        int totalEnvironment = 0;
        int totalSupport = 0;

        foreach (var region in allRegions)
        {
            int support = 0;
            PolicyDemand demand = null;

            switch (party)
            {
                case "A":
                    support = (region.population * region.partyA.supportRate) / 100;
                    demand = region.partyA.policyDemand;
                    break;
                case "B":
                    support = (region.population * region.partyB.supportRate) / 100;
                    demand = region.partyB.policyDemand;
                    break;
                case "C":
                    support = (region.population * region.partyC.supportRate) / 100;
                    demand = region.partyC.policyDemand;
                    break;
            }

            if (support > 0 && demand != null)
            {
                totalEconomy += demand.economy * support;
                totalWelfare += demand.welfare * support;
                totalSecurity += demand.security * support;
                totalEnvironment += demand.environment * support;
                totalSupport += support;
            }
        }

        PolicyDemand avgDemand = new PolicyDemand();
        if (totalSupport > 0)
        {
            avgDemand.economy = totalEconomy / totalSupport;
            avgDemand.welfare = totalWelfare / totalSupport;
            avgDemand.security = totalSecurity / totalSupport;
            avgDemand.environment = totalEnvironment / totalSupport;
        }

        return avgDemand;
    }

    public void ClosePromotionPanel()
    {
        gameObject.SetActive(false); // promotionPanel 대신 gameObject
    }

    // 선거운동 결과를 EventPanel에 표시
    public void ShowEventResult(bool isSuccess, int amountA, int amountB, int amountC)
    {
        string result = isSuccess ? "선거운동 성공!" : "선거운동 실패!";
        supportChangeText.text = $"{result}\n갑당 {amountA:+#;-#;0}%, 을당 {amountB:+#;-#;0}%, 병당 {amountC:+#;-#;0}%";
    }
    void OnStartVoteButtonClick()
    {
        UIManager.Instance.ShowElectionView();
        VotingUI.Instance.StartVoting();
    }

}