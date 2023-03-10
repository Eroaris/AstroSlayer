using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Goal : MonoBehaviour
{
    public TextMeshProUGUI questtext;
    public CanvasGroup questpanel;
    public GameStateManager GSM;
    public Player player;
    private AudioSource audioSource;
    private SpriteRenderer _spriteRenderer;
    Color lerpedColor;

    public AudioClip reactorComplete;
    public AudioClip reactorCharging;
    float goalTime = 60f; 
    float timePassed;
    private bool playerInside;
    private static int activatedGoals;
    private float percentComplete;
    private bool isCompleted;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerInside = false;
    }
    private void Start()
    {
        activatedGoals = 0;
        isCompleted = false;
        StartCoroutine(Questtext(activatedGoals, 3, 1));
    }
    private IEnumerator Questtext(int goalsActivated,float waitDuration, float fadeDuration)
    {
        questpanel.alpha = 1;
        
        questtext.text = "Find and activate Reactor Batteries to escape. \n \n  Batteries activated:   " + goalsActivated +"/4";
        
        yield return new WaitForSeconds(waitDuration);

        for (float t = 0; t < fadeDuration; t+= Time.deltaTime)
        {
            questpanel.alpha = Mathf.Lerp(1f,0f,t/ fadeDuration);
            
            yield return null;
        }
        questpanel.alpha = 0;
    }
    public void Update()
    {
        if (playerInside)
        {
            timePassed += Time.deltaTime;
            percentComplete = timePassed / goalTime;
            percentComplete = Mathf.Clamp01(percentComplete);
            lerpedColor = Color.Lerp(new Color(0.01960784f,0.8784314f,0.4941177f,0), new Color(0.01960784f,0.8784314f,0.4941177f,1), percentComplete);
            _spriteRenderer.color = lerpedColor;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = false;
            audioSource.Stop();
            if (percentComplete >= 1 && GSM.GetCurrentState() == GameStateManager.GameState.Playing && isCompleted == false)
            {
                activatedGoals++;
                
                player._currentHealth++;
                player.hearts[player._currentHealth].gameObject.SetActive(true);
                player._currentHealth++;
                player.hearts[player._currentHealth].gameObject.SetActive(true);
                
                isCompleted = true;
                
                audioSource.PlayOneShot(reactorComplete);
                
                StartCoroutine(Questtext(activatedGoals, 3, 1));
                
                if (activatedGoals == 4 && GSM.GetCurrentState() != GameStateManager.GameState.Win)
                {
                    GSM.SetCurrentState(GameStateManager.GameState.Win);
                }
            }
        }
    }
}
