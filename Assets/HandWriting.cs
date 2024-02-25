using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWriting : MonoBehaviour
{
    public int width = 64;
    public Texture2D raw;
    public Material targetMateral;
    public Color scolor;

    float temp;
    // Start is called before the first frame update
    void Start()
    {
        raw = new Texture2D(width, width);
        targetMateral.SetTexture("_MainTex", raw);

        temp = 64.0f / Camera.main.pixelWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)) {
            //print(Input.mousePosition);
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //print(Camera.main.pixelWidth);
            pos *=temp;
            print(pos);
            raw.SetPixel((int)pos.x, (int)pos.y,scolor);
            raw.Apply();
            targetMateral.SetTexture("_MainTex", raw);

        }
    }
}
