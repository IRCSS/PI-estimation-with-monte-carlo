using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MonteCarloSampler : MonoBehaviour {

    public  int           numberOfSamplesPerPass;
    public  Text          PIValueT;
    public  Text          NSamplesT;
    public  Text          ErrorT;

    private CommandBuffer cbDrawPoints;
    private Mesh          toDraw;
    private Vector4[]     points;
    private ComputeBuffer computerBufferpoints;
    private RenderTexture screenTexture;
    private Material      mt;
    private Camera        mainCm;
    private CommandBuffer cb;

    const   float PI         = 3.1415926535897f;
    private float EstmatedPI;
    private int   nSamples;
    private float error;
    private int   samplesIn = 0;
    private int   samplesOut = 0;
    // Use this for initialization
    void Start () {


        screenTexture        = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        computerBufferpoints = new ComputeBuffer(numberOfSamplesPerPass, sizeof(float) * 4);
        points               = new Vector4[numberOfSamplesPerPass];
        mt                   = new Material(Shader.Find("Unlit/points"));

       

    

        mainCm  = Camera.main;
        cb      = new CommandBuffer();
        cb.name = "sampling";

        cb.Blit(screenTexture, BuiltinRenderTextureType.CameraTarget);
        mainCm.AddCommandBuffer(CameraEvent.AfterEverything, cb); 
  

    }
	

     void Resample()
    {

        for (int i = 0; i < numberOfSamplesPerPass; i++)
        {
            //float width = Mathf.Floor(Mathf.Sqrt(numberOfSamplesPerPass));
            //float x = i % width;
            //float y = Mathf.Floor(i / width);
            //points[i] = new Vector4((x / width) * 2f - 1f, (y / width) * 2f - 1f, 0.5f, 0f);

            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            float length  =  Mathf.Sqrt(x * x + y * y);

            bool inCircle = (length <= 1.0f);

            if (inCircle)       samplesIn++;
            else                samplesOut++;


            points[i] = new Vector4(x, y, 0.5f, inCircle? 1: 0);

  

            
        }

    

        computerBufferpoints.SetData(points);
    }

	// Update is called once per frame
	void Update () {
        

        Resample();
        Graphics.SetRenderTarget(screenTexture);
        mt.SetPass(0);
        mt.SetFloat ("_frameCount", (float) Time.frameCount);
        mt.SetBuffer("_ParticleDataBuff", computerBufferpoints);
        Graphics.DrawProcedural(MeshTopology.Triangles, 3, numberOfSamplesPerPass);
        Graphics.SetRenderTarget(null);

        nSamples += numberOfSamplesPerPass;
        error     = PI - EstmatedPI;

        EstmatedPI = 4f* (float)samplesIn / ((float)samplesOut+ (float)samplesIn) ;
        PIValueT.text = "Est. PI value: " + EstmatedPI;
        ErrorT.text   = "Error: "             + error;
        NSamplesT.text= "N samples: "         + nSamples;
    }
}
