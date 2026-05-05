using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    public GameObject panSecoO;
    public GameObject lecheO;
    public GameObject bandejaO;
    public GameObject panMojadoO;
    private bool panSeco, leche, bandeja, panMojado;


    void Start()
    {
        panSeco = leche = bandeja = panMojado = false;
    }

    void Update()
    {
        if (panSeco && leche && bandeja)
        {
            panMojado = true;
            panSeco = leche = false;
        }
    }

    public void ITPresente(string id)
    {
        if (id == "leche")
        {
            leche = true;
        }
        if (id == "panSeco")
        {
            panSeco = true;
        }
        if (id == "bandeja")
        {
            bandeja = true;
        }
    }

    public void ITAusente(string id)
    {
        if (id == "leche")
        {
            leche = true;
        }
        if (id == "panSeco")
        {
            panSeco = true;
        }
        if (id =="bandeja")
        {
            bandeja = true;
        }
    }
}
