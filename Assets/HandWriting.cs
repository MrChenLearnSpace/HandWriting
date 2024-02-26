using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using System.Linq;
using System;
public class HandWriting : MonoBehaviour
{
    public int width = 28;
    public Texture2D raw;
    public Material targetMateral;
    public Color scolor;
    public NNModel m_Modle;

    public float temp;
    Model model;
    IWorker engine;
    float[] prediected;
    // Start is called before the first frame update
    void Start()
    {

        raw = new Texture2D(width, width);
        targetMateral.SetTexture("_MainTex", raw);

        temp = (float)width / Camera.main.pixelWidth;
        model = ModelLoader.Load(m_Modle);
        engine = WorkerFactory.CreateWorker(model, WorkerFactory.Device.CPU);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButton(0)) {
            //print(Input.mousePosition);
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //print(Camera.main.pixelWidth);
            pos *=temp;
            raw.SetPixel((int)pos.x, (int)pos.y,scolor);
            raw.Apply();
            targetMateral.SetTexture("_MainTex", raw);

        }

    }
    public void Reconfig() {
        Tensor input = new Tensor(raw, 1);

        Tensor output = engine.Execute(input).PeekOutput();
        input.Dispose();
        prediected = output.AsFloats().ToArray();
        string result = "";
        for (int i = 0; i < prediected.Length; i++) {
            result += prediected[i] + "  ";
        }
        print(" Max: " + Array.IndexOf(prediected, prediected.Max()) + "  " + "Predicted: " + result);
        raw = null;

        raw = new Texture2D(width, width);
        targetMateral.SetTexture("_MainTex", raw);
    }
}
