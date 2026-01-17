using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("View Panels")]
    public GameObject mapView;
    public GameObject promotionView;
    public GameObject electionView;

    void Awake()
    {
        Instance = this;

        // 초기에는 지도 뷰만 표시
        mapView.SetActive(true);
        promotionView.SetActive(false);
        electionView.SetActive(false);
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

    public void ShowElectionView()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        electionView.SetActive(true);
    }

    public void HideAllPanels()
    {
        mapView.SetActive(false);
        promotionView.SetActive(false);
        electionView.SetActive(false);
    }

}