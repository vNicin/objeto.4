using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randColor : MonoBehaviour
{
    struct Cube
    {
        public float velocidadeInicial;
        public float deltaDistance;
        public Color color;
        
    }

    public ComputeShader computeShader;
    public int iteractions = 50;
    public int count = 100;
    GameObject[] gameObjects;
    Cube[] data;
    public GameObject modelPref;


    public float[] posY;
    float[] velInicial;
    float g = 9.8f;
    public GameObject ground;
    public int massaMinima;
    public int massaMaxima;

    public int forcaMinima;
    public int forcaMaxima;
    public bool falso;

    public bool fallCPU = false;
    public bool fallGPU = false;
    public bool colorChanged = false;

    float tempoI;
    float tempoF;

    float tempoo;

    // Start is called before the first frame update
    void Start()
    {
        
        data = new Cube[count * count];
        posY = new float[count * count];
        velInicial = new float[count * count];

        for (int i = 0; i < data.Length; i++)
        {
            posY[i] = gameObject.transform.position.y;
            velInicial[i] = 0;
        }
        falso = true;

    }

    // Update is called once per frame
    void Update()
    {

        if (fallCPU)
        {
            CairPelaCPU();
        }
        if (fallGPU)
        {
            CairPelaGPU();
            tempoo += Time.deltaTime;
            print(tempoo);
        }
        ChecarChao();
       
    }

    private void ChecarChao()
    {

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].transform.position.y <= ground.transform.position.y)
            {
                if (fallCPU)
                {
                    fallCPU = false;
                    changeColor(gameObjects[i]);
                }


                if (fallGPU)
                {
                  
                    fallGPU = false;
                    changeColor(gameObjects[i]);
                  
                }



                gameObjects[i].transform.position = new Vector3(gameObjects[i].transform.position.x, ground.transform.position.y+0.25f, gameObjects[i].transform.position.z);
                velInicial[i] =0;
                //  velInicial[i] = Random.Range(1, 5);
            }
           

        }

      

    }

    void OnGUI()
    {
      

            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {
                createCube();
            }
        

        if(data!= null)
        {
            if(GUI.Button(new Rect(110,0,100,50), "gravity CPU"))
            {
                fallCPU = true;
                StartTime();

               

              
               

                
            }
        }
       

        if(data != null)
        {
            if (GUI.Button(new Rect(220,0,100,50), "gravity GPU"))
            {

                fallGPU = true;
                StartTime();


                /*int totalSize = 4 * sizeof(float) + 3 * sizeof(float);
                ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);//utilizando GPU
                computeBuffer.SetData(data); //copiar dados da cpu pra gpu
                computeShader.SetBuffer(0, "cubes", computeBuffer);
                computeShader.SetInt("iteraction", iteractions);
                computeShader.Dispatch(0, data.Length / 10, 1, 1);
               
                computeBuffer.GetData(data); //passando os dados de volta para a cpu
                for (int i = 0; i < gameObjects.Length; i++)
                {
                  
                    gameObjects[i].GetComponent<Rigidbody>().isKinematic = false;
                    int random = Random.Range(massaMinima, massaMaxima);
                    int forcaRandom = Random.Range(forcaMinima, forcaMaxima);
                    //gameObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, random));
                    gameObjects[i].GetComponent<Rigidbody>().drag = forcaRandom;
                    gameObjects[i].GetComponent<Rigidbody>().mass = random;
                    //gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", data[i].color);


                }
             

                computeBuffer.Dispose();//libera a memoria q havia sido alocada
               // int totalSize = 4 * sizeof(float) + 3 * sizeof(float);
               // ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);//utilizando GPU
               // computeBuffer.SetData(data); //copiar dados da cpu pra gpu
               // computeShader.SetBuffer(0, "cubes", computeBuffer);
               // computeShader.SetInt("iteraction", iteractions);
               // computeShader.Dispatch(0, data.Length / 10, 1, 1);

               // computeBuffer.GetData(data); //passando os dados de volta para a cpu
              //  for (int i = 0; i < gameObjects.Length; i++)
              //  {
                 //   gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color",data[i].color);
               // }

               // computeBuffer.Dispose();//libera a memoria q havia sido alocada*/

            }
        }
    }

    private void CairPelaGPU()
    {
        int totalSize = sizeof(float) * 4 + 3 * sizeof(float) -4;
        ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);//utilizando GPU
         computeBuffer.SetData(data);
     
     

   
        for (int i = 0; i < gameObjects.Length; i++)
        {
            computeShader.SetBuffer(0, "cubeFall", computeBuffer);
            computeShader.SetFloat("posI", posY[i]);
            computeShader.SetFloat("velI", velInicial[i]);
            computeShader.SetFloat("deltaTime", Time.deltaTime);
            computeShader.SetInt("nCubes", data.Length);
            computeShader.Dispatch(0, data.Length / 10, 1, 1);
        }
        computeBuffer.GetData(data);

        for (int i = 0; i < gameObjects.Length; i++)
        {
            posY[i] = data[i].deltaDistance;
            velInicial[i] = data[i].velocidadeInicial;

            gameObjects[i].transform.position = new Vector3(gameObjects[i].transform.position.x, posY[i], gameObjects[i].transform.position.z);
        }
        ChangeColorGPU();
        computeBuffer.Dispose();

    }
    private void CairPelaCPU()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            float[] vF = new float[gameObjects.Length];
            float[] dP = new float[gameObjects.Length];
            float[] pF = new float[gameObjects.Length];

            vF[i] = velInicial[i] + g * Time.deltaTime;
            dP[i] = ((velInicial[i] + vF[i]) * Time.deltaTime) / 2;
            pF[i] = posY[i] - dP[i];

            gameObjects[i].transform.position = new Vector3(gameObjects[i].transform.position.x, pF[i], gameObjects[i].transform.position.z);

            velInicial[i] = vF[i];
            posY[i] = gameObjects[i].transform.position.y;
        }


    }
    private void createCube()//ainda em cpu
    {
        data = new Cube[count * count];
        gameObjects = new GameObject[count * count];

        for (int i = 0; i < count; i++)
        {
            float offsetX = (-count / 2 + i);
            for(int j =0; j<count; j++)
            {
                float offsetY = (-count / 2 + j);
                Color _color = Random.ColorHSV();
                GameObject go = GameObject.Instantiate(modelPref, new Vector3(offsetX * 0.7f, 0, offsetY * 0.7f), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _color);
      

                gameObjects[i * count + j] = go;
                data[i * count + j] = new Cube();
            
            }
        }
    }

    public void ChangeColorGPU()
    {
        int totalSize = sizeof(float) * 4 + 3 * sizeof(float);

        ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);
        computeBuffer.SetData(data);

        computeShader.SetBuffer(0, "cubes", computeBuffer);
        computeShader.SetInt("interactions", iteractions);
        computeShader.SetInt("nCubes", data.Length);

        computeShader.Dispatch(0, Mathf.CeilToInt(data.Length / 10), 1, 1);

        computeBuffer.GetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", data[i].color);
        }
        EndTime();
        computeBuffer.Dispose();

     
    }



    public void changeColor(GameObject ga)
    {
        
             
        ga.GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Random.ColorHSV());
        EndTime();
       
    }

    /*private void applyGravity()
    {

        
        if (id == 1)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {

                int random = Random.Range(massaMinima, massaMaxima);
                int forcaRandom = Random.Range(forcaMinima, forcaMaxima);
                gameObjects[i].GetComponent<Rigidbody>().mass = random;
                gameObjects[i].GetComponent<Rigidbody>().drag = forcaRandom;
            }
            id++;
        }


    }*/


    public void StartTime()
    {
        tempoI = Time.realtimeSinceStartup;
        print(tempoI);
    }

    public void EndTime()
    {
        tempoF = Time.realtimeSinceStartup;
        print("Tempo final = " + tempoF + ". Tempo total percorrido = " + (tempoF - tempoI));
    }
}
