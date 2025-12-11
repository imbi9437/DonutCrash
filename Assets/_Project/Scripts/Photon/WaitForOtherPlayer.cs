using UnityEngine;

public class WaitForOtherPlayer : MonoBehaviour
{
    void Start()
    {
        PhotonManager.CheckStartable();
    }
}
