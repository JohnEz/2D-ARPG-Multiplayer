using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasGroup))]
public class Tooltip : Singleton<Tooltip> {

    [SerializeField]
    private float fadeDuration;

    public enum Anchor {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
        Left,
        Right,
        Top,
        Bottom
    }

    public enum Anchoring {
        Corners,
        LeftOrRight,
        TopOrBottom
    }

    public enum Corner : int {
        BottomLeft = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3,
    }

    private RectTransform _rect;

    private CanvasGroup _canvasGroup;

    private RectTransform _anchorRect;

    private Canvas _canvas;

    private bool _isShown;

    //private bool _isTransitioning = false;

    private bool _transitionToState = false;

    private TooltipLines _tooltipLines = new TooltipLines();

    [SerializeField]
    private Vector2 _anchoredOffset = Vector2.zero;

    [SerializeField]
    private Anchoring _anchoring = Anchoring.Corners;

    [SerializeField]
    private Image _anchorGraphic;

    [SerializeField]
    private Vector2 _anchorGraphicOffset;

    [SerializeField]
    private GameObject _linePrefab;

    [SerializeField]
    private GameObject _columnPrefab;

    private float Alpha { get => _canvasGroup.alpha; set => _canvasGroup.alpha = value; }

    private void Awake() {
        _rect = GetComponent<RectTransform>();

        _canvasGroup = GetComponent<CanvasGroup>();

        _canvas = GetComponentInParent<Canvas>();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        Alpha = 0;
        _isShown = false;
        OnHide();
    }

    private void Start() {
        _rect.anchorMin = new Vector2(0.5f, 0.5f);
        _rect.anchorMax = new Vector2(0.5f, 0.5f);
    }

    private void UpdatePositionAndAnchor() {
        UpdatePivot();

        if (_anchorRect != null) {
            Vector3[] targetWorldCorners = new Vector3[4];
            _anchorRect.GetWorldCorners(targetWorldCorners);

            if (_anchoring == Anchoring.Corners) {
                Corner pivotCorner = VectorPivotToCorner(_rect.pivot);

                Corner oppositeCorner = GetOppositeCorner(pivotCorner);

                Vector2 pivotBasedOffset = new Vector2(
                    _rect.pivot.x == 1f ? _anchoredOffset.x * -1f : _anchoredOffset.x,
                    _rect.pivot.y == 1f ? _anchoredOffset.y * -1f : _anchoredOffset.y
                );

                Vector2 anchorPoint = _canvas.transform.InverseTransformPoint(targetWorldCorners[(int)oppositeCorner]);

                _rect.anchoredPosition = pivotBasedOffset + anchorPoint;
            } else if (_anchoring == Anchoring.LeftOrRight || _anchoring == Anchoring.TopOrBottom) {
                Vector2 topleft = _canvas.transform.InverseTransformPoint(targetWorldCorners[1]);

                if (_anchoring == Anchoring.LeftOrRight) {
                    Vector2 pivotBasedOffset = new Vector2(
                        _rect.pivot.x == 1f ? _anchoredOffset.x * -1f : _anchoredOffset.x,
                        _anchoredOffset.y
                    );

                    _rect.anchoredPosition = topleft + pivotBasedOffset + new Vector2(
                        _rect.pivot.x == 0 ? _anchorRect.rect.width : 0f,
                        (_anchorRect.rect.height / 2f) * -1f
                    );
                } else if (_anchoring == Anchoring.TopOrBottom) {
                    Vector2 pivotBasedOffset = new Vector2(
                        _anchoredOffset.x,
                        _rect.pivot.y == 1f ? _anchoredOffset.y * -1f : _anchoredOffset.y
                    );

                    _rect.anchoredPosition = topleft + pivotBasedOffset + new Vector2(
                        _anchorRect.rect.width / 2f,
                        _rect.pivot.y == 0 ? 0f : _anchorRect.rect.height * -1f
                    );
                }
            }
        }

        // force tooltip to nearest even number
        _rect.anchoredPosition = new Vector2(Mathf.Round(_rect.anchoredPosition.x), Mathf.Round(_rect.anchoredPosition.y));
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x + (_rect.anchoredPosition.x % 2f), _rect.anchoredPosition.y + (_rect.anchoredPosition.y % 2f));
    }

    public void UpdatePivot() {
        Vector3 targetPosition = Input.mousePosition;

        if (_anchoring == Anchoring.Corners) {
            Vector2 corner = new Vector2(
                ((targetPosition.x > (Screen.width / 2f)) ? 1f : 0f),
                ((targetPosition.y > (Screen.height / 2f)) ? 1f : 0f)
            );

            SetPivot(corner);
        } else if (_anchoring == Anchoring.LeftOrRight) {
            Vector2 pivot = new Vector2(((targetPosition.x > (Screen.width / 2f)) ? 1f : 0f), 0.5f);

            SetPivot(pivot);
        } else if (_anchoring == Anchoring.TopOrBottom) {
            Vector2 pivot = new Vector2(0.5f, ((targetPosition.y > (Screen.height / 2f)) ? 1f : 0f));

            SetPivot(pivot);
        }
    }

    public void SetAnchor(RectTransform anchorRect) {
        _anchorRect = anchorRect;
    }

    public void SetWidth(float width) {
        _rect.sizeDelta = new Vector2(width, _rect.sizeDelta.y);
    }

    public void SetPivot(Vector2 pivot) {
        _rect.pivot = pivot;

        UpdateAnchorGraphicPosition();
    }

    public void SetPivot(Corner point) {
        switch (point) {
            case Corner.BottomLeft:
            SetPivot(new Vector2(0f, 0f));
            break;

            case Corner.BottomRight:
            SetPivot(new Vector2(1f, 0f));
            break;

            case Corner.TopLeft:
            SetPivot(new Vector2(0f, 1f));
            break;

            case Corner.TopRight:
            SetPivot(new Vector2(1f, 1f));
            break;
        }
    }

    private void UpdateAnchorGraphicPosition() {
        if (_anchorGraphic == null) {
            return;
        }

        RectTransform anchorRect = _anchorGraphic.transform as RectTransform;

        if (_anchoring == Anchoring.Corners) {
            anchorRect.gameObject.SetActive(true); // TODO remove
            anchorRect.pivot = Vector2.zero;

            anchorRect.anchorMax = _rect.pivot;
            anchorRect.anchorMin = _rect.pivot;

            anchorRect.anchoredPosition = new Vector2(
                _rect.pivot.x == 1f ? _anchorGraphicOffset.x * -1f : _anchorGraphicOffset.x,
                _rect.pivot.y == 1f ? _anchorGraphicOffset.y * -1f : _anchorGraphicOffset.y
            );

            anchorRect.localScale = new Vector3(
                _rect.pivot.x == 0f ? 1f : -1f,
                _rect.pivot.y == 0f ? 1f : -1f,
                anchorRect.localScale.z
            );
        } else {
            anchorRect.gameObject.SetActive(false); // TODO remove
            //TODO probably want another image for up down left and right that sticks out slightly
        }
    }

    public void Show() {
        UpdatePositionAndAnchor();

        CreateTooltipLines();

        FadeToState(true);
    }

    public void Hide() {
        _tooltipLines = new TooltipLines();

        FadeToState(false);
    }

    private void FadeToState(bool newState) {
        _transitionToState = newState;
        _canvasGroup.DOFade(newState ? 1f : 0f, fadeDuration).OnComplete(OnFadeComplete);
    }

    private void OnFadeComplete() {
        _isShown = _transitionToState;

        if (!_isShown) {
            OnHide();
        }
    }

    private void OnHide() {
        // clean all setup
        ClearTooltipLines();

        _anchorRect = null;
    }

    public static Corner VectorPivotToCorner(Vector2 pivot) {
        if (pivot.x == 0f && pivot.y == 0f) {
            return Corner.BottomLeft;
        } else if (pivot.x == 0f && pivot.y == 1f) {
            return Corner.TopLeft;
        } else if (pivot.x == 1f && pivot.y == 0f) {
            return Corner.BottomRight;
        }

        return Corner.TopRight;
    }

    public static Corner GetOppositeCorner(Corner corner) {
        switch (corner) {
            case Corner.BottomLeft:
            return Corner.TopRight;

            case Corner.BottomRight:
            return Corner.TopLeft;

            case Corner.TopLeft:
            return Corner.BottomRight;

            case Corner.TopRight:
            return Corner.BottomLeft;
        }

        return Corner.BottomLeft;
    }

    public static Anchor VectorPivotToAnchor(Vector2 pivot) {
        if (pivot.x == 0f && pivot.y == 0f) {
            return Anchor.BottomLeft;
        } else if (pivot.x == 0f && pivot.y == 1f) {
            return Anchor.TopLeft;
        } else if (pivot.x == 1f && pivot.y == 0f) {
            return Anchor.BottomRight;
        } else if (pivot.x == 0.5f && pivot.y == 0f) {
            return Anchor.Bottom;
        } else if (pivot.x == 0.5f && pivot.y == 1f) {
            return Anchor.Top;
        } else if (pivot.x == 0f && pivot.y == 0.5f) {
            return Anchor.Left;
        } else if (pivot.x == 1f && pivot.y == 0.5f) {
            return Anchor.Right;
        }

        return Anchor.TopRight;
    }

    // text configuration, maybe move all this out?
    private int _spacerHeight = 6;

    public void AddTitle(string text) {
        _tooltipLines.AddLine(text, LineStyle.Title);
    }

    public void AddSubTitle(string text) {
        _tooltipLines.AddLine(text, LineStyle.SubTitle);
    }

    public void AddText(string text) {
        _tooltipLines.AddLine(text, LineStyle.Default);
    }

    public void AddColumn(string text) {
        _tooltipLines.AddColumn(text, LineStyle.Default);
    }

    public void AddDescription(string text) {
        _tooltipLines.AddLine(text, LineStyle.Description);
    }

    public void AddSpacer() {
        RectOffset padding = new RectOffset(0, 0, _spacerHeight, 0);

        _tooltipLines.AddLine("", padding);
    }

    private void ClearTooltipLines() {
        foreach (Transform child in transform) {
            LayoutElement childElement = child.GetComponent<LayoutElement>();
            if (childElement == null || !childElement.ignoreLayout) {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateTooltipLines() {
        ClearTooltipLines();

        if (_tooltipLines == null || _tooltipLines.Lines.Count == 0) {
            return;
        }

        _tooltipLines.Lines.ForEach(line => {
            GameObject newLineObject = CreateLine(line.Padding);

            if (!string.IsNullOrEmpty(line.LeftText)) {
                CreateLineColumn(newLineObject.transform, line.LeftText, true, line.Style);
            }

            if (!string.IsNullOrEmpty(line.RightText)) {
                CreateLineColumn(newLineObject.transform, line.RightText, false, line.Style);
            }
        });
    }

    private GameObject CreateLine(RectOffset padding) {
        GameObject newLineObject = Instantiate(_linePrefab, transform);
        newLineObject.GetComponent<HorizontalLayoutGroup>().padding = padding;

        return newLineObject;
    }

    private GameObject CreateLineColumn(Transform lineTransform, string text, bool isLeft, LineStyle style) {
        GameObject newColumnObject = Instantiate(_columnPrefab, lineTransform);

        TMP_Text textRenderer = newColumnObject.GetComponent<TMP_Text>();
        textRenderer.text = text;
        textRenderer.alignment = isLeft ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.BottomRight;

        // TODO get styles from linestyle
        textRenderer.fontSize = style == LineStyle.Title ? 24 : 18;
        textRenderer.fontStyle = style == LineStyle.Title || style == LineStyle.SubTitle ? FontStyles.Bold : FontStyles.Normal;
        textRenderer.lineSpacing = 1;
        //textRenderer.color =

        return newColumnObject;
    }
}