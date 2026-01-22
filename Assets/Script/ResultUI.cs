using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 씬 재시작용

public class ResultUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI statusText;
    public Button nextButton; // → 버튼 (성공 시)
    public Button restartButton; // 처음으로 버튼 (실패 시)

    [Header("District Containers")]
    public Transform[] districtContainers;

    [Header("Party Colors")]
    public Color partyAColor = Color.red;
    public Color partyBColor = Color.green;
    public Color partyCColor = Color.blue;
    public Color defaultColor = Color.gray;

    [Header("Settings")]
    public float revealDelay = 0.5f;

    void Start()
    {
        // 처음엔 텍스트 숨기기
        if (resultText != null) resultText.text = "";
        if (statusText != null) statusText.text = "";

        // 처음엔 버튼 숨기기
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        // 버튼 클릭 이벤트 연결
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClick);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartButtonClick);

        // 처음엔 모두 회색
        ResetDistrictColors();
    }

    public void ShowResults()
    {
        // 타이틀 업데이트
        int round = GameManager.Instance.GetCurrentRound();
        titleText.text = $"<{round}회 선거 결과>";

        // 텍스트 초기화
        resultText.text = "";
        statusText.text = "";

        // 버튼 초기화
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        // District 이미지 색깔 초기화
        ResetDistrictColors();

        StartCoroutine(RevealResults());
    }

    // District 색깔 초기화
    void ResetDistrictColors()
    {
        foreach (var container in districtContainers)
        {
            if (container != null)
            {
                Image[] childImages = container.GetComponentsInChildren<Image>();
                foreach (var img in childImages)
                {
                    img.color = defaultColor;
                }
            }
        }
    }

    IEnumerator RevealResults()
    {
        // 패널 켜진 후 1초 대기
        yield return new WaitForSeconds(1f);

        int partyASeats = 0; // 갑당 의석수 카운트

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
                partyASeats++; // 갑당 의석 +1
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

        // 모든 색깔이 나타난 후
        resultText.text = $"총 6석 중 {partyASeats}석을 획득하였습니다.";

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 의석 결과 카드 분배
        CardDistributor distributor = FindObjectOfType<CardDistributor>();
        if (distributor != null)
        {
            distributor.AddSeatRewardCards(partyASeats);
            distributor.FinalizePlayerCards();
            Debug.Log($"[ResultUI] 카드 분배 완료 - {partyASeats}석");
        }
        else
        {
            Debug.LogError("CardDistributor를 찾을 수 없습니다!");
        }

        // 현재 라운드 가져오기
        int currentRound = GameManager.Instance.GetCurrentRound();

        // 성공/실패 판정 (2라운드부터만 0석일 때 실패)
        bool isSuccess = (currentRound == 1) ? true : (partyASeats > 0);

        if (isSuccess)
        {
            statusText.text = "의석 획득 성공";
            statusText.color = Color.red;
        }
        else
        {
            statusText.text = "의석 획득 실패";
            statusText.color = Color.red;
        }

        // 1초 대기 후 버튼 표시
        yield return new WaitForSeconds(1f);

        if (isSuccess)
        {
            nextButton.gameObject.SetActive(true); // → 버튼 표시
        }
        else
        {
            restartButton.gameObject.SetActive(true); // 처음으로 버튼 표시
        }

        Debug.Log("선거 결과 표시 완료!");
    }
    // → 버튼 클릭 시 (성공)
    void OnNextButtonClick()
    {
        Debug.Log("세부 결과 화면으로 이동");

        // UIManager를 통해 전환
        UIManager.Instance.ShowDetailResultView();
    }

    // 처음으로 버튼 클릭 시 (실패)
    void OnRestartButtonClick()
    {
        Debug.Log("게임 첫 화면으로");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}