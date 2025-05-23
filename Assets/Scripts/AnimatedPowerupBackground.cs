using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimatedPowerupBackground : MonoBehaviour
{

    public Image image;
    public BattleManager.PowerupTypes powerupType;
    private float originalImageTransparency;
    public float transitionDuration = 0.5f;

    public void SetOriginalTransparencyThenHide() {
        originalImageTransparency = image.color.a;
        //then, default to making it transparent
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

    }

    public void ShowIfMatchesType(BattleManager.PowerupTypes type) {
        if (type == powerupType)
            Show();
        else
            Hide();
    }

    private void Hide() {
        if (image.color.a != 0) {
            Color newcolor = new Color(image.color.r, image.color.g, image.color.b, 0);
            image.DOColor(newcolor, transitionDuration);
        }
    }

    private void Show() {
        if (image.color.a != originalImageTransparency) {
            Color newcolor = new Color(image.color.r, image.color.g, image.color.b, originalImageTransparency);
            image.DOColor(newcolor, transitionDuration);
        }
    }
}
