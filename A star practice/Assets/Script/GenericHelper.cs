using UnityEngine;
using System.Collections;

public class GenericHelper
{

    public static LayerMask GetLayerMask(string ln)
    {
        return 1 << LayerMask.NameToLayer(ln);
    }

}
