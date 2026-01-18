using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    private Dictionary<string, Card> cardDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeCards();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeCards()
    {
        cardDictionary = new Dictionary<string, Card>();

        // M1: 언론 기사
        Card m1 = new Card("M1", "언론 기사", CardType.M);
        m1.attackValue = 30;
        m1.attackerGain = 20;
        m1.fullDefenseCards.AddRange(new[] { "M1", "M2" });
        m1.partialDefenseCards.AddRange(new[] { "M5", "D2" });
        cardDictionary.Add("M1", m1);

        // M2: TV 인터뷰
        Card m2 = new Card("M2", "TV 인터뷰", CardType.M);
        m2.attackValue = 24;
        m2.attackerGain = 16;
        m2.fullDefenseCards.AddRange(new[] { "M3", "D1" });
        cardDictionary.Add("M2", m2);

        // M3: 토론회
        Card m3 = new Card("M3", "토론회", CardType.M);
        m3.attackValue = 18;
        m3.attackerGain = 12;
        m3.fullDefenseCards.Add("M3"); // 같은 M3로만 막을 수 있음
        cardDictionary.Add("M3", m3);

        // M4: SNS
        Card m4 = new Card("M4", "SNS", CardType.M);
        m4.attackValue = 15;
        m4.attackerGain = 10;
        m4.fullDefenseCards.AddRange(new[] { "M1", "D2" });
        m4.partialDefenseCards.Add("M4");
        cardDictionary.Add("M4", m4);

        // M5: 의회 발언
        Card m5 = new Card("M5", "의회 발언", CardType.M);
        m5.attackValue = 9;
        m5.attackerGain = 6;
        m5.fullDefenseCards.AddRange(new[] { "D1", "M5" });
        m5.partialDefenseCards.Add("M4");
        cardDictionary.Add("M5", m5);

        // A1: 캠페인 슬로건
        Card a1 = new Card("A1", "캠페인 슬로건", CardType.A);
        a1.attackValue = 27;
        a1.attackerGain = 18;
        a1.fullDefenseCards.AddRange(new[] { "M1", "M2" });
        a1.partialDefenseCards.AddRange(new[] { "D2", "M4" });
        cardDictionary.Add("A1", a1);

        // A2: 현수막
        Card a2 = new Card("A2", "현수막", CardType.A);
        a2.attackValue = 12;
        a2.attackerGain = 8;
        a2.fullDefenseCards.Add("D1");
        a2.partialDefenseCards.AddRange(new[] { "D2", "M4" });
        cardDictionary.Add("A2", a2);

        // D1: 공식 선거운동 자료집
        Card d1 = new Card("D1", "공식 선거운동 자료집", CardType.D);
        cardDictionary.Add("D1", d1);

        // D2: 법적 대응
        Card d2 = new Card("D2", "법적 대응", CardType.D);
        cardDictionary.Add("D2", d2);

        // S1: 공격 넘기기
        Card s1 = new Card("S1", "공격 넘기기", CardType.S);
        s1.specialEffect = SpecialEffect.PassAttack;
        cardDictionary.Add("S1", s1);

        // S2: 순서 바꾸기
        Card s2 = new Card("S2", "순서 바꾸기", CardType.S);
        s2.specialEffect = SpecialEffect.ReverseOrder;
        cardDictionary.Add("S2", s2);

        // S3: 공격 증폭
        Card s3 = new Card("S3", "공격 증폭", CardType.S);
        s3.specialEffect = SpecialEffect.AmplifyAttack;
        cardDictionary.Add("S3", s3);
    }

    // 카드 ID로 카드 정보 가져오기
    public Card GetCard(string cardId)
    {
        if (cardDictionary.ContainsKey(cardId))
        {
            return cardDictionary[cardId];
        }
        Debug.LogWarning($"카드 {cardId}를 찾을 수 없습니다.");
        return null;
    }

    // 모든 카드 ID 리스트 가져오기
    public List<string> GetAllCardIds()
    {
        return new List<string>(cardDictionary.Keys);
    }
}