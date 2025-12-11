using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CAS = _Project.Scripts.EventStructs.CameraAnimationStructs;

public class CameraAnimator : MonoBehaviour
{
    public GameObject clips;
    public bool isUnscaledTime;
    
    private Dictionary<string, ProceduralAnimation> animations;
    private List<ProceduralAnimation> animationList;
    private List<ProceduralAnimation> animationRemoveList;
    private TransformContainer transformContainer;
    
    void Start()
    {
        animations = new Dictionary<string, ProceduralAnimation>();
        animationList = new List<ProceduralAnimation>();
        transformContainer = new TransformContainer();
        
        foreach (Transform i in clips.transform)
        {
            ProceduralAnimation anim = i.GetComponent<ProceduralAnimation>();
            animations.Add(anim.animationName, anim);
        }

        EventHub.Instance.RegisterEvent<CAS.RequestCameraAnimation>(RequestCameraAnimation);
    }

    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<CAS.RequestCameraAnimation>(RequestCameraAnimation);
    }

    private void Update()
    {
        transformContainer.Reset();
        foreach (ProceduralAnimation anim in animationList)
        {
            transformContainer += anim.GetTransform(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        transform.localPosition = transformContainer.position;
        transform.localRotation = Quaternion.Euler(transformContainer.rotation);
        // transform.localScale = transformContainer.scale;

        animationRemoveList = animationList.FindAll(obj => obj.animationTime > 2f);
        foreach (ProceduralAnimation anim in animationRemoveList)
        {
            animationList.Remove(anim);
            Destroy(anim.gameObject);
        }
    }

    private void RequestCameraAnimation(CAS.RequestCameraAnimation evt) => AddAnimation(evt.clip, evt.weight);

    private void AddAnimation(string animationName, float weight)
    {
        ProceduralAnimation anim = Instantiate(animations[animationName], transform);
        anim.weight = weight;
        animationList.Add(anim);
    }
}
