using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderTexture : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexturer;
    // Start is called before the first frame update
    void Start()
    {
        renderTexturer = new RenderTexture(256, 256, 32);
        renderTexturer.enableRandomWrite = true;
        renderTexturer.Create();

        computeShader.SetTexture(0, "Result", renderTexturer);
        computeShader.SetFloat("resolution", renderTexturer.width);
        computeShader.Dispatch(0, renderTexturer.width / 8, renderTexturer.width/8, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
