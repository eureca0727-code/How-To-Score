using UnityEngine;

public class Region : MonoBehaviour
{
    [Header("지역 정보")]
    public string regionName = "A";
    public int population; // 주민 수 (5~15)
    public int supportGap;   // 갑 지지율
    public int supportEul;   // 을 지지율
    public int supportByung; // 병 지지율

    [Header("색상 설정")]
    public Color normalColor = new Color(0.5f, 0.7f, 0.9f);      // 옅은 푸른색
    public Color hoverColor = new Color(0.7f, 0.85f, 1f);        // 밝은 푸른색
    public Color selectedColor = new Color(0.9f, 0.95f, 1f);     // 매우 밝은 푸른색

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = normalColor;

        // 초기 데이터 설정 (나중에 GameManager에서 할 예정)
        InitializeRandomData();
    }

    void InitializeRandomData()
    {
        population = Random.Range(5, 16);
        supportGap = Random.Range(0, 101);
        int remaining = 100 - supportGap;
        supportEul = Random.Range(0, remaining + 1);
        supportByung = 100 - supportGap - supportEul;
    }

    // 마우스 올렸을 때
    void OnMouseEnter()
    {
        if (!isSelected)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    // 마우스 뗐을 때
    void OnMouseExit()
    {
        if (!isSelected)
        {
            spriteRenderer.color = normalColor;
        }
    }

    // 클릭했을 때
    void OnMouseDown()
    {
        Select();
    }

    public void Select()
    {
        // 다른 지역 선택 해제
        Region[] allRegions = FindObjectsOfType<Region>();
        foreach (Region region in allRegions)
        {
            region.Deselect();
        }

        isSelected = true;
        spriteRenderer.color = selectedColor;

        // 정보 출력 (임시로 콘솔에)
        Debug.Log($"=== {regionName}지역 정보 ===");
        Debug.Log($"주민 수: {population}00만 명");
        Debug.Log($"갑 지지율: {supportGap}%");
        Debug.Log($"을 지지율: {supportEul}%");
        Debug.Log($"병 지지율: {supportByung}%");

        // TODO: 나중에 UI 정보창에 표시
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = normalColor;
    }
}