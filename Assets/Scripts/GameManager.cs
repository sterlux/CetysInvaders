using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public EnemySwarm enemySwarm;
    
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text stateText;

    private int _score;
    private bool _isGameOver;
    private PlayerControls _playerControls;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Gameplay.Restart.performed += OnRestart; 
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Gameplay.Restart.performed -= OnRestart; 
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _score = 0;
        _isGameOver = false;
        stateText.text = string.Empty;
        UpdateUI();

        if (player != null)
            player.OnPlayerHit += OnPlayerHitHandler;
        
        if (enemySwarm != null)
        {
            enemySwarm.OnEnemyKilled += AddScore;
            enemySwarm.OnSwarmReachedBottom += Lose;
        }
    }

    private void AddScore(int amount)
    {
        _score += amount;
        UpdateUI();

        if (enemySwarm._enemies.Count == 0)
            Win();
    }

    private void OnPlayerHitHandler()
    {
        UpdateUI();
        if (player.lives <= 0)
            Lose();
    }

    private void UpdateUI()
    {
        if(scoreText) 
            scoreText.text = $"Score: {_score}";
        if(livesText && player)
            livesText.text = $"Lives: {player.lives}";
    }
    
    private void Win()
    {
        _isGameOver = true;
        if(stateText)
            stateText.text = "You Win!\nPress 'R' to Restart";
        //Time.timeScale = 0f;
    }
    
    private void Lose()
    {
        _isGameOver = true;
        if(stateText)
            stateText.text = "Game Over\nPress 'R' to Restart";
        Time.timeScale = 0f;
    }
    private void OnRestart(InputAction.CallbackContext context)
    {
        if (!_isGameOver) return;
        
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
