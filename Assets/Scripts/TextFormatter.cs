using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public static class TextFormatter{

    private static readonly string damageKeywordColor = "9C2931";

    private static readonly string healKeywordColor = "92E8C0";
    private static readonly string waterKeywordColor = "0A95D0";
    //private static readonly string earthKeywordColor = "7D5743";
    //private static readonly string fireKeywordColor = "F87820";
    //private static readonly string lightningKeywordColor = "FFCF00";
    //private static readonly string darkKeywordColor = "534F8F";
    //private static readonly string swordKeywordColor = "981F88";

    public static List<string> FormatStringList(List<string> input){
        List<string> output = new();
        foreach (string s in input){
            output.Add(FormatString(s));
            //string copy = s;
            //FormatString(copy);
            //output.Add(copy);
        }
        return output;
    }

    public static string FormatString(string input){

        string output = input;

        //damage keywords
        HighlightKeyword(ref output, "debuff", damageKeywordColor);
        HighlightKeyword(ref output, "+1 damage", damageKeywordColor);
        HighlightKeyword(ref output, "+2 damage", damageKeywordColor);
        HighlightKeyword(ref output, "+3 damage", damageKeywordColor);
        HighlightKeyword(ref output, "+4 damage", damageKeywordColor);

        //water keywords
        HighlightKeyword(ref output, "flooded", waterKeywordColor);
        HighlightKeyword(ref output, "power of water", waterKeywordColor);
        
        //healing keywords
        HighlightKeyword(ref output, "healing magic", healKeywordColor);
        HighlightKeyword(ref output, "power of healing", healKeywordColor);

        //double spaces for readability
        output = output.Replace(" ", "  ");

        return output;
    }

    private static void HighlightKeyword(ref string original, string keyword, string color){
        string replacement = "<color=#" + color + ">" + keyword + "</color>";
        original = original.Replace(keyword, replacement);
    }

}


