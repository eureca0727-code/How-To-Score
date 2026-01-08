using UnityEngine;
using TMPro;

public class PartyBarChart : MonoBehaviour
{
    [Header("Bar Sections")]
    public RectTransform barA;  // 왼쪽
    public RectTransform barB;  // 중간
    public RectTransform barC;  // 오른쪽

    [Header("Labels")]
    public TextMeshProUGUI labelA;
    public TextMeshProUGUI labelB;
    public TextMeshProUGUI labelC;

    public float totalWidth = 300f; // 전체 막대 너비

    // 퍼센트 버전
    public void UpdateChartPercent(int percentA, int percentB, int percentC)
    {
        UpdateChart(percentA, percentB, percentC, 100);
    }

    // 절대값 버전
    public void UpdateChart(int valueA, int valueB, int valueC)
    {
        int total = valueA + valueB + valueC;
        UpdateChart(valueA, valueB, valueC, total);
    }

    // 절대값 버전 - float (추가)
    public void UpdateChart(float valueA, float valueB, float valueC)
    {
        float total = valueA + valueB + valueC;
        UpdateChart(valueA, valueB, valueC, total);
    }


    // 공통 로직
    private void UpdateChart(float valueA, float valueB, float valueC, float total)
    {
        if (total == 0) total = 1;

        // 각 섹션의 너비 계산
        float widthA = (valueA / (float)total) * totalWidth;
        float widthB = (valueB / (float)total) * totalWidth;
        float widthC = (valueC / (float)total) * totalWidth;

        // 너비 설정
        barA.sizeDelta = new Vector2(widthA, barA.sizeDelta.y);
        barB.sizeDelta = new Vector2(widthB, barB.sizeDelta.y);
        barC.sizeDelta = new Vector2(widthC, barC.sizeDelta.y);

        // 위치 조정 (왼쪽부터 쌓기)
        barA.anchoredPosition = new Vector2(0, 0);
        barB.anchoredPosition = new Vector2(widthA, 0);
        barC.anchoredPosition = new Vector2(widthA + widthB, 0);

        // 라벨
        if (total == 100)
        {
            labelA.text = $"A: {valueA}%";
            labelB.text = $"B: {valueB}%";
            labelC.text = $"C: {valueC}%";
        }
        else
        {
            labelA.text = $"A: {valueA}";
            labelB.text = $"B: {valueB}";
            labelC.text = $"C: {valueC}";
        }
    }
}