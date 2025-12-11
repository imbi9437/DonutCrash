using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damage;

    private Sequence _sequence;

    public void Initialize(int damage, bool isCrit)
    {
        string prefix = isCrit ? "!" : "";
        this.damage.text = $"{prefix}{damage.ToString()}";
        this.damage.color = isCrit ? Color.red : Color.white;
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        transform.localScale = Vector3.one;
        _sequence.Append(transform.DOScale(Vector3.one * 1.2f, .1f)
            .SetEase(Ease.OutCubic));
        _sequence.Append(transform.DOScale(Vector3.one, .3f))
            .SetEase(Ease.InCubic);
        _sequence.Join(transform.DOLocalMoveZ(transform.position.z + 2f, 2f)
            .SetEase(Ease.InOutCubic));
        _sequence.Join(this.damage.DOFade(0f, 2f)
            .SetEase(Ease.InCubic));
        _sequence.OnComplete(() =>Destroy(gameObject));
    }
}
