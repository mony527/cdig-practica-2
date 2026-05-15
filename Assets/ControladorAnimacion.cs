using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAnimacion : MonoBehaviour
{
    public ControladorAR controladorAR;
    public ControladorUI controladorUI;
    public GameObject prefabPan;

    private float velocidad = 5.0f;
    private float distanciaUmbral = 0.1f;
    private Vector3 offsetPosicion = new Vector3(0, 2.0f, 0);
    private Vector3 offsetRotacion = new Vector3(0, 90, 0);

    private GameObject panAnimado;
    private int indiceActual = 0;
    private string[] secuenciaIDs = { "bandeja", "bolHuevo", "sarten", "azucar", "plato" };
    private bool animacionActivaAnteriormente = false;

    void Start()
    {
        if (prefabPan != null)
        {
            panAnimado = Instantiate(prefabPan);
            panAnimado.name = "Pan_Animacion_Loop";
            panAnimado.SetActive(false);
        }
    }

    void Update()
    {
        bool recetaCompleta = controladorUI != null && controladorUI.IsRecetaCompleta();
        bool teclaPulsada = Input.GetKey(KeyCode.A);

        if (recetaCompleta && teclaPulsada)
        {
            if (!animacionActivaAnteriormente)
            {
                IniciarAnimacion();
            }
            EjecutarAnimacion();
            animacionActivaAnteriormente = true;
        }
        else
        {
            if (animacionActivaAnteriormente)
            {
                ResetearAnimacion();
            }
            animacionActivaAnteriormente = false;
        }
    }

    private void IniciarAnimacion()
    {
        if (panAnimado != null)
        {
            indiceActual = 0;
            panAnimado.SetActive(true);

            controladorAR.SetVisibilidadIngrediente("panDulce", false);

            Transform primerElemento = controladorAR.GetDetectorTransform(secuenciaIDs[0]);
            if (primerElemento != null)
            {
                panAnimado.transform.position = primerElemento.position + primerElemento.TransformDirection(offsetPosicion);
                panAnimado.transform.rotation = primerElemento.rotation * Quaternion.Euler(offsetRotacion);
            }
        }
    }

    private void EjecutarAnimacion()
    {
        if (panAnimado != null)
        {
            string idObjetivo = secuenciaIDs[indiceActual];
            Transform targetTransform = controladorAR.GetDetectorTransform(idObjetivo);

            if (targetTransform != null)
            {
                Vector3 desplazamientoGlobal = targetTransform.TransformDirection(offsetPosicion);
                Vector3 targetPosicion = targetTransform.position + desplazamientoGlobal;
                Quaternion targetRotacion = targetTransform.rotation * Quaternion.Euler(offsetRotacion);

                panAnimado.transform.position = Vector3.MoveTowards(
                    panAnimado.transform.position,
                    targetPosicion,
                    velocidad * Time.deltaTime
                );

                panAnimado.transform.rotation = Quaternion.Slerp(
                    panAnimado.transform.rotation,
                    targetRotacion,
                    velocidad * Time.deltaTime
                );

                if (Vector3.Distance(panAnimado.transform.position, targetPosicion) < distanciaUmbral)
                {
                    indiceActual = (indiceActual + 1) % secuenciaIDs.Length;
                }
            }
            else
            {
                indiceActual = (indiceActual + 1) % secuenciaIDs.Length;
            }
        }
    }

    private void ResetearAnimacion()
    {
        if (panAnimado != null)
        {
            panAnimado.SetActive(false);
            indiceActual = 0;
            if (controladorUI.IsRecetaCompleta()) controladorAR.SetVisibilidadIngrediente("panDulce", true);
        }
    }
}
