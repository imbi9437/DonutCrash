using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    public string animationName;
    public GameObject clips;
    public bool isUnscaledTime;
    public bool usePosition = true;
    public bool useRotation = true;
    
    private Dictionary<string, ProceduralAnimation> animations;
    private List<ProceduralAnimationTransition> animationTransitionList;
    private Queue<ProceduralAnimationTransition> animationTransitionRemoveQueue;
    private List<ProceduralEffectTransition> effectTransitionList;
    private Queue<ProceduralEffectTransition> effectTransitionRemoveQueue;
    private TransformContainer transformContainer;

    void Start()
    {
        animations = new Dictionary<string, ProceduralAnimation>();
        animationTransitionList = new List<ProceduralAnimationTransition>();
        animationTransitionRemoveQueue = new Queue<ProceduralAnimationTransition>();
        effectTransitionList = new List<ProceduralEffectTransition>();
        effectTransitionRemoveQueue = new Queue<ProceduralEffectTransition>();
        transformContainer = new TransformContainer();
        
        foreach (Transform i in clips.transform)
        {
            ProceduralAnimation anim = i.GetComponent<ProceduralAnimation>();
            animations.Add(anim.animationName, anim);
        }
    }

    private void Update()
    {
        foreach (ProceduralAnimationTransition i in animationTransitionList)
        {
            i.normalizedTime += (isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / i.duration;
            if (i.NextAnimating(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) > 1f)
            {
                i.proceduralAnimation.weight = i.endWeight;
                animationTransitionRemoveQueue.Enqueue(i);
            }
        }

        foreach (ProceduralEffectTransition i in effectTransitionList)
        {
            i.normalizedTime += (isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / i.duration;
            if (i.NextAnimating(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) > 1f)
            {
                i.proceduralAnimationEffect.weight = i.endWeight;
                effectTransitionRemoveQueue.Enqueue(i);
            }
        }

        transformContainer.Reset();
        foreach (ProceduralAnimation anim in animations.Values)
        {
            transformContainer += anim.GetTransform(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        
        if (usePosition) transform.localPosition = transformContainer.position;
        if (useRotation) transform.localRotation = Quaternion.Euler(transformContainer.rotation);

        while (animationTransitionRemoveQueue.Count > 0)
        {
            ProceduralAnimationTransition proceduralAnimationTransition = animationTransitionRemoveQueue.Dequeue();
            animationTransitionList.Remove(proceduralAnimationTransition);
        }

        while (effectTransitionRemoveQueue.Count > 0)
        {
            ProceduralEffectTransition proceduralEffectTransition = effectTransitionRemoveQueue.Dequeue();
            effectTransitionList.Remove(proceduralEffectTransition);
        }
    }

    /// <summary>
    /// 애니메이션의 비중전환을 추가하는 메서드
    /// 해당 메서드를 통해 비중 전환을 시작하면 이전에 동일한 애니메이션의 비중 전환은 취소 됨
    /// </summary>
    /// <param name="animationName">전환할 애니메이션 이름</param>
    /// <param name="endWeight">완료시의 비중</param>
    /// <param name="duration">완료까지의 시간</param>
    /// <param name="ease">비중 전환 방식, 기본 : Linear</param>
    public void AddAnimationTransition(string animationName, float endWeight, float duration, EaseType ease = EaseType.Linear)
    {
        animationTransitionList.RemoveAll(obj => obj.proceduralAnimation.animationName == animationName);
        animationTransitionList.Add(new ProceduralAnimationTransition(animations[animationName], endWeight, duration, ease));
    }

    /// <summary>
    /// 애니메이션 이펙트의 비중전환을 추가하는 메서드
    /// 해당 메서드를 통해 비중 전환을 시작하면 이전에 동일한 애니메이션의 비중 전환은 취소 됨
    /// </summary>
    /// <param name="animationName">전환할 애니메이션 이름</param>
    /// <param name="endWeight">완료시의 비중</param>
    /// <param name="duration">완료까지의 시간</param>
    /// <param name="ease">비중 전환 방식, 기본 : Linear</param>
    public void AddEffectTransition(string animationName, string effectName, float endWeight, float duration, EaseType ease = EaseType.Linear)
    {
        effectTransitionList.RemoveAll(obj => obj.animationName == animationName && obj.proceduralAnimationEffect.effectName == effectName);
        effectTransitionList.Add(new ProceduralEffectTransition(animations[animationName].animationName, animations[animationName].FindEffect(effectName), endWeight, duration, ease));
    }
}

public class ProceduralAnimationTransition
{
    public ProceduralAnimation proceduralAnimation;
    public float startWeight;
    public float endWeight;
    public float weightVector;
    public float duration;
    public EaseType ease;
    public float normalizedTime;

    private float progress;

    /// <summary>
    /// 새로운 절차생성 애니메이션 비중 전환 정보 생성자
    /// </summary>
    /// <param name="proceduralAnimation">전환할 절차 애니메이션의 이름</param>
    /// <param name="endWeight">전환 완료시의 비중</param>
    /// <param name="duration">전환 완료에 필요한 시간</param>
    public ProceduralAnimationTransition(ProceduralAnimation proceduralAnimation, float endWeight, float duration)
    {
        this.proceduralAnimation = proceduralAnimation;
        this.startWeight = proceduralAnimation.weight;
        this.endWeight = endWeight;
        weightVector = this.endWeight - startWeight;
        this.duration = duration;
        normalizedTime = 0f;
        progress = 0f;
        this.ease = EaseType.Linear;
    }

    /// <summary>
    /// 새로운 절차생성 애니메이션 비중 전환 정보 생성자
    /// EaseType타입의 파라미터를 통해 전환 보간 메서드를 선택
    /// </summary>
    /// <param name="ease">전환 메서드</param>
    public ProceduralAnimationTransition(ProceduralAnimation proceduralAnimation, float endWeight, float duration, EaseType ease) :  this(proceduralAnimation, endWeight, duration)
    {
        this.ease = ease;
    }

    /// <summary>
    /// 파라미터로 입력한 deltaTime만큼 이동한 다음 애니메이션을 진행 한후 normalizedTime을 반환합니다.
    /// </summary>
    /// <returns>전환 효과의 normalizedTime</returns>
    public float NextAnimating(float deltaTime)
    {
        normalizedTime += deltaTime / duration;

        switch (ease)
        {
            case EaseType.Linear:
                progress = normalizedTime;
                break;
            case EaseType.InCubic:
                progress = normalizedTime * normalizedTime * normalizedTime;
                break;
            case EaseType.OutCubic:
                progress = 1 - Mathf.Pow(1 - normalizedTime, 3);
                break;
            case EaseType.InOutCubic:
                progress = normalizedTime < .5f
                    ? 4 * normalizedTime * normalizedTime * normalizedTime
                    : 1 - Mathf.Pow(-2 * normalizedTime + 2, 3) / 2;
                break;
        }
        
        proceduralAnimation.weight = startWeight + (endWeight - startWeight) * progress;

        return normalizedTime;
    }
}

public class ProceduralEffectTransition
{
    public string animationName;
    public ProceduralAnimationEffect proceduralAnimationEffect;
    public float startWeight;
    public float endWeight;
    public float weightVector;
    public float duration;
    public EaseType ease;
    public float normalizedTime;

    private float progress;

    /// <summary>
    /// 새로운 절차생성 애니메이션의 이펙트 비중 전환 정보 생성자
    /// </summary>
    /// <param name="proceduralAnimation">전환할 절차 애니메이션의 이름</param>
    /// <param name="endWeight">전환 완료시의 비중</param>
    /// <param name="duration">전환 완료에 필요한 시간</param>
    public ProceduralEffectTransition(string animationName, ProceduralAnimationEffect proceduralAnimationEffect, float endWeight, float duration)
    {
        this.animationName = animationName;
        this.proceduralAnimationEffect = proceduralAnimationEffect;
        this.startWeight = proceduralAnimationEffect.weight;
        this.endWeight = endWeight;
        weightVector = this.endWeight - startWeight;
        this.duration = duration;
        normalizedTime = 0f;
        progress = 0f;
        this.ease = EaseType.Linear;
    }

    /// <summary>
    /// 새로운 절차생성 애니메이션의 이펙트 비중 전환 정보 생성자
    /// EaseType타입의 파라미터를 통해 전환 보간 메서드를 선택
    /// </summary>
    /// <param name="ease">전환 메서드</param>
    public ProceduralEffectTransition(string animationName, ProceduralAnimationEffect proceduralAnimationEffect, float endWeight, float duration, EaseType ease) : this(animationName, proceduralAnimationEffect, endWeight, duration)
    {
        this.ease = ease;
    }

    /// <summary>
    /// 파라미터로 입력한 deltaTime만큼 이동한 다음 애니메이션을 진행 한후 normalizedTime을 반환합니다.
    /// </summary>
    /// <returns>전환 효과의 normalizedTime</returns>
    public float NextAnimating(float deltaTime)
    {
        normalizedTime += deltaTime / duration;

        switch (ease)
        {
            case EaseType.Linear:
                progress = normalizedTime;
                break;
            case EaseType.InCubic:
                progress = normalizedTime * normalizedTime * normalizedTime;
                break;
            case EaseType.OutCubic:
                progress = 1 - Mathf.Pow(1 - normalizedTime, 3);
                break;
            case EaseType.InOutCubic:
                progress = normalizedTime < .5f
                    ? 4 * normalizedTime * normalizedTime * normalizedTime
                    : 1 - Mathf.Pow(-2 * normalizedTime + 2, 3) / 2;
                break;
        }

        proceduralAnimationEffect.weight = startWeight + (endWeight - startWeight) * progress;

        return normalizedTime;
    }
}

public enum EaseType
{
    Linear,
    InCubic,
    OutCubic,
    InOutCubic,
}
