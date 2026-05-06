using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Controlador : MonoBehaviour
{
    public GameObject panSeco3D;
    public GameObject leche3D;
    public GameObject bandeja3D;

    public GameObject panMojado3D;
    public GameObject bolHuevo3D;
    public GameObject panRebozado3D;

    public GameObject aceite3D;
    public GameObject sarten3D;
    public GameObject sartenLista3D;

    public GameObject panFrito3D;

    public GameObject azucar3D;
    public GameObject canela3D;
    public GameObject mezclaDulce3D;

    public GameObject panDulce3D;

    public GameObject plato3D;

    private bool panSeco, leche, bandeja, panMojado, bolHuevo, panRebozado, aceite, sarten, sartenLista,panFrito, azucar, canela, mezclaDulce, panDulce, plato, torrija ;


    void Start()
    {
        panSeco = leche = bandeja = panMojado = bolHuevo = panRebozado = aceite = sarten = sartenLista = panFrito = azucar = canela = mezclaDulce = panDulce = plato = torrija = false;
        
        mezclaDulce3D.SetActive(false);
    }

    void Update()
    {
 
    }

    public void Actualizar()
    {
        if (panSeco && leche && bandeja)
        {
            panMojado = true;
            panSeco = leche = false;

            panSeco3D.SetActive(false);
            leche3D.SetActive(false);
            panMojado3D.SetActive(true);

            panMojado3D.transform.SetParent(bandeja3D.transform);
            panMojado3D.transform.localPosition = new Vector3(0, 1.0f, 0);
        }

        if (panMojado && !bandeja)
        {
            panMojado=false;
            panMojado3D.SetActive(false);

            panSeco3D.SetActive(true);
            leche3D.SetActive(true);
        }

        if(aceite && sarten) 
        { 
            aceite = false;
            sarten = false;
            sartenLista = true;

            aceite3D.SetActive(false);
            sarten3D.SetActive(false);
            sartenLista3D.SetActive(true);
        }

        if(sartenLista && !sarten)
        {
            sartenLista = false;

            aceite3D.SetActive(true);
            sarten3D.SetActive(true);
            sartenLista3D.SetActive(false);
        }

        if (azucar && canela) 
        { 
            azucar = false;
            canela = false;
            mezclaDulce = true;

            azucar3D.SetActive(false);
            canela3D.SetActive(false);
            mezclaDulce3D.SetActive(true);
            mezclaDulce3D.transform.localPosition = azucar3D.transform.localPosition;

        }

        if(mezclaDulce && !azucar)
        {
            mezclaDulce = false;

            azucar3D.SetActive(true);
            canela3D.SetActive(true);
            mezclaDulce3D.SetActive(false);

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
        if(id == "bolHuevo") 
        {
            bolHuevo = true;
        }
        if(id == "aceite")
        {
            aceite = true;
        }
        if (id == "sarten")
        {
            sarten = true;
        }
        if (id == "azucar")
        {
            azucar = true;
        }
        if (id == "canela")
        {
            canela = true;
        }
        if (id == "plato")
        {
            plato = true;
        }
        Actualizar();
    }

    public void ITAusente(string id)
    {
        if (id == "leche")
        {
            leche = false;
        }
        if (id == "panSeco")
        {
            panSeco = false;
        }
        if (id == "bandeja")
        {
            bandeja = false;
        }
        if (id == "bolHuevo")
        {
            bolHuevo = false;
        }
        if (id == "aceite")
        {
            aceite = false;
        }
        if (id == "sarten")
        {
            sarten = false;
        }
        if (id == "azucar")
        {
            azucar = false;
        }
        if (id == "canela")
        {
            canela = false;
        }
        if (id == "plato")
        {
            plato = false;
        }
        Actualizar();
     
    }
}
