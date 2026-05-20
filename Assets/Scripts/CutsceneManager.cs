using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public UnityEngine.UI.Image cutsceneImage;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("Content")]
    public Sprite forestIntroSprite;
    public Sprite cityIntroSprite;
    public Sprite labIntroSprite;
    public Sprite endingSprite;

    private List<string> dialogueLines = new List<string>();
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string fullText;

    void Start()
    {
        SetupCutscene();
        if (dialogueLines.Count > 0)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
    }

    void SetupCutscene()
    {
        if (GameFlowManager.Instance == null) return;

        dialogueLines.Clear();
        currentLineIndex = 0;

        switch (GameFlowManager.Instance.currentState)
        {
            case GameFlowManager.GameState.ForestIntro:
                dialogueLines.Add("The forest is deep and full of mysteries. I must find my way through.");
                if (forestIntroSprite != null) cutsceneImage.sprite = forestIntroSprite;
                break;
            case GameFlowManager.GameState.CityIntro:
                dialogueLines.Add("Finally, the city. But the corruption has reached here too.");
                if (cityIntroSprite != null) cutsceneImage.sprite = cityIntroSprite;
                break;
            case GameFlowManager.GameState.LabIntro:
                dialogueLines.Add("This laboratory holds the answers... and the source of the nightmare.");
                if (labIntroSprite != null) cutsceneImage.sprite = labIntroSprite;
                break;
            case GameFlowManager.GameState.Ending:
                dialogueLines.Add("The source has been neutralized. The world can finally heal.");
                dialogueLines.Add("Thank you for playing.");
                if (endingSprite != null) cutsceneImage.sprite = endingSprite;
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (isTyping)
        {
            // Skip typing and show full line
            StopAllCoroutines();
            dialogueText.text = dialogueLines[currentLineIndex];
            isTyping = false;
        }
        else
        {
            // Move to next line or end cutscene
            currentLineIndex++;
            if (currentLineIndex < dialogueLines.Count)
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
            }
            else
            {
                EndCutscene();
            }
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void EndCutscene()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.CompleteCurrentState();
        }
    }
}
