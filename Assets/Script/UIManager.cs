using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("View Panels")]
    public GameObject mapView;
    public GameObject promotionView;  

    void Awake()
    {
        Instance = this;
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

        if (targetRegion != null)
        {
            Debug.Log($"{targetRegion.regionName} 지역 홍보 시작");
            // TODO: 해당 지역 타겟팅
        }
        else
        {
            Debug.Log("전국 홍보 시작");
            // TODO: 전국 타겟팅
        }
    }
}