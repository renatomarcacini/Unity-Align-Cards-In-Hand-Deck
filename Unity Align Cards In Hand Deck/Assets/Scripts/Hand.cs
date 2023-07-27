using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Hand : MonoBehaviour
{
    [Header("Hand Settings")]
    [Space(10)]

    [SerializeField] private bool automaticOrganizeHand = false;
    [SerializeField] List<Transform> cards = new List<Transform>();

    [SerializeField] private float horizontalRadius = 5f;
    [SerializeField] private float maxHorizontalRadius = 50f;
    [SerializeField] private float horizontalRadiusGapModifier = 0.75f;
    [SerializeField] private float verticalRadius = 3f;
    [SerializeField] private float maxVerticalRadius = 1f;
    [SerializeField] private float verticalRadiusGapModifier = 0.5f;

    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float endAngle = 180f;

    [Header("Cards Rotation Focus Settings")]
    [Space(10)]
    [SerializeField] private Vector3 angleFocusOffset = new Vector3(0, -5f, 0);
    [SerializeField] private float rotationModifier = -90f;

    public event Action<Hand> OnCardAdded;

    [Header("Debug")]
    [Space(10)]
    [SerializeField] private bool debugUpdate = false;
    [SerializeField] private bool debugGizmo = false;

    private void Update()
    {
        if (debugUpdate)
            DebugUpdate();
    }

    public void AddCardOnHand(Transform card, float duration = 1)
    {
        card.SetParent(transform);
        cards.Add(card);

        if(!debugUpdate)
            PlaceCardsInElipse(duration);
    }

    private void DebugUpdate()
    {
        float angleStep = (endAngle - startAngle) / (cards.Count - 1);
        float currentAngle = startAngle;

        //Apply automatic gap between cards in hand
        if (automaticOrganizeHand)
        {
            float horizontalGapModifier = maxHorizontalRadius / (maxHorizontalRadius * horizontalRadiusGapModifier);
            horizontalRadius = cards.Count * horizontalGapModifier;

            if (horizontalRadius >= maxHorizontalRadius)
                horizontalRadius = maxHorizontalRadius;

            float verticalGapModifier = maxVerticalRadius / (maxVerticalRadius * verticalRadiusGapModifier);
            verticalRadius = cards.Count * verticalGapModifier;

            if (verticalRadius >= maxVerticalRadius)
                verticalRadius = maxVerticalRadius;
        }


        for (int i = 0; i < cards.Count; i++)
        {
            // Angles and Positions in elipse
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            float xPosition = 0.1f * horizontalRadius * Mathf.Cos(angleInRadians);
            float yPosition = 0.1f * verticalRadius * Mathf.Sin(angleInRadians);

            Vector3 cardSlot = new Vector3(xPosition, yPosition, -(0.01f * i));
            cards[i].position = Vector3.Lerp(cards[i].position, transform.position + cardSlot, 10 * Time.deltaTime);
            currentAngle += angleStep;

            //Rotate in circular way
            Vector3 focusPosition = transform.position - angleFocusOffset;
            Vector3 direction = focusPosition - cards[i].position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationModifier;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            cards[i].rotation = Quaternion.Slerp(cards[i].rotation, rotation, 10 * Time.deltaTime);
        }
    }

    private void PlaceCardsInElipse(float duration)
    {
        float angleStep = (endAngle - startAngle) / (cards.Count - 1);
        float currentAngle = startAngle;
        if (automaticOrganizeHand)
        {
            float horizontalGapModifier = maxHorizontalRadius / (maxHorizontalRadius * horizontalRadiusGapModifier);
            horizontalRadius = cards.Count * horizontalGapModifier;

            if (horizontalRadius >= maxHorizontalRadius)
                horizontalRadius = maxHorizontalRadius;

            float verticalGapModifier = maxVerticalRadius / (maxVerticalRadius * verticalRadiusGapModifier);
            verticalRadius = cards.Count * verticalGapModifier;

            if (verticalRadius >= maxVerticalRadius)
                verticalRadius = maxVerticalRadius;
        }


        Sequence sequenceMoveToSlot = DOTween.Sequence();
        for (int i = 0; i < cards.Count; i++)
        {
            // Angles and Positions in elipse
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            float xPosition = 0.1f * horizontalRadius * Mathf.Cos(angleInRadians);
            float yPosition = 0.1f * verticalRadius * Mathf.Sin(angleInRadians);

            Vector3 cardSlot = new Vector3(xPosition, yPosition, -(0.01f * i));
            Vector2 slotPosition = transform.position + cardSlot;
            currentAngle += angleStep;
            sequenceMoveToSlot.Join(cards[i].DOMove(slotPosition, duration));
        }

        sequenceMoveToSlot.OnComplete(() => {

            //Rotate in circular way
            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 focusPosition = transform.position - angleFocusOffset;
                Vector3 direction = focusPosition - cards[i].position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationModifier;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                cards[i].rotation = rotation;
            }
       
            OnCardAdded?.Invoke(this);
        });
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (debugGizmo)
        {
            Vector3 focusPosition = transform.position - angleFocusOffset;
            Gizmos.DrawSphere(focusPosition, 0.3f);
            for (int i = 0; i < cards.Count; i++)
            {
                Gizmos.DrawLine(cards[i].position, focusPosition);
            }

            Gizmos.color = Color.green;
            float angleStep = (endAngle - startAngle) / 50;

            for (int i = 0; i < 50; i++)
            {
                float angle = startAngle + i * angleStep;
                float angleInRadians = angle * Mathf.Deg2Rad;
                float x = transform.position.x + 0.1f * horizontalRadius * Mathf.Cos(angleInRadians);
                float y = transform.position.y + 0.1f * verticalRadius * Mathf.Sin(angleInRadians);

                Vector3 point = new Vector3(x, y, transform.position.z);
                Gizmos.DrawSphere(point, 0.05f); // Draw a small sphere at each point
            }
        }

    }
}