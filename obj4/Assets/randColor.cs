using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randColor : MonoBehaviour
{
    struct Cube
    {
        public Vector3 position;
        public Color color;
    }

    public ComputeShader computeShader;
    public int iteractions = 50;
    public int count = 100;
    GameObject[] gameObjects;
    Cube[] data;
    public GameObject modelPref;

    public int massaMinima;
    public int massaMaxima;

    public int forcaMinima;
    public int forcaMaxima;
    int id = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (data == null)
        {


            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {
                createCube();
            }
        }

        if(data!= null)
        {
            if(GUI.Button(new Rect(110,0,100,50), "Random CPU"))
            {
                for( int k = 0; k<iteractions; k++)
                {

                
                    for ( int i = 0; i< gameObjects.Length; i++)
                      {
                        gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                     }
                }
            }
        }
        if (data != null)
        {
            applyGravity();
          
        }

        if(data != null)
        {
            if (GUI.Button(new Rect(220,0,100,50), "Random GPU"))
            {
                int totalSize = 4 * sizeof(float) + 3 * sizeof(float);
                ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);//utilizando GPU
                computeBuffer.SetData(data); //copiar dados da cpu pra gpu
                computeShader.SetBuffer(0, "cubes", computeBuffer);
                computeShader.SetInt("iteraction", iteractions);
                computeShader.Dispatch(0, data.Length / 10, 1, 1);

                computeBuffer.GetData(data); //passando os dados de volta para a cpu
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color",data[i].color);
                }

                computeBuffer.Dispose();//libera a memoria q havia sido alocada

            }
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
                GameObject go = GameObject.Instantiate(modelPref, new Vector3(offsetX * 0.7f, offsetY * 0.7f,0 ), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _color);

                gameObjects[i * count + j] = go;
                data[i * count + j] = new Cube();
                data[i * count + j].position = go.transform.position;
                data[i * count + j].color = _color;
            }
        }
    }

    public static void changeColor(GameObject ga)
    {
        Color _color = Random.ColorHSV();
        Material objeto = ga.GetComponent<MeshRenderer>().material;
        objeto.color = _color;
    }

    private void applyGravity()
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


    }


}
