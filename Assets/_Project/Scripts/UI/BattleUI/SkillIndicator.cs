using DG.Tweening;
using TMPro;
using UnityEngine;

public class SkillIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    private Sequence _sequence;

    public void Initialize(string str, bool isAdd)
    {
        text.text = str;
        text.color = Color.white;
        
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        transform.localScale = Vector3.zero;
        _sequence.Append(transform.DOScale(Vector3.one, .2f)
            .SetEase(Ease.OutCubic));
        _sequence.Join(text.DOColor(isAdd ? Color.blue : Color.red, .2f)
            .SetEase(Ease.OutCubic));
        _sequence.Append(transform.DOScale(Vector3.one * 1.2f, 2f));
        _sequence.Append(transform.DOScale(Vector3.zero, .2f)
            .SetEase(Ease.InCubic));
        _sequence.Join(text.DOColor(Color.clear, .2f)
            .SetEase(Ease.InCubic));
        _sequence.OnComplete(() => Destroy(gameObject));
    }
}
