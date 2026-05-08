using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    [System.Serializable]
    public class IngredienteData
    {
        public string id;              
        public GameObject prefab;      
        [HideInInspector] public GameObject instancia; 
        [HideInInspector] public bool presente;        
    }

    public List<IngredienteData> ingredientes = new List<IngredienteData>();

    private Dictionary<string, IngredienteData> mapa;

    void Start()
    {
        mapa = new Dictionary<string, IngredienteData>();

        foreach (var ing in ingredientes)
            mapa.Add(ing.id, ing);
    }

    public void ITPresente(string id)
    {
        if (mapa.ContainsKey(id))
        {
            var ing = mapa[id];
            ing.presente = true;

            if (ing.instancia == null)
            {
                ing.instancia = Instantiate(ing.prefab);
                ing.instancia.name = ing.id + "_3D";
            }
            
            ing.instancia.SetActive(true);

            ActualizarEstados();
        }
    }

    public void ITAusente(string id)
    {
        if (!mapa.ContainsKey(id)) return;

        var ing = mapa[id];
        ing.presente = false;

        if (ing.instancia != null)
            ing.instancia.SetActive(false);

        ActualizarEstados();
    }

    private void ActualizarEstados()
    {
        // 1. Verificamos si los ingredientes base están presentes
        bool panMojado = mapa["panSeco"].presente &&
                         mapa["leche"].presente &&
                         mapa["bandeja"].presente;

        // 2. CRUCIAL: Solo entramos si la base está lista PERO el panMojado NO está presente aún
        if (panMojado && !mapa["panMojado"].presente)
        {
            // Desactivamos los visuales de los ingredientes viejos
            mapa["panSeco"].instancia.SetActive(false);
            mapa["leche"].instancia.SetActive(false);

            mapa["panMojado"].instancia.transform.SetParent(mapa["bandeja"].instancia.transform);
            mapa["panMojado"].instancia.transform.localPosition = new Vector3(0, 1.0f, 0);
           
        }
        if (panMojado && !mapa["bandeja"].presente)
        {
            mapa["panMojado"].presente = false;
            mapa["panMojado"].instancia.SetActive(false);

            mapa["panSeco"].instancia.SetActive(true);
            mapa["leche"].instancia.SetActive(true);
        }
    }
}
