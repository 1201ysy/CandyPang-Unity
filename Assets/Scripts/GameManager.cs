using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{


    public static GameManager instance = null;

    public int currentLvl = 1;
    public int remainingMove = 0;

    public int[] candyGoal;

    public bool canMoveCandy = true;

    public bool isGameOver = false;


    // Game Start Panel
    [SerializeField] private GameObject gameStartPanel;
    [SerializeField] private Image stageNumImage;
    [SerializeField] private Image candyImage1;
    [SerializeField] private Image candyImage2;
    [SerializeField] private TextMeshProUGUI goalCandyText1;
    [SerializeField] private TextMeshProUGUI goalCandyText2;
    [SerializeField] private TextMeshProUGUI moveText;

    [SerializeField] private Sprite[] candies;
    [SerializeField] private Sprite[] stages;



    // Game Panel
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Image gameCandyImage1;
    [SerializeField] private Image gameCandyImage2;
    [SerializeField] private TextMeshProUGUI gameGoalCandyText1;
    [SerializeField] private TextMeshProUGUI gameGoalCandyText2;
    [SerializeField] private TextMeshProUGUI gameMoveText;

    private int goalCandyIndex1;
    private int goalCandyIndex2;


    // Game Over Panel

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button okButton;

    [SerializeField] private Sprite[] gameOverButtons;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    void Start()
    {
        canMoveCandy = false;
        SetLevel();
        SetCandyGoal();
        SetMaxMove();

    }

    public int GetBoardWitdh()
    {
        if (currentLvl == 1)
        {
            return 7;
        }
        else if (currentLvl == 2)
        {
            return 8;
        }

        return 9;
    }

    public int GetBoardHeight()
    {
        if (currentLvl == 1)
        {
            return 7;
        }
        else if (currentLvl == 2)
        {
            return 8;
        }
        return 9;
    }

    private void SetLevel()
    {
        currentLvl = PlayerPrefs.GetInt("CurrentLevel", 1);
        int index = currentLvl - 1;
        if (index >= stages.Length - 1)
        {
            index = stages.Length - 1;
        }
        stageNumImage.sprite = stages[index];

    }

    private void SetMaxMove()
    {
        if (remainingMove == 1)
        {
            remainingMove = 20;
        }
        else if (currentLvl == 2)
        {
            remainingMove = 15;
        }
        else
        {
            remainingMove = 10;
        }

        moveText.text = "MOVES : " + remainingMove.ToString();
        gameMoveText.text = "MOVES : " + remainingMove.ToString();
    }

    public void DecreaseMove()
    {
        remainingMove -= 1;
        gameMoveText.text = "MOVES : " + remainingMove.ToString();
    }

    private void SetCandyGoal()
    {
        if (currentLvl == 1)
        {
            candyGoal = new int[] { 10, 10, 0, 0, 0 };
        }
        else if (currentLvl == 2)
        {
            candyGoal = new int[] { 0, 0, 30, 30, 0 };
        }
        else
        {
            candyGoal = new int[] { 60, 0, 0, 0, 60 };
        }

        // Set Image/text
        int candy = 1;
        for (int i = 0; i < candyGoal.Length; i++)
        {
            if (candyGoal[i] > 0)
            {
                if (candy == 1)
                {
                    goalCandyIndex1 = i;
                    candyImage1.sprite = candies[i];
                    goalCandyText1.text = candyGoal[i].ToString();

                    gameCandyImage1.sprite = candies[i];
                    gameGoalCandyText1.text = candyGoal[i].ToString();
                    candy++;
                }
                else if (candy == 2)
                {
                    goalCandyIndex2 = i;
                    candyImage2.sprite = candies[i];
                    goalCandyText2.text = candyGoal[i].ToString();

                    gameCandyImage2.sprite = candies[i];
                    gameGoalCandyText2.text = candyGoal[i].ToString();
                    break;
                }

            }
        }
    }

    public void DecreaseBlue()
    {
        DecreaseCandyAt(0);
    }
    public void DecreaseGreen()
    {
        DecreaseCandyAt(1);
    }
    public void DecreasePurple()
    {
        DecreaseCandyAt(2);
    }
    public void DecreaseRed()
    {
        DecreaseCandyAt(3);
    }
    public void DecreaseYellow()
    {
        DecreaseCandyAt(4);
    }

    public void DecreaseCandyAt(int index)
    {
        candyGoal[index] -= 1;
        if (candyGoal[index] < 0)
        {
            candyGoal[index] = 0;
        }

        if (index == goalCandyIndex1)
        {
            gameGoalCandyText1.text = candyGoal[index].ToString();
        }
        else if (index == goalCandyIndex2)
        {
            gameGoalCandyText2.text = candyGoal[index].ToString();
        }

    }


    public void CheckGameOver()
    {
        isGameOver = true;
        foreach (int goal in candyGoal)
        {
            if (goal > 0)
            {
                isGameOver = false;
                break;
            }
        }

        if (isGameOver)
        {

            canMoveCandy = false;
            resultText.text = "MISSION COMPLETE";
            okButton.image.sprite = gameOverButtons[0];
            IncreaseLevel();
            gamePanel.SetActive(false);
            gameOverPanel.SetActive(true);

        }
        else if (remainingMove == 0)
        {
            isGameOver = true;
            canMoveCandy = false;
            Debug.Log("Failed Level");
            resultText.text = "MISSION FAILED";
            okButton.image.sprite = gameOverButtons[1];
            gamePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    private void IncreaseLevel()
    {
        currentLvl++;
        PlayerPrefs.SetInt("CurrentLevel", currentLvl);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void StartGame()
    {
        canMoveCandy = true;
        gameStartPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}

