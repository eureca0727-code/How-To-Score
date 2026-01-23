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

        // 초기에는 맵 뷰만 표시
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
            // DebugUI.Instance가 null이면 FindAnyObjectByType으로 찾기
            if (DebugUI.Instance == null)
            {
                DebugUI debugUI = FindAnyObjectByType<DebugUI>(FindObjectsInactive.Include); // 비활성화된 것도 찾음
                if (debugUI != null)
                {
                    debugUI.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("DebugUI를 찾을 수 없습니다!");
                }
            }
            else
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

    // 전국 홍보
    public void ShowPromotionView()
    {
        ShowPromotionView(null);
    }

    // 특정 지역 홍보
    public void ShowPromotionView(Region targetRegion)
    {
        mapView.SetActive(false);
        promotionView.SetActive(true);

        // PromotionUI 찾기
        PromotionUI promotionUI = promotionView.GetComponent<PromotionUI>();
        if (promotionUI == null)
        {
            promotionUI = promotionView.GetComponentInChildren<PromotionUI>();
        }

        if (targetRegion != null)
        {
            Debug.Log($"{targetRegion.regionName} 지역 홍보 시작");
            promotionUI.SetRegionalMode(targetRegion);
        }
        else
        {
            Debug.Log("전국 홍보 시작");
            promotionUI.SetNationalMode();
        }

        // 차트 업데이트
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

        // DetailResultUI의 ShowDetailResult 호출
        detailResultView.GetComponent<DetailResultUI>().ShowDetailResult();
    }

    public void HideAllPanels()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        electionView.SetActive(false);
    }

}