using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public GameObject dialogPanel;             
    public TextMeshProUGUI dialogText;

    [TextArea(3, 10)] public List<string> dialogLines = new List<string>();
    public float lineDisplayDuration = 3f;
    public float fadeDuration = 0.5f;

    private int currentLineIndex = 0;
    private Coroutine dialogCoroutine;
    private bool isFading = false;


    private List<Graphic> panelGraphics = new List<Graphic>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (dialogPanel != null)
        {
            panelGraphics.AddRange(dialogPanel.GetComponentsInChildren<Graphic>(true));
        }

        dialogPanel.SetActive(false);     

    }

    void Update()
    {
        if (dialogPanel.activeSelf &&
        PlayerController.instance.controller.Movement.SkipDialog.triggered &&
        !isFading)
        {
            SkipLine();
        }
    }

    public void StartDialog(List<string> lines, float durationPerLine = 3f)
    {
        dialogLines = lines;
        lineDisplayDuration = durationPerLine;
        currentLineIndex = 0;

        if (dialogCoroutine != null)
            StopCoroutine(dialogCoroutine);

        dialogCoroutine = StartCoroutine(RunDialog());
    }

    private IEnumerator RunDialog()
    {
        dialogPanel.SetActive(true);
        yield return StartCoroutine(FadeGraphics(0f, 1f)); // fade in

        while (currentLineIndex < dialogLines.Count)
        {
            dialogText.text = dialogLines[currentLineIndex];
            yield return new WaitForSeconds(lineDisplayDuration);
            currentLineIndex++;
        }

        yield return StartCoroutine(FadeGraphics(1f, 0f)); // fade out
        dialogPanel.SetActive(false);
    }

    private void SkipLine()
    {
        if (isFading) return;

        currentLineIndex++;
        if (currentLineIndex < dialogLines.Count)
        {
            dialogText.text = dialogLines[currentLineIndex];

            if (dialogCoroutine != null)
                StopCoroutine(dialogCoroutine);

            dialogCoroutine = StartCoroutine(ResumeDialog());
        }
        else
        {
            if (dialogCoroutine != null)
                StopCoroutine(dialogCoroutine);

            StartCoroutine(FadeGraphics(1f, 0f));
            dialogPanel.SetActive(false);
        }
    }

    private IEnumerator ResumeDialog()
    {
        yield return new WaitForSeconds(lineDisplayDuration);
        currentLineIndex++;

        if (currentLineIndex < dialogLines.Count)
        {
            dialogText.text = dialogLines[currentLineIndex];
            dialogCoroutine = StartCoroutine(ResumeDialog());
        }
        else
        {
            StartCoroutine(FadeGraphics(1f, 0f));
            dialogPanel.SetActive(false);
        }
    }

    private IEnumerator FadeGraphics(float from, float to)
    {
        isFading = true;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Lerp(from, to, time / fadeDuration);

            foreach (var g in panelGraphics)
            {
                if (g != null)
                {
                    Color c = g.color;
                    c.a = t;
                    g.color = c;
                }
            }

            yield return null;
        }

        isFading = false;
    }
}
