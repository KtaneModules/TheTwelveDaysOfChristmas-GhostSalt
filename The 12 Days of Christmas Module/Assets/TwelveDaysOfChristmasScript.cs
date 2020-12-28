using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class TwelveDaysOfChristmasScript : MonoBehaviour
{

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMNeedyModule Module;
    public KMAudio Audio;
    public KMSelectable[] Keys;
    public TextMesh Text;

    private int SelectedLyric;
    private string[] Lyrics = { "Partridges in pear trees", "Turtle doves", "French hens", "Calling birds", "Gold rings", "Geese a-laying", "Swans a-swimming", "Maids a-milking", "Ladies dancing", "Lords a-leaping", "Pipers piping", "Drummers drumming" };
    private bool Deactivated = true;

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        for (int i = 0; i < 12; i++)
        {
            int x = i;
            Keys[x].OnInteract += delegate { StartCoroutine(ButtonPress(x)); return false; };
        }
        Module.OnNeedyActivation += delegate { Activate(); };
        Module.OnTimerExpired += delegate { Deactivate(); };
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Activate()
    {
        SelectedLyric = Rnd.Range(0, 12);
        Text.text = Lyrics[SelectedLyric];
        Deactivated = false;
    }

    void Deactivate()
    {
        Text.text = "";
        Module.HandleStrike();
        Deactivated = true;
    }

    private IEnumerator ButtonPress(int pos)
    {
        Audio.PlaySoundAtTransform("press", Keys[pos].transform);
        Keys[pos].AddInteractionPunch();
        for (int i = 0; i < 3; i++)
        {
            Keys[pos].transform.localPosition -= new Vector3(0, 0.0025f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        if (!Deactivated)
        {
            if (pos == SelectedLyric)
            {
                Module.HandlePass();
            }
            else
            {
                Module.HandleStrike();
                Module.HandlePass();
            }
            Text.text = "";
            Deactivated = true;
        }
        for (int i = 0; i < 3; i++)
        {
            Keys[pos].transform.localPosition += new Vector3(0, 0.0025f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} 12' to press button 12.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        string[] ValidCommands = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
        if (!ValidCommands.Contains(command))
        {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
        yield return null;
        Keys[int.Parse(command) - 1].OnInteract();
    }
}
