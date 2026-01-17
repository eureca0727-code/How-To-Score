using System.Collections;
using UnityEngine;
using TMPro;

public class VotingUI : MonoBehaviour
{
    public static VotingUI Instance;

    [Header("UI References")]
    public GameObject votingPanel;
    public TextMeshProUGUI votingText;
    public GameObject electionPanel;

    [Header("Settings")]
    public float votingDuration = 3f; // 투표 화면 표시 시간
    public float dotAnimationSpeed = 0.5f; // 점 변경 속도

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    // 투표 시작
    public void StartVoting()
    {
        electionPanel.SetActive(false);
        // 투표 패널만 활성화
        votingPanel.SetActive(true);

        // 애니메이션 시작
        StartCoroutine(VotingAnimation());
    }

    private IEnumerator VotingAnimation()
    {
        float elapsedTime = 0f;
        int dotCount = 0;
        float nextDotTime = 0f;

        while (elapsedTime < votingDuration)
        {
            // 점 애니메이션 (투표 중, 투표 중., 투표 중.., 투표 중...)
            if (elapsedTime >= nextDotTime)
            {
                dotCount = (dotCount % 3) + 1; // 1, 2, 3 반복
                string dots = new string('.', dotCount);
                votingText.text = $"투표 중{dots}";
                nextDotTime = elapsedTime + dotAnimationSpeed;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 투표 완료 후
        votingPanel.SetActive(false); // VotingPanel 끄기
        electionPanel.SetActive(true); // ElectionPanel 켜기

        // 결과 애니메이션 시작
        electionPanel.GetComponent<ResultUI>().ShowResults();

        Debug.Log("투표 완료!");
    }
}