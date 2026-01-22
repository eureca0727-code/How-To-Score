using System;

[Serializable]
public class QuestData
{
    public string question;           // 질문

    // Yes 선택
    public string yesResponse;        // Yes 성공 시 텍스트
    public RewardType yesRewardType;  // Yes 보상 타입
    public string yesCardId;          // Yes 카드 ID (M1, A1 등)
    public int yesSupportChangeA;     // Yes 지지율 갑
    public int yesSupportChangeB;     // Yes 지지율 을
    public int yesSupportChangeC;     // Yes 지지율 병

    // No 선택
    public string noResponse;         // No 성공 시 텍스트
    public RewardType noRewardType;   // No 보상 타입
    public string noCardId;           // No 카드 ID
    public int noSupportChangeA;      // No 지지율 갑
    public int noSupportChangeB;      // No 지지율 을
    public int noSupportChangeC;      // No 지지율 병

    // 실패 패널티 (Yes/No 공통)
    public bool hasFail;              // 실패 패널티 있는지 여부
    public int failSupportChangeA;    // 실패 시 지지율 갑
    public int failSupportChangeB;    // 실패 시 지지율 을
    public int failSupportChangeC;    // 실패 시 지지율 병
}

public enum RewardType
{
    None,      // 보상 없음
    Card,      // 카드 보상
    Support    // 지지율 보상
}