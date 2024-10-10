using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCheckGame : MonoBehaviour
{
    [Header("Game Objects")]
    public RectTransform checkbarBackground;
    public Transform movingLine;
    public Transform successZone;
    public Transform hand;
    public Transform player;

    [Header("UI Elements")]
    public Text scoreText;
    public Text speedText;
    public Text comboText;

    [Header("Game Settings")]
    public float lineSpeed = 5f;
    public float successZoneWidth = 0.5f;
    public float handMoveSpeed = 2f;
    public float pointsPerSuccess = 10f;
    public float evasionBonus = 5f;

    [Header("Hand Timing")]
    public float minHandWaitTime = 5f;
    public float maxHandWaitTime = 15f;

    private float currentDirection = 1f;
    private int playerPosition = 1;
    private Vector3[] playerPositions;
    private bool isHandMoving = false;
    private Vector3 handStartPosition;
    private float handProgress;
    private int targetPositionIndex = 1;
    private int comboCount = 0;
    private float lineMaxX;
    private Vector3 checkbarCenter;

    private bool alert;
    public float riseSpeed;
    private void Start()
    {
        InitializeCheckbar();
        InitializePlayerPositions();
        InitializeUI();
        StartCoroutine(HandRise());
    }

    private void InitializeCheckbar()
    {
        Vector3[] corners = new Vector3[4];
        checkbarBackground.GetWorldCorners(corners);
        float checkbarWidth = corners[2].x - corners[0].x;

        lineMaxX = checkbarWidth / 2;
        checkbarCenter = checkbarBackground.position;

        movingLine.position = checkbarCenter;
        MoveSuccessZoneToRandomPosition();
    }

    private void MoveSuccessZoneToRandomPosition()
    {
        float randomX = Random.Range(-lineMaxX + successZoneWidth / 2, lineMaxX - successZoneWidth / 2);
        Vector3 newPosition = checkbarCenter + new Vector3(randomX, 0, 0);
        successZone.position = new Vector3(newPosition.x, checkbarCenter.y, successZone.position.z);
    }

    private void InitializePlayerPositions()
    {
        playerPositions = new Vector3[3];
        playerPositions[0] = new Vector3(-2, player.position.y, 0);
        playerPositions[1] = new Vector3(0, player.position.y, 0);
        playerPositions[2] = new Vector3(2, player.position.y, 0);

        player.position = playerPositions[playerPosition];
        handStartPosition = new Vector3(0, 5f, 0);
        hand.position = handStartPosition;
    }

    private void InitializeUI()
    {
        UpdateScoreText();
        UpdateComboText();
    }

    private void Update()
    {
        HandlePlayerMovement();
        HandleLineMovement();
        HandleHand();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    private void HandlePlayerMovement()
    {
        int previousPosition = playerPosition;

        if (Input.GetKeyDown(KeyCode.LeftArrow) && playerPosition > 0)
        {
            playerPosition--;
            player.position = playerPositions[playerPosition];
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && playerPosition < 2)
        {
            playerPosition++;
            player.position = playerPositions[playerPosition];
        }

        if (isHandMoving && previousPosition != playerPosition)
        {
            CheckEvasion();
        }
    }

    private void HandleLineMovement()
    {
        float speedMultiplier = 1f + (GameManager.Instance.GetUpgradeValue("Speed") / 10f);
        float movement = lineSpeed * speedMultiplier * currentDirection * Time.deltaTime;

        Vector3 newPosition = movingLine.position + new Vector3(movement, 0, 0);
        Vector3 relativePosition = newPosition - checkbarCenter;

        if (Mathf.Abs(relativePosition.x) > lineMaxX)
        {
            currentDirection *= -1;
            relativePosition.x = Mathf.Sign(relativePosition.x) * lineMaxX;
            newPosition = checkbarCenter + relativePosition;
        }

        movingLine.position = newPosition;
    }

    private void HandleHand()
    {
        if (isHandMoving)
        {
            handProgress += handMoveSpeed * Time.deltaTime;
            Vector3 targetPosition = playerPositions[targetPositionIndex];
            hand.position = Vector3.Lerp(handStartPosition, targetPosition, handProgress);

            if (handProgress >= 1f)
            {
                if (targetPositionIndex == playerPosition)
                {
                    GameManager.Instance.AddCurrency(-5);
                    comboCount = 0;
                    UpdateScoreText();
                    UpdateComboText();
                }
                ResetHand();
                StartCoroutine(HandRoutine());
            }
        }
    }

    private void CheckSuccess()
    {
        float relativeLineX = movingLine.position.x - checkbarCenter.x;
        float relativeSuccessZoneX = successZone.position.x - checkbarCenter.x;
        float distance = Mathf.Abs(relativeLineX - relativeSuccessZoneX);

        if (distance < successZoneWidth / 2)
        {
            float basePoints = pointsPerSuccess;
            float speedMultiplier = 1f + (GameManager.Instance.GetUpgradeValue("Speed") / 10f);
            int earnedPoints = Mathf.RoundToInt(basePoints * speedMultiplier);

            GameManager.Instance.AddCurrency(earnedPoints);
            UpdateScoreText();
        }
        else
        {
            GameManager.Instance.AddCurrency(-2);
            comboCount = 0;
            UpdateScoreText();
            UpdateComboText();
        }

        MoveSuccessZoneToRandomPosition();
    }

    private void CheckEvasion()
    {
        if (handProgress > 0.5f)
        {
            comboCount++;
            float bonusPoints = evasionBonus * comboCount;
            GameManager.Instance.AddCurrency(Mathf.RoundToInt(bonusPoints));
            UpdateScoreText();
            UpdateComboText();
        }
    }

    private IEnumerator HandRise()
    {
        if (!alert)
        {
            hand.position += new Vector3(0,riseSpeed * Time.deltaTime,0);
            if (hand.position == handStartPosition)
            {
                StartCoroutine(HandRoutine());
                yield return null;
            }
        }
        if (alert)
        {
            StartCoroutine(HandRoutine());
            yield return null;
        }
       
    }
    private IEnumerator HandRoutine()
    {
        while (true)
        {
            isHandMoving = false;
            hand.position = handStartPosition;
            handProgress = 0f;

            float waitTime = Random.Range(minHandWaitTime, maxHandWaitTime);
            yield return new WaitForSeconds(waitTime);

            targetPositionIndex = playerPosition;
            isHandMoving = true;

            while (isHandMoving)
            {
                yield return null;
            }
        }
    }

    private void ResetHand()
    {
        isHandMoving = false;
        hand.position = handStartPosition;
        handProgress = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.currency}";
        }
    }

    private void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = comboCount > 0 ? $"Combo: x{comboCount}" : "";
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        if (checkbarBackground != null)
        {
            Gizmos.color = Color.yellow;
            Vector3[] corners = new Vector3[4];
            checkbarBackground.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(checkbarCenter, 0.1f);
                Gizmos.DrawLine(checkbarCenter + Vector3.left * lineMaxX,
                               checkbarCenter + Vector3.right * lineMaxX);
            }
        }
    }
}