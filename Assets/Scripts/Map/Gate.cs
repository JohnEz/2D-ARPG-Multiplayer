using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
public class Gate : MonoBehaviour {

    [SerializeField]
    private AudioClip closeSFX;

    [SerializeField]
    private AudioClip openSFX;

    [SerializeField]
    private Color closedColor = Color.white;

    [SerializeField]
    private Color openColor = Color.white;

    [SerializeField]
    private Sprite closedSprite;

    [SerializeField]
    private Sprite openSprite;

    [SerializeField]
    private bool startsOpen = true;

    [SerializeField]
    private List<AiBrain> OnEnterCombatList;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Animator animator;

    private bool isOpen;

    // Start is called before the first frame update
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        if (startsOpen) {
            isOpen = false;
            Open();
        } else {
            isOpen = true;
            Close();
        }
    }

    private void OnEnable() {
        OnEnterCombatList.ForEach(ai => {
            ai.OnEnterCombat += Close;
            ai.OnDeath += Open;
        });
    }

    private void OnDisable() {
        OnEnterCombatList.ForEach(ai => {
            ai.OnEnterCombat -= Close;
            ai.OnDeath -= Open;
        });
    }

    public void Open() {
        if (isOpen) {
            return;
        }

        boxCollider.enabled = false;
        isOpen = true;
        spriteRenderer.color = openColor;
        animator?.SetBool("IsOpen", isOpen);
        AudioManager.Instance.PlaySound(openSFX, transform);
    }

    public void Close() {
        if (!isOpen) {
            return;
        }

        boxCollider.enabled = true;
        isOpen = false;
        spriteRenderer.color = closedColor;
        animator?.SetBool("IsOpen", isOpen);
        AudioManager.Instance.PlaySound(closeSFX, transform);
    }
}