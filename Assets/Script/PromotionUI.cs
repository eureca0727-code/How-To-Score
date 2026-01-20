using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;   

    [Header("Charts")]
    public MultiSectionPieChart partyChart;        // ��/��/�� ������
    public PartyBarChart partyBarChart;            // ��/��/�� ���� �׷��� (�߰�)
    public MultiSectionPieChart partyA_PolicyChart; // �� ������ ��å ����
    public MultiSectionPieChart partyB_PolicyChart; // �� ������ ��å ����
    public MultiSectionPieChart partyC_PolicyChart; // �� ������ ��å ����

    [Header("Quest System")]
    public GameObject questPanel;
    public TextMeshProUGUI questionText;
    public Button yesButton;
    public Button noButton;

    [Header("Event System")]
    public GameObject eventPanel;
    public TextMeshProUGUI supportChangeText; // ������ ��ȭ�� ǥ��
    public TextMeshProUGUI eventText; // �̺�Ʈ ���
    public Button startvoteButton; // ���� ���� ��ư


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
        if (promoteButton != null) promoteButton.onClick.AddListener(OnPromoteButtonClick);  // �߰�
        if (startvoteButton != null) startvoteButton.onClick.AddListener(OnStartVoteButtonClick); // �� �߰�


        // Quest Panel �ʱ⿡�� ���α�
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
    }

    void InitializeQuests()
    {
        allQuests = new QuestData[]
        {
        new QuestData
        {
            question = "�ٽ� �������� ���� ������ �������� ��ǥ�Ͻðڽ��ϱ�?",
            yesResponse = "�������� �������� �������� ũ�� ����ߴ�.",
            noResponse = "�°��� �µ��� �����Ͽ� ������ �����ߴ�."
        },
        new QuestData
        {
            question = "���� ��������� ���� �ຸ�� ���� �������� ��ȭ�Ͻðڽ��ϱ�?",
            yesResponse = "���� ������ ����Ͽ� ���ű� ������ Ȯ���ߴ�.",
            noResponse = "���� ���� ��Ȧ�� �Ϻ� �����ڰ� ��Ż�ߴ�."
        },
        new QuestData
        {
            question = "���� �ٽ� ������ ��Ȯ���ϴ� ��Ը� ��ȸ�� �����Ͻðڽ��ϱ�?",
            yesResponse = "��ȸ �������� ���ſ �ֵ����� ��Ҵ�.",
            noResponse = "������ �ຸ�� ���ϸ� ��� ���ݿ� ����ߴ�."
        },
        new QuestData
        {
            question = "Ÿ ������ ������ �����ϴ� ������ �װ�Ƽ�� ������ �����ұ��?",
            yesResponse = "��� ������ �ŷڵ��� ��Ƴ����� �� �����ߴ�.",
            noResponse = "�Ż����� �������� �ߵ����� ȣ���� �����."
        },
        new QuestData
        {
            question = "���� ������ ���� ���� ����� ���帶ũ �Ǽ��� ����Ͻðڽ��ϱ�?",
            yesResponse = "���� ���� ���� �е����� ������ �����.",
            noResponse = "���� ���翡 �Ǹ��� �ֹε��� ���� ���ȴ�."
        },
        new QuestData
        {
            question = "û������ �ܳ��� ������ ���ڸ� ������ ��ǥ�Ͻðڽ��ϱ�?",
            yesResponse = "û����� ������ ������ �������� ����ߴ�.",
            noResponse = "���ٸ� ���� ���� �ϻ��� �����Ǿ���."
        },
        new QuestData
        {
            question = "Ÿ ���� �ĺ��� ��å�� ������ �����ϴ� ���ȸ�� �����ұ��?",
            yesResponse = "��� ������ ���� �������� ��ġ�� �����ߴ�.",
            noResponse = "������ ������ ����� ������ ����ߴ�."
        },
        new QuestData
        {
            question = "ȯ�� ��ü�� ź�Ҽ� ���� �䱸�� ���� �����Ͻðڽ��ϱ�?",
            yesResponse = "ȯ�� �߽� �ຸ�� ���ο� �������� Ȯ���ߴ�.",
            noResponse = "ȯ�� ��å ����� �ù� ��ü�� ������ �޾Ҵ�."
        },
        new QuestData
        {
            question = "���� ���� Ȯ�븦 ���� ���� �λ��� �ʿ��ϴٴ� ������ �����ڽ��ϱ�?",
            yesResponse = "������ �µ��� �������� �ö����� �ݹߵ� �����.",
            noResponse = "�ΰ��� �̽��� ���� ���� ������ ���ߴ�."
        },
        new QuestData
        {
            question = "���� ���� ������ �湮�Ͽ� �һ���� ����å�� ����Ͻðڽ��ϱ�?",
            yesResponse = "�λ� �ຸ�� ���� �򰡸� �޾� �������� �ö���.",
            noResponse = "���� �湮 ���� ������ ��å�� �����ߴ�."
        },
        new QuestData
        {
            question = "���δ� �湮�� ���� ������ �Ⱥ� ��å�� �����Ͻðڽ��ϱ�?",
            yesResponse = "�Ⱥ� ������ �����Ǹ� �̸� ��ȣ�ϴ� �������� �����ߴ�.",
            noResponse = "�Ⱥ� �̽����� �ټ� �ұ����� �λ��� �����."
        },
        new QuestData
        {
            question = "�� �� ��ĵ�� ��Ȥ�� ���� ���ǥ�� ���� ����ؾ� �ұ��?",
            yesResponse = "���� ����� ������ ��ȭ�� ���Ƴ´�.",
            noResponse = "���θ��� ��ó���� �ʰ� ö���ϰ� ������ �����ϸ� ������ �غ��ߴ�."
        },
        new QuestData
        {
            question = "�α� �������� ���� ���� ��û�� �޾Ƶ��̰ڽ��ϱ�?",
            yesResponse = "������ �������� ����ø� �� �ִ� ��ȸ�� �����.",
            noResponse = "��ġ�� �������� �����ϸ� �ܿ� Ȯ���� �̷��."
        },
        new QuestData
        {
            question = "������� ���� ���� �λ� ���� ��� �������� ���Žðڽ��ϱ�?",
            yesResponse = "������� �е����� ������ �̲���´�.",
            noResponse = "���� ������ �ش� �������� �����ϴ� ���� ����ߴ�."
        },
        new QuestData
        {
            question = "�츮 ������ ��¡���� �����ϴ� ���� ���� ķ������ ���ϱ��?",
            yesResponse = "�귣�� �̹����� ��ȭ�Ǹ� ������ ī�带 �����.",
            noResponse = "�Ǹ����� ķ�������� ��ȯ�Ͽ� ī�带 Ȯ���ߴ�."
        }
        };
    }

    // "ȫ�� �����ϱ�" ��ư Ŭ�� ��
    public void OnPromoteButtonClick()
    {
        // 90% ����, 10% ����
        bool isSuccess = Random.Range(0, 100) < 90;

        if (isNationalMode) // ���� ���ſ
        {
            ProcessNationalPromotion(isSuccess);
        }
        else // Ư�� ���� ���ſ
        {
            ProcessRegionalPromotion(isSuccess);
        }

        // ����Ʈ ����
        questPanel.SetActive(true);
        StartQuest();
    }

    // Ư�� ���� ���ſ ó��
    private void ProcessRegionalPromotion(bool isSuccess)
    {
        campaignSuccess = isSuccess;
        if (isSuccess)
        {
            // ����: �� +30, ��/�� -15
            campaignChangeA = 30; campaignChangeB = -15; campaignChangeC = -15; //eventPanel�� ǥ���� �� ����
            selectedRegion.ChangeSupportRate(30, -15, -15);
            Debug.Log($"{selectedRegion.regionName} ���ſ ����!");
        }
        else
        {
            // ����: �� -30, ��/�� +15
            campaignChangeA = -30; campaignChangeB = 15; campaignChangeC = 15; //eventPanel�� ǥ���� �� ����
            selectedRegion.ChangeSupportRate(-30, 15, 15);
            Debug.Log($"{selectedRegion.regionName} ���ſ ����!");
        }
    }

    // ���� ���ſ ó��
    private void ProcessNationalPromotion(bool isSuccess)
    {
        campaignSuccess = isSuccess;
        if (isSuccess)
        {
            // ����: ��� ���� �� +6, ��/�� -3
            campaignChangeA = 6; campaignChangeB = -3; campaignChangeC = -3; //eventPanel�� ǥ���� �� ����

            GameManager.Instance.ChangeAllRegionsSupport(6, -3, -3);
            Debug.Log("���� ���ſ ����!");
        }
        else
        {
            // ����: ��� ���� �� -6, ��/�� +3
            campaignChangeA = -6; campaignChangeB = 3; campaignChangeC = 3; //eventPanel�� ǥ���� �� ����
            GameManager.Instance.ChangeAllRegionsSupport(-6, 3, 3);
            Debug.Log("���� ���ſ ����!");
        }
    }
    public void StartQuest()
    {
        float regionSupportA = selectedRegion.partyA.supportRate;
        Debug.Log($"���� {selectedRegion.regionName}�� ���� ������: {regionSupportA}%");

        int randomIndex;
        // ���� �������� 50 �̻��̸� 0~5 (������ ����), �̸��̸� 5~14 (�Ϲ� ������)
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

    // Start Promotion ��ư���� ȣ��
    public void ShowPromotionPanel()
    {
        // ���� �г� ���� �ʱ�ȭ
        if (questPanel != null) questPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
        UpdateCharts();
    }

    // ���� ���� ���� 
    public void SetNationalMode()
    {
        isNationalMode = true;
        selectedRegion = null;
    }

    // ���� ���� ���� 
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
        titleText.text = "���� ������";

        // 1. ���� ��/��/�� ������
        float totalA = GameManager.Instance.GetTotalPartyASupport();
        float totalB = GameManager.Instance.GetTotalPartyBSupport();
        float totalC = GameManager.Instance.GetTotalPartyCSupport();
        partyChart.SetData3(totalA, totalB, totalC);

        // 2. �� �����ڵ��� ��å ���� (���� ���)
        var partyA_Demand = CalculateNationalPolicyDemand("A");
        partyA_PolicyChart.SetData4(
            partyA_Demand.economy,
            partyA_Demand.welfare,
            partyA_Demand.security,
            partyA_Demand.environment
        );

        // 3. �� �����ڵ��� ��å ����
        var partyB_Demand = CalculateNationalPolicyDemand("B");
        partyB_PolicyChart.SetData4(
            partyB_Demand.economy,
            partyB_Demand.welfare,
            partyB_Demand.security,
            partyB_Demand.environment
        );

        // 4. �� �����ڵ��� ��å ����
        var partyC_Demand = CalculateNationalPolicyDemand("C");
        partyC_PolicyChart.SetData4(
            partyC_Demand.economy,
            partyC_Demand.welfare,
            partyC_Demand.security,
            partyC_Demand.environment
        );
        partyBarChart.UpdateChart(totalA, totalB, totalC); // ���� �׷��� ������Ʈ �߰�
    }

    void ShowRegionalData()
    {
        if (selectedRegion == null)
        {
            Debug.LogError("���õ� ������ �����ϴ�!");
            return;
        }

        titleText.text = $"{selectedRegion.regionName} ������";

        // 1. �ش� ���� ��/��/�� ������ , debug �α� ���߿� �����
        Debug.Log($"=== {selectedRegion.regionName} ������ ===");
        Debug.Log($"��: {selectedRegion.partyA.supportRate}%");
        Debug.Log($"��: {selectedRegion.partyB.supportRate}%");
        Debug.Log($"��: {selectedRegion.partyC.supportRate}%");

        partyChart.SetData3(
            selectedRegion.partyA.supportRate,
            selectedRegion.partyB.supportRate,
            selectedRegion.partyC.supportRate
        );

        // 2. �� ��å ����
        Debug.Log($"=== �� ������ ��å ���� ===");
        Debug.Log($"����: {selectedRegion.partyA.policyDemand.economy}");
        Debug.Log($"����: {selectedRegion.partyA.policyDemand.welfare}");
        Debug.Log($"�Ⱥ�: {selectedRegion.partyA.policyDemand.security}");
        Debug.Log($"ȯ��: {selectedRegion.partyA.policyDemand.environment}");

        partyA_PolicyChart.SetData4(
            selectedRegion.partyA.policyDemand.economy,
            selectedRegion.partyA.policyDemand.welfare,
            selectedRegion.partyA.policyDemand.security,
            selectedRegion.partyA.policyDemand.environment
        );

        // 3. �� ��å ����
        Debug.Log($"=== �� ������ ��å ���� ===");
        Debug.Log($"����: {selectedRegion.partyB.policyDemand.economy}");
        Debug.Log($"����: {selectedRegion.partyB.policyDemand.welfare}");
        Debug.Log($"�Ⱥ�: {selectedRegion.partyB.policyDemand.security}");
        Debug.Log($"ȯ��: {selectedRegion.partyB.policyDemand.environment}");

        partyB_PolicyChart.SetData4(
            selectedRegion.partyB.policyDemand.economy,
            selectedRegion.partyB.policyDemand.welfare,
            selectedRegion.partyB.policyDemand.security,
            selectedRegion.partyB.policyDemand.environment
        );

        // 4. �� ��å ����
        Debug.Log($"=== �� ������ ��å ���� ===");
        Debug.Log($"����: {selectedRegion.partyC.policyDemand.economy}");
        Debug.Log($"����: {selectedRegion.partyC.policyDemand.welfare}");
        Debug.Log($"�Ⱥ�: {selectedRegion.partyC.policyDemand.security}");
        Debug.Log($"ȯ��: {selectedRegion.partyC.policyDemand.environment}");

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

    // ���� ���纰 ��å ���� ��� ���
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
        gameObject.SetActive(false); // promotionPanel ��� gameObject
    }

    // ���ſ ����� EventPanel�� ǥ��
    public void ShowEventResult(bool isSuccess, int amountA, int amountB, int amountC)
    {
        string result = isSuccess ? "���ſ ����!" : "���ſ ����!";
        supportChangeText.text = $"{result}\n���� {amountA:+#;-#;0}%, ���� {amountB:+#;-#;0}%, ���� {amountC:+#;-#;0}%";
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