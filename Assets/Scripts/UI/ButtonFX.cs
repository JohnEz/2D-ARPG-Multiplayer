using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField]
    private AudioClip _hoverSound;

    [SerializeField]
    private AudioClip _clickSound;

    [SerializeField]
    private bool _shouldScale = true;

    private float _hoverScaleModifier = 1.1f;
    private const float SCALE_DURATION = .25f;

    private Vector3 _baseScale;
    private Vector3 _hoverScale;

    private RectTransform _rectTransform;

    private Button _myButton;

    private void Awake() {
        _baseScale = transform.localScale;
        _hoverScale = new Vector3(_baseScale.x * _hoverScaleModifier, _baseScale.x * _hoverScaleModifier, _baseScale.x * _hoverScaleModifier);

        _rectTransform = GetComponent<RectTransform>();
        _myButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PlaySound(_hoverSound);

        if (_shouldScale) {
            _rectTransform.DOScale(_hoverScale, SCALE_DURATION).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_shouldScale) {
            _rectTransform.DOScale(_baseScale, SCALE_DURATION).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
    }

    public void HandleClick() {
        // we want to play the sound even if the button is disabled because it can be disabled on click
        PlaySound(_clickSound, true);
    }

    private void PlaySound(AudioClip clip, bool playEvenIfDisabled = false) {
        if (!playEvenIfDisabled && !_myButton.IsInteractable()) {
            return;
        }

        AudioManager.Instance.PlaySound(clip);
    }
}