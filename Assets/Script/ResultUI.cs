using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("District Containers")]
    public Transform[] districtContainers; // District0~5 GameObject들

    [Header("Party Colors")]
    public Color partyAColor = Color.red;    // 갑당 빨강
    public Color partyBColor = Color.green;  // 을당 초록
    public Color partyCColor = Color.blue;   // 병당 파랑
    public Color defaultColor = Color.gray;  // 기본 회색

    [Header("Settings")]
    public float revealDelay = 0.5f; // 0.5초 간격

    void Start()
    {
        // 처음엔 모두 회색
        foreach (var container in districtContainers)
        {
            Image[] childImages = container.GetComponentsInChildren<Image>();
            foreach (var img in childImages)
            {
                img.color = defaultColor;
            }
        }
    }

    public void ShowResults()
    {
        StartCoroutine(RevealResults());
    }

    IEnumerator RevealResults()
    {
        yield return new WaitForSeconds(1f);

        // 각 선거구 결과 계산 및 표시
        for (int i = 0; i < 6; i++)
        {
            Debug.Log($"District {i} 처리 시작");

            if (districtContainers[i] == null)
            {
                Debug.LogError($"District {i}가 null입니다!");
                continue;
            }

            // 선거구 i의 당선자 계산
            var support = GameManager.Instance.GetDistrictSupport(i);

            // 최대 지지율 찾기
            Color winnerColor = defaultColor;

            if (support.partyA > support.partyB && support.partyA > support.partyC)
            {
                winnerColor = partyAColor; // 갑당 승리
            }
            else if (support.partyB > support.partyA && support.partyB > support.partyC)
            {
                winnerColor = partyBColor; // 을당 승리
            }
            else
            {
                winnerColor = partyCColor; // 병당 승리
            }

            // District i 안의 모든 Image(Triangle, Circle) 색깔 변경
            Image[] childImages = districtContainers[i].GetComponentsInChildren<Image>();
            foreach (var img in childImages)
            {
                img.color = winnerColor;
            }

            Debug.Log($"선거구 {i}: 갑={support.partyA:F1}% 을={support.partyB:F1}% 병={support.partyC:F1}% → 당선: {winnerColor}");

            // 0.5초 대기
            yield return new WaitForSeconds(revealDelay);
        }

        Debug.Log("선거 결과 표시 완료!");
    }
}