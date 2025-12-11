using _Project.Scripts.EventStructs;
using _Project.Scripts.InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    void OnTriggerEnter(Collider other)
    {
        //EventHub.Instance?.RaiseEvent(new InGameStructs.UnderDonuts());

        if(other.GetComponent<DonutObject>() != null)
        {
            IsOccupied = true;
            Debug.Log($"{gameObject.name} 스포너에 도넛 진입. 점유됨.");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<DonutObject>() != null)
        {
            IsOccupied = false;
            Debug.Log($"{gameObject.name}스포너에서 도넛 나감. 비어있음.");
        }
    }

}
