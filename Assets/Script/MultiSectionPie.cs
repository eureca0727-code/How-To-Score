using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiSectionPieChart : MonoBehaviour
{
    public List<Image> sectionImages; // Inspector에서 순서대로 연결

    // =============================
    // PromotionUI에서 호출하는 메서드
    // =============================

    // 3분할 (정당 지지율)
    public void UpdateChart(float value1, float value2, float value3)
    {
        SetDataMultiple(new float[] { value1, value2, value3 });
    }

    // 4분할 (정책 선호)
    public void UpdateChart(int value1, int value2, int value3, int value4)
    {
        SetDataMultiple(new float[] { value1, value2, value3, value4 });
    }

    // =============================
    // 내부 처리
    // =============================

    void SetDataMultiple(float[] values)
    {
        // 총합 계산 (float로!)
        float total = 0f;
        foreach (float val in values)
        {
            total += val;
        }

        if (total <= 0f || values.Length > sectionImages.Count)
        {
            HideAll();
            return;
        }

        float currentAngle = 0f;

        for (int i = 0; i < values.Length; i++)
        {
            float percent = values[i] / total;

            sectionImages[i].fillAmount = percent;
            sectionImages[i].transform.localRotation =
                Quaternion.Euler(0, 0, -currentAngle);

            sectionImages[i].gameObject.SetActive(true);

            currentAngle += 360f * percent;
        }

        // 사용 안 하는 이미지 숨김
        for (int i = values.Length; i < sectionImages.Count; i++)
        {
            sectionImages[i].gameObject.SetActive(false);
        }
    }

    void HideAll()
    {
        foreach (var img in sectionImages)
        {
            img.fillAmount = 0f;
            img.gameObject.SetActive(false);
        }
    }
}