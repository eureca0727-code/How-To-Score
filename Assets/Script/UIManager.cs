using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("View Panels")]
    public GameObject mapView;
    public GameObject promotionView;
    public GameObject cardBattleView;
    public GameObject electionView;
    public GameObject detailResultView; 


    void Awake()
    {
        Instance = this;

        // �ʱ⿡�� ���� �丸 ǥ��
        mapView.SetActive(true);
        promotionView.SetActive(false);
        cardBattleView.SetActive(false);
        electionView.SetActive(false);
        detailResultView.SetActive(false); 

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (DebugUI.Instance != null)
            {
                DebugUI.Instance.gameObject.SetActive(!DebugUI.Instance.gameObject.activeSelf);
            }
        }
    }

    public void ShowMapView()
    {
        mapView.SetActive(true);
        promotionView.SetActive(false);
    }

    // ���� ȫ��
    public void ShowPromotionView()
    {
        ShowPromotionView(null);
    }

    // Ư�� ���� ȫ��
    public void ShowPromotionView(Region targetRegion)
    {
        mapView.SetActive(false);
        promotionView.SetActive(true);

        // PromotionUI ã��
        PromotionUI promotionUI = promotionView.GetComponent<PromotionUI>();
        if (promotionUI == null)
        {
            promotionUI = promotionView.GetComponentInChildren<PromotionUI>();
        }

        if (targetRegion != null)
        {
            Debug.Log($"{targetRegion.regionName} ���� ȫ�� ����");
            promotionUI.SetRegionalMode(targetRegion);
        }
        else
        {
            Debug.Log("���� ȫ�� ����");
            promotionUI.SetNationalMode();
        }

        // ��Ʈ ������Ʈ
        promotionUI.ShowPromotionPanel();
    }

    public void ShowCardBattleView()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        cardBattleView.SetActive(true);
        electionView.SetActive(false);
        detailResultView.SetActive(false);

        // CardBattleUI 시작
        CardBattleUI battleUI = cardBattleView.GetComponent<CardBattleUI>();
        if (battleUI != null)
        {
            battleUI.StartBattle();
        }
    }

    public void ShowElectionView()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        cardBattleView.SetActive(false);
        electionView.SetActive(true);
    }
    public void ShowDetailResultView()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        electionView.SetActive(false);
        detailResultView.SetActive(true);

        // DetailResultUI�� ShowDetailResult ȣ��
        detailResultView.GetComponent<DetailResultUI>().ShowDetailResult();
    }

    public void HideAllPanels()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        electionView.SetActive(false);
    }

}