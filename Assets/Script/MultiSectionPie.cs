using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiSectionPieChart : MonoBehaviour
{
    public List<Image> sectionImages; // Inspector에서 순서대로 연결

    // 3분할용 (정당)
    public void SetData3(int value1, int value2, int value3)
    {
        SetDataMultiple(new int[] { value1, value2, value3 });
    }

    // 4분할용 (정책)
    public void SetData4(int value1, int value2, int value3, int value4)
    {

        SetDataMultiple(new int[] { value1, value2, value3, value4 });
    }

    // 범용 함수
    void SetDataMultiple(int[] values)
    {
        // 총합 계산
        int total = 0;
        foreach (int val in values)
        {
            total += val;
        }

        if (total == 0 || values.Length > sectionImages.Count)
        {
            HideAll();
            return;
        }

        float currentAngle = 0f;

        for (int i = 0; i < values.Length; i++)
        {
            float percent = (float)values[i] / total;

            sectionImages[i].fillAmount = percent;
            sectionImages[i].transform.localRotation = Quaternion.Euler(0, 0, -currentAngle);
            sectionImages[i].gameObject.SetActive(true);

            currentAngle += 360f * percent;
        }

        // 사용 안 하는 이미지는 숨김
        for (int i = values.Length; i < sectionImages.Count; i++)
        {
            sectionImages[i].gameObject.SetActive(false);
        }
    }

    void HideAll()
    {
        foreach (var img in sectionImages)
        {
            img.fillAmount = 0;
        }
    }

}
