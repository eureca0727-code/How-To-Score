using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // 텍스트 제어를 위해 추가

public class OpeningManager : MonoBehaviour
{
    [Header("메인 메뉴 UI")]
    public GameObject mainMenuPanel;
    public Button startButton;

    [Header("스토리 연출 UI")]
    public GameObject storyPanel;
    public Image storyImage;
    public TextMeshProUGUI storyText;
    public Sprite[] storySprites; // 여러 장의 삽화를 넣을 배열
    [TextArea(3, 10)]
    public string[] storyLines;   // 삽화에 맞는 설명글 배열

    [Header("씬 설정")]
    public string mainSceneName = "GameScene";

    private int currentStoryIndex = 0;

    void Start()
    {
        // 초기 세팅
        mainMenuPanel.SetActive(true);
        storyPanel.SetActive(false);
        startButton.onClick.AddListener(StartStorySequence);
    }

    // 1. 게임 시작 버튼을 누르면 호출됨
    public void StartStorySequence()
    {
        mainMenuPanel.SetActive(false);
        storyPanel.SetActive(true);
        ShowNextStory();
    }

    // 2. 스토리를 한 장씩 출력
    public void ShowNextStory()
    {
        if (currentStoryIndex < storyLines.Length)
        {
            // 삽화와 텍스트 교체
            if (storySprites.Length > currentStoryIndex)
                storyImage.sprite = storySprites[currentStoryIndex];

            storyText.text = storyLines[currentStoryIndex];
            currentStoryIndex++;
        }
        else
        {
            // 모든 스토리가 끝나면 게임 시작
            LoadGameScene();
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}