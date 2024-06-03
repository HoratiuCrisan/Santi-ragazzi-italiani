using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackgroundImage : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite background;

    public Sprite newImage;

    public void ChangeImage()
    {
        if (background != null && newImage != null)
        {
            background = newImage;
        }
    }
}
