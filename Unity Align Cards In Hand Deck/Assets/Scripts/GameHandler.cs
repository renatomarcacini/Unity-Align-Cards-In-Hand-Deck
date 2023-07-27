using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Hand hand1;
    [SerializeField] private Hand hand2;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Button addCardButton1;
    [SerializeField] private Button addCardButton2;

    private void Awake()
    {
        addCardButton1.onClick.AddListener(() => OnClickAddCardInHand(hand1));
        addCardButton2.onClick.AddListener(() => OnClickAddCardInHand(hand2));

        hand1.OnCardAdded += Hand_OnCardAdded;
        hand2.OnCardAdded += Hand_OnCardAdded;
    }

    private void Hand_OnCardAdded(Hand hand)
    {
        Debug.Log($"Card added: {hand.name}");
    }

    private void OnClickAddCardInHand(Hand hand)
    {
        Transform card = Instantiate(cardPrefab, transform.position, Quaternion.identity).transform;
        float durationMove = 0.3f;
        hand.AddCardOnHand(card, durationMove);
    }
}
