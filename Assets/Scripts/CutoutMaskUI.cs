//made using code from https://youtu.be/XJJl19N2KFM?si=A925RH7E8kHgIr9h

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutoutMaskUI : Image {

    public override Material materialForRendering {
        get{
            Material material = new Material(base.materialForRendering);
            material.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            return material;
        }
    }
}
