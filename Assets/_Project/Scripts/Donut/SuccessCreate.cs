using _Project.Scripts.EventStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuccessCreate : MonoBehaviour
{
    [SerializeField] private Image successMark;


    void Awake()
    {
        EventHub.Instance?.RegisterEvent<RecipeEventStructs.RequestOnSuccessMark>(OnMarkRapper);
    }
    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.RequestOnSuccessMark>(OnMarkRapper);
    }

    private void OnMarkRapper(RecipeEventStructs.RequestOnSuccessMark evt) => OnOffMark(evt.ison);

    void Start()
    {
        OnOffMark(false);
    }
    private void OnOffMark(bool ison)
    {
        successMark.gameObject.SetActive(ison);
    }
}
