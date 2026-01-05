using UnityEngine;
using UnityEditor;
using TMPro;

public class FontChanger : EditorWindow
{
    public TMP_FontAsset newFont;

    [MenuItem("Tools/Change All Fonts")]
    public static void ShowWindow()
    {
        GetWindow<FontChanger>("Font Changer");
    }

    void OnGUI()
    {
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("새 폰트", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("모든 폰트 변경"))
        {
            ChangeAllFonts();
        }
    }

    void ChangeAllFonts()
    {
        if (newFont == null)
        {
            Debug.LogError("폰트를 선택해주세요!");
            return;
        }

        TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        int count = 0;

        foreach (var text in allTexts)
        {
            if (text.gameObject.scene.name != null) // Scene 내 오브젝트만
            {
                Undo.RecordObject(text, "Change Font");
                text.font = newFont;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        Debug.Log($"{count}개의 텍스트 폰트를 변경했습니다.");
    }
}