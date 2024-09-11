using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine;

public class PopUpAnim : MonoBehaviour
{
    public AnimationCurve _opacityCurve;
    public AnimationCurve _scaleCurve;
    public AnimationCurve _heightCurve;

    private TextMeshProUGUI _tmp;

    private Vector3 _origin;

    private float _time = 0;

    private void Awake()
    {
        _tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _origin = transform.position;
    }

    void Update()
    {
        _tmp.color = new Color(1, 1, 1, _opacityCurve.Evaluate(_time));
        transform.localScale = Vector3.one * _scaleCurve.Evaluate(_time);
        transform.position = _origin + new Vector3(0, 1 + _heightCurve.Evaluate(_time));

        _time += Time.deltaTime;
    }
}