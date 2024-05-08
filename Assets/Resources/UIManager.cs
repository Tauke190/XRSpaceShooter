using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.TypewriterEffects;


[System.Serializable]
public struct Dialogue
{
    [TextArea(minLines: 5, maxLines: 20)]
    public string text;
    public int duration;
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro _score;
    [SerializeField] public TextMeshPro _wavetext;
    [SerializeField] private TextMeshPro _enemyremainingtext;
    [SerializeField] private TextMeshPro _ammoremainingtext;
    [SerializeField] private Slider _healtslider;
    
    private Transform player;


    public Dialogue[] dialogue;
  
    private int dialogueindex;

    public Typewriter typewriterAnimator;

    private void Start()
    {
        player = GameManager.instance.XRPlayer.transform; 
    }

    public void StartDialogue()
    {
        StartCoroutine(TextDialogue());
    }

    // -- Class
    private IEnumerator TextDialogue()
    {
        yield return new WaitForSeconds(2);
      
        while (dialogueindex < dialogue.Length)
        {
            typewriterAnimator.Animate(dialogue[dialogueindex].text);
            yield return new WaitForSeconds(dialogue[dialogueindex].duration);
            if (dialogueindex == 5)
            {
                GameManager.instance.SpawnTutorialOrb();
            }
            dialogueindex++;
       
        }
        GameManager.instance.StartGame();
    }

    private void Update()
    {
        _score.text = "Score: " + GameManager.instance.XRPlayer._score;
        _enemyremainingtext.text = "Enemy Remaining : " + EnemyManager.instance.remainingEnemies + " /"+EnemyManager.instance.waves[EnemyManager.instance.waveIndex].count;
        _ammoremainingtext.text = "Ammo Remaining : " + GameManager.instance.XRPlayer.ammo;
        _healtslider.value = GameManager.instance.XRPlayer.health / 100f;
        SetWaveTextPosition();
    }

    private void SetWaveTextPosition()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 newposition = player.position + dir * 1f + new Vector3(0,0.25f,0);
        _wavetext.transform.position = newposition;
        _wavetext.transform.rotation = Quaternion.LookRotation(dir);
    }
}
