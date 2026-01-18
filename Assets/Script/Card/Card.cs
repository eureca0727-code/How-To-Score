using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public string cardId; // "M1", "M2", "A1", "D1", "S1" 등
    public string cardName; // "언론 기사", "TV 인터뷰" 등
    public CardType cardType; // M, A, D, S

    // 공격 관련
    public int attackValue; // 공격 수치
    public int attackerGain; // 공격자가 얻는 지지율 (괄호 안 숫자)

    // 방어 관련
    public List<string> fullDefenseCards; // 완전 방어 가능한 카드들
    public List<string> partialDefenseCards; // 일부 방어 가능한 카드들

    // 특수 효과
    public SpecialEffect specialEffect;

    public Card(string id, string name, CardType type)
    {
        cardId = id;
        cardName = name;
        cardType = type;
        fullDefenseCards = new List<string>();
        partialDefenseCards = new List<string>();
        specialEffect = SpecialEffect.None;
    }
}

public enum CardType
{
    M, // 공격/방어 겸용
    A, // 공격 전용
    D, // 방어 전용
    S  // 특수
}

public enum SpecialEffect
{
    None,
    PassAttack,      // S1: 공격 넘기기
    ReverseOrder,    // S2: 순서 바꾸기
    AmplifyAttack    // S3: 공격 증폭
}