using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

internal struct CombatTextParams {
    public string text;
    public Color colour;
    public float fontSize;
}

public class CharacterCanvasController : MonoBehaviour {
    private const float COMBAT_TEXT_THROTTLE = 0.1f;

    public GameObject combatTextPrefab;
    public GameObject buffIconPrefab;

    public HpBarController hpBar;
    [SerializeField] private CastBarController castBar;
    public Transform UnitFrameTransform;

    private Queue<CombatTextParams> combatTextQueue = new Queue<CombatTextParams>();

    private bool canCreateCombatText = true;

    private CharacterController _characterController;
    private CharacterStateController _stateController;
    private int myTeam;

    private Color[] teamColours = new Color[] {
        new Color (0.7294f, 0.9569f, 0.1176f), //green
        new Color(0.8039f, 0.4039f, 0.2039f), //red
        new Color (0, 0.9647f, 1), //blue
        //new Color (0.8431f, 0.2f, 0.2f), //red
    };

    private void Awake() {
        _characterController = GetComponentInParent<CharacterController>();
        _stateController = GetComponentInParent<CharacterStateController>();
    }

    private void Start() {
        Initialise();
    }

    private void OnEnable() {
        _stateController.OnDeath += HandleCharacterDeath;
    }

    private void OnDisable() {
        _stateController.OnDeath -= HandleCharacterDeath;
    }

    private void HandleCharacterDeath() {
        UnitFrameTransform.gameObject.SetActive(false);
    }

    private void Update() {
        //TODO there might be a better way to not check for this each time as well
        if (combatTextQueue.Count > 0 && canCreateCombatText) {
            CombatTextParams combatText = combatTextQueue.Dequeue();
            CreateCombatText(combatText.text, combatText.colour, combatText.fontSize);
        }
    }

    public void Initialise() {
        //myTeam = (int)myCharacter.faction;

        NetworkStats myStats = _characterController.GetComponent<NetworkStats>();
        hpBar.Initialize(myStats);
        //hpBar.SetHPColor(teamColours[myTeam]);

        CastController castController = _characterController.GetComponent<CastController>();
        castBar.Initialize(castController);

        myStats.OnTakeDamage += OnTakeDamage;
        //myCharacter.OnReceiveHealing.AddListener(CreateHealText);
    }

    private void OnTakeDamage(int damage, bool isShield, bool sourceIsPlayer, CharacterController source) {
        // we only want to show combat text to the client that causes the damage
        if (!sourceIsPlayer) {
            return;
        }

        if (isShield) {
            CreateShieldText(damage);
        } else {
            CreateDamageText(damage);
        }
    }

    public void CreateDamageText(int damage) {
        float fontSize = CalculateCombatTextFontSize(damage);
        //CreateCombatText(damage.ToString(), new Color(0.8431f, 0.2f, 0.2f), fontSize);
        CreateCombatText(damage.ToString(), new Color(0.845f, 0.8607f, 0.8961f), fontSize);
    }

    public void CreateHealText(int healing) {
        float fontSize = CalculateCombatTextFontSize(healing);
        CreateCombatText(healing.ToString(), new Color(0.7294f, 0.9569f, 0.1176f), fontSize);
    }

    public void CreateShieldText(int shield) {
        CreateCombatText($"({shield.ToString()})", new Color(0.445f, 0.4607f, 0.4961f), CombatText.MIN_FONT_SIZE);
    }

    public void CreateBasicText(string text) {
        CreateCombatText(text, new Color(1f, 1f, 1f), 0.5f);
    }

    private float CalculateCombatTextFontSize(float value) {
        float minValue = 5;
        float maxValue = 30;

        float clampedValue = Mathf.Clamp(value, minValue, maxValue);

        float modValue = clampedValue - minValue;

        float fontSizeDif = CombatText.MAX_FONT_SIZE - CombatText.MIN_FONT_SIZE;

        float percentage = modValue / (maxValue - minValue);

        float fontSize = CombatText.MIN_FONT_SIZE + (percentage * fontSizeDif);

        return fontSize;
    }

    public void CreateCombatText(string text, Color colour, float fontSize) {
        if (canCreateCombatText) {
            SpawnCombatText(text, colour, fontSize);
        } else {
            CombatTextParams newText;
            newText.text = text;
            newText.colour = colour;
            newText.fontSize = fontSize;
            combatTextQueue.Enqueue(newText);
        }
    }

    public void SpawnCombatText(string text, Color color, float fontSize) {
        canCreateCombatText = false;
        GameObject newDamageText = Instantiate(combatTextPrefab, transform);

        //randomise X
        newDamageText.transform.localPosition = new Vector3(Random.value - 0.5f, newDamageText.transform.localPosition.y, newDamageText.transform.localPosition.z);

        newDamageText.GetComponent<CombatText>().Setup(text, color, fontSize);
        StartCoroutine(AllowCreateCombatText());
    }

    private IEnumerator AllowCreateCombatText() {
        yield return new WaitForSeconds(COMBAT_TEXT_THROTTLE);
        canCreateCombatText = true;
    }
}