using UnityEngine;
using System.Collections.Generic;

public class DistrictManager : MonoBehaviour
{
    public static DistrictManager Instance;

    private Color[] districtColors = new Color[]
    {
        new Color(1f, 0.7f, 0.7f),    // A - 연한 빨강
        new Color(0.7f, 0.7f, 1f),    // B - 연한 파랑
        new Color(0.7f, 1f, 0.7f),    // C - 연한 초록
        new Color(1f, 1f, 0.7f),      // D - 연한 노랑
        new Color(1f, 0.7f, 1f),      // E - 연한 보라
        new Color(0.7f, 1f, 1f)       // F - 연한 청록
    };

    // 7가지 선거구 패턴 (0=A, 1=B, 2=C, 3=D, 4=E, 5=F)
    // 각 배열은 12개 타일의 선거구 ID (0행: ABCD, 1행: EFGH, 2행: IJKL)
    private int[][] districtPatterns = new int[][]
    {
        // 패턴 1: AA BB / CC DD / EE FF
        new int[] {0,0,1,1,  2,2,3,3,  4,4,5,5},
    
        // 패턴 2: AA BC / DE BC / DE FF
        new int[] {0,0,1,2,  3,4,1,2,  3,4,5,5},
    
        // 패턴 3: AB BC / AD DC / EE FF
        new int[] {0,1,1,2,  0,3,3,2,  4,4,5,5},
    
        // 패턴 4: AB CC / AB DD / EE FF
        new int[] {0,1,2,2,  0,1,3,3,  4,4,5,5},
    
        // 패턴 5: AA BC / DD BC / EE FF
        new int[] {0,0,1,2,  3,3,1,2,  4,4,5,5},
    
    
        // 패턴 6: AB CD / AB CD / EE FF
        new int[] {0,1,2,3,  0,1,2,3,  4,4,5,5},
    
    
        // 패턴 7: AA BB / CD DE / CF FE
        new int[] {0,0,1,1,  2,3,3,4,  2,5,5,4},
    
    };

    void Awake()
    {
        Instance = this;
    }

    public void AssignRandomDistricts(Region[] allRegions)
    {
        // 7가지 중 랜덤 선택
        int randomPattern = Random.Range(0, districtPatterns.Length);
        int[] pattern = districtPatterns[randomPattern]; 

        // 각 지역에 선거구 ID와 색상 할당
        for (int i = 0; i < allRegions.Length; i++)
        {
            int districtId = pattern[i];
            allRegions[i].districtId = districtId;
            allRegions[i].GetComponent<SpriteRenderer>().color = districtColors[districtId];
        }
    }

    // 선거구 색상 반환(다음 라운드 진행용)
    public Color GetDistrictColor(int districtId)
    {
        if (districtId >= 0 && districtId < districtColors.Length)
        {
            return districtColors[districtId];
        }

        Debug.LogError($"잘못된 districtId: {districtId}");
        return Color.white;
    }
}