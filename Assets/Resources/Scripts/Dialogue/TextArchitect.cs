using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    public string currentText => tmpro.text;
    public string targetText { get; private set; } = "";
    public string preText { get; private set; } = "";

    public string fullTargetText => preText + targetText;

    public enum BuildMethod
    {
        Instant,
        Typewriter,
        Fade
    }

    public BuildMethod buildMethod = BuildMethod.Typewriter;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());

        return buildProcess;
    }

    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());

        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public void Stop()
    {
        if (!isBuilding)
        {
            return;
        }

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.Typewriter:
                yield return Build_Typewriter();
                break;
        }

        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
    }

    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.Typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }

        Stop();
        OnComplete();
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.Instant:
                Prepare_Instant();
                break;
            case BuildMethod.Typewriter:
                Prepare_Typewriter();
                break;
        }
    }

    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += charactersPerCycle;
            PlayDialogueSound(tmpro.maxVisibleCharacters);
            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount)
    {
        if (currentDisplayedCharacterCount % 4 == 0)
        {
            FMOD.Studio.EventInstance sfxTyping;
            sfxTyping = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/SFX_UI Dialogue");
            sfxTyping.start();
        }
    }
}
