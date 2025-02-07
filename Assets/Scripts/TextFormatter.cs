using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public static class TextFormatter{

    private static readonly string damageKeywordColor = "9C2931";

    public static List<string> FormatStringList(List<string> input){
        List<string> output = new();
        foreach (string s in input)
            output.Add(FormatString(s));
        return output;
    }

    public static string FormatString(string input){

        string output = input;

        output = output.Replace("<damage>", "<color=#" + damageKeywordColor + ">");
        output = output.Replace("<water>", "<color=#" + StaticVariables.waterPowerupColor.ToHexString() + ">");
        output = output.Replace("<healing>", "<color=#" + StaticVariables.healingPowerupColor.ToHexString() + ">");
        output = output.Replace("<earth>", "<color=#" + StaticVariables.earthPowerupColor.ToHexString() + ">");
        output = output.Replace("<fire>", "<color=#" + StaticVariables.firePowerupColor.ToHexString() + ">");
        output = output.Replace("<lightning>", "<color=#" + StaticVariables.lightningPowerupColor.ToHexString() + ">");
        output = output.Replace("<dark>", "<color=#" + StaticVariables.darknessPowerupColor.ToHexString() + ">");
        output = output.Replace("<sword>", "<color=#" + StaticVariables.swordPowerupColor.ToHexString() + ">");
        output = output.Replace("<>", "</color>");
        
        output = output.Replace("<playername>", StaticVariables.playerName);

        //double spaces for readability
        output = output.Replace(" ", "  ");

        return output;
    }

}


