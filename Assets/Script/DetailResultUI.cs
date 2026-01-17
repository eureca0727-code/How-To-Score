using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailResultUI : MonoBehaviour
{
    [Header("UI")]
    public Button closeButton;
    public TextMeshProUGUI closeButtonText; 


    [Header("Colors")]
    public Color partyAColor = new Color(1f, 0f, 0f, 0.6f); // 빨강 반투명
    public Color partyBColor = new Color(0f, 1f, 0f, 0.6f); // 초록 반투명
    public Color partyCColor = new Color(0f, 0f, 1f, 0.6f); // 파랑 반투명

    public static DetailResultUI Instance;

    void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
    }

    public void ShowDetailResult()
    {
        // 버튼 텍스트 변경
        if (closeButtonText != null)
        {
            if (GameManager.Instance.IsGameOver())
            {
                closeButtonText.text = "엔딩 보기";
            }
            else
            {
                closeButtonText.text = "다음 라운드 진행하기";
            }
        }

        // Map의 각 Region에 색깔 적용
        ApplyWinnerColorsToMap();
    }
    void ApplyWinnerColorsToMap()
    {
        Region[] allRegions = GameManager.Instance.mapContainer.GetComponentsInChildren<Region>();

        foreach (var region in allRegions)
        {
            int districtId = region.districtId;
            var support = GameManager.Instance.GetDistrictSupport(districtId);

            Color winnerColor;

            if (support.partyA > support.partyB && support.partyA > support.partyC)
            {
                winnerColor = partyAColor; // 갑당
            }
            else if (support.partyB > support.partyA && support.partyB > support.partyC)
            {
                winnerColor = partyBColor; // 을당
            }
            else
            {
                winnerColor = partyCColor; // 병당
            }

            // Region의 SpriteRenderer에 색 적용
            SpriteRenderer sr = region.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = winnerColor;
            }
        }
    }

    void OnCloseButtonClick()
    {
        // 색깔 원래대로 (흰색)
        ResetMapColors();

        gameObject.SetActive(false);

        // 다음 라운드 시작
        GameManager.Instance.StartNewRound();

        // 지도 화면으로
        UIManager.Instance.ShowMapView();
    }

    void ResetMapColors()
    {
        Region[] allRegions = GameManager.Instance.mapContainer.GetComponentsInChildren<Region>();

        foreach (var region in allRegions)
        {
            SpriteRenderer sr = region.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 선거구 색으로 복원
                Color districtColor = DistrictManager.Instance.GetDistrictColor(region.districtId);
                sr.color = districtColor;
            }
        }
    }
}