using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Text scoreText, hiText, livesText, bombsText, powerText, grazeText, messageText;
    private Image bossHPFill;
    private GameObject bossHPContainer;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => BuildUI();

    void BuildUI()
    {
        if (!GetComponent<Canvas>())
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 720);
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Right-side stats panel
        var panel = UI("StatsPanel", transform);
        Anchors(panel, new Vector2(0.78f, 0f), new Vector2(1f, 1f));
        panel.AddComponent<Image>().color = new Color(0.04f, 0.04f, 0.14f, 0.88f);

        float y = -24f;
        scoreText = Label(panel, "Score",  "0",    ref y);
        hiText    = Label(panel, "Hi",     "0",    ref y);
        livesText = Label(panel, "Lives",  "3",    ref y);
        bombsText = Label(panel, "Bombs",  "3",    ref y);
        powerText = Label(panel, "Power",  "1.00", ref y);
        grazeText = Label(panel, "Graze",  "0",    ref y);

        // Boss HP bar — background + filled image, no Slider needed
        bossHPContainer = UI("BossHP", transform);
        Anchors(bossHPContainer, new Vector2(0.02f, 0.955f), new Vector2(0.76f, 0.985f));
        bossHPContainer.AddComponent<Image>().color = new Color(0.15f, 0.05f, 0.15f);

        var fillGO = UI("Fill", bossHPContainer.transform);
        Anchors(fillGO, Vector2.zero, Vector2.one);
        bossHPFill = fillGO.AddComponent<Image>();
        bossHPFill.color = new Color(0.85f, 0.1f, 0.85f);
        bossHPFill.type = Image.Type.Filled;
        bossHPFill.fillMethod = Image.FillMethod.Horizontal;
        bossHPFill.fillAmount = 1f;
        bossHPContainer.SetActive(false);

        // Centre-screen message text
        var msgGO = UI("Message", transform);
        Anchors(msgGO, new Vector2(0.05f, 0.44f), new Vector2(0.74f, 0.6f));
        messageText = msgGO.AddComponent<Text>();
        messageText.font = Font();
        messageText.fontSize = 28;
        messageText.fontStyle = FontStyle.Bold;
        messageText.color = Color.yellow;
        messageText.alignment = TextAnchor.MiddleCenter;
    }

    // Creates a child GO with a RectTransform (required for UI elements in a Canvas)
    static GameObject UI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Anchors(GameObject go, Vector2 min, Vector2 max)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    Text Label(GameObject parent, string title, string value, ref float y)
    {
        var go = UI(title, parent.transform);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, y);
        rt.sizeDelta = new Vector2(0f, 46f);
        y -= 50f;

        var t = go.AddComponent<Text>();
        t.font = Font(); t.fontSize = 17;
        t.color = Color.white;
        t.alignment = TextAnchor.UpperCenter;
        t.text = $"{title}\n{value}";
        return t;
    }

    static Font Font() => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    void Update()
    {
        if (GameManager.Instance == null) return;
        var g = GameManager.Instance;
        if (scoreText) scoreText.text = $"Score\n{(int)g.Score:N0}";
        if (hiText)    hiText.text    = $"Hi\n{g.HighScore:N0}";
        if (livesText) livesText.text = $"Lives\n{g.Lives}";
        if (bombsText) bombsText.text = $"Bombs\n{g.Bombs}";
        if (powerText) powerText.text = $"Power\n{g.Power:F2}";
        if (grazeText) grazeText.text = $"Graze\n{g.Graze}";
    }

    public void ShowBossHP(bool show) { if (bossHPContainer) bossHPContainer.SetActive(show); }
    public void UpdateBossHP(float t) { if (bossHPFill) bossHPFill.fillAmount = Mathf.Clamp01(t); }

    public void ShowMessage(string msg, float dur) => StartCoroutine(ShowMsg(msg, dur));

    IEnumerator ShowMsg(string msg, float dur)
    {
        if (messageText) { messageText.text = msg; yield return new WaitForSeconds(dur); if (messageText) messageText.text = ""; }
    }
}
