using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
public class HandWriting : MonoBehaviour
{
    public int width = 28;
    public Texture2D raw;
    public Material targetMateral;
    public Color scolor;
    public NNModel m_Modle;
    public Text uiDisplay;

    public float temp;
    public Vector4 maxDis;
    Model model;
    IWorker engine;
    float[] prediected;
    // Start is called before the first frame update
    void Start()
    {

       InitRecofig();

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
            maxDis.x = Math.Min(maxDis.x,(int) pos.x);
            maxDis.y = Math.Max(maxDis.y, (int)pos.x);
            maxDis.z = Math.Min(maxDis.z, (int)pos.y);
            maxDis.w = Math.Max(maxDis.w, (int)pos.y);
            raw.SetPixel((int)pos.x, (int)pos.y,scolor);
            raw.Apply();
            targetMateral.SetTexture("_MainTex", raw);

        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            Reconfig();
        }
    }
    public void Reconfig() {
        int x = (int)(((int)maxDis.x + (int)maxDis.y) * 0.5f);
        int y = (int)(((int)maxDis.z + (int)maxDis.w) * 0.5f);
        //将图片平移到中心
        float x1 = width * 0.5f - x;
        float y1 = width * 0.5f - y;
        if (x1<0&&y1<0) {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++) {
                    if (raw.GetPixel(i, j) == scolor) {
                        raw.SetPixel(i, j, Color.black);
                        raw.SetPixel((int)(i + x1), (int)(j + y1), scolor);
                    }
                }
           // print("111111111111");
        }
        else if (x1 >= 0 && y1 < 0) {
            for (int i = width - 1; i >= 0; i--)
                for (int j = 0; j < width; j++) {
                    if (raw.GetPixel(i, j) == scolor) {
                        raw.SetPixel(i, j, Color.black);
                        raw.SetPixel((int)(i + x1), (int)(j + y1), scolor);
                    }
                }
          // print("22222222222222");
        }
        else if (x1 < 0 && y1 >= 0) {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++) {
                    if (raw.GetPixel(i, j) == scolor) {
                        raw.SetPixel(i, j, Color.black);
                        raw.SetPixel((int)(i + x1), (int)(j + y1), scolor);
                    }
                }
           // print("3333333");
        }
        else {
            for (int i = width; i >= 0; i--)
                for (int j = width; j >= 0; j--) {
                    if (raw.GetPixel(i, j) == scolor) {
                        raw.SetPixel(i, j, Color.black);
                        raw.SetPixel((int)(i + x1), (int)(j + y1), scolor);
                    }
                }
           // print("444444444444");
        }
        //for (int i = 0; i < width; i++)
        //    for (int j = 0; j < width; j++) {
        //        if (raw.GetPixel(i, j) == scolor) {
        //            raw.SetPixel(i, j, Color.black);
        //            raw.SetPixel((int)(i + x1), (int)(j + y1), scolor);
        //        }
        //    }
        raw.Apply();
        Tensor input = new Tensor(raw, 1);

        Tensor output = engine.Execute(input).PeekOutput();
        input.Dispose();
        prediected = output.AsFloats().ToArray();
        string result = "";
        for (int i = 0; i < prediected.Length; i++) {
            result += prediected[i] + "  ";
        }
        uiDisplay.text = Array.IndexOf(prediected, prediected.Max()).ToString();
        print(" Max: " + Array.IndexOf(prediected, prediected.Max()) + "  " + "Predicted: " + result);
        InitRecofig();
    }
    void InitRecofig() {
        raw = null;

        raw = new Texture2D(width, width);
        for(int i = 0; i < width; i++)
            for (int j = 0; j < width; j++)
                raw.SetPixel(i, j, Color.black);
        raw.Apply();
        targetMateral.SetTexture("_MainTex", raw);
        maxDis = new Vector4(width, 0, width, 0);
    }
}
