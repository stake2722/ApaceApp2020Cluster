using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] TmLib.TmPlanets.Planet m_planetType = TmLib.TmPlanets.Planet.Earth;
    [SerializeField] Material m_ringMat = null;

    // Start is called before the first frame update
    void Start()
    {
        TmLib.TmPlanets.makeupToPlanet(m_planetType, gameObject, m_ringMat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
