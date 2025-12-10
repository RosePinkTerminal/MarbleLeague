using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public static FPSCounter Instance { get; private set; }
    
    public TextMeshProUGUI fpsText;
    
    private float updateInterval = 0.5f; // update twice per second
    private float accum = 0.0f;
    private int frames = 0;
    private float timeLeft;
    private Canvas canvas;

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        timeLeft = updateInterval;

        // Create canvas 
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FPS Canvas");
            canvasObj.transform.SetParent(transform);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999; // Make sure it's on top
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
        }

        // Create FPS text 
        if (fpsText == null)
        {
            GameObject textObj = new GameObject("FPS Counter Text");
            textObj.transform.SetParent(canvas.transform);
            
            fpsText = textObj.AddComponent<TextMeshProUGUI>();
            
            // top-right corner pos
            RectTransform rectTransform = fpsText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10);
            rectTransform.sizeDelta = new Vector2(200, 50);
            
            // Style
            fpsText.fontSize = 24;
            fpsText.color = Color.yellow;
            fpsText.alignment = TextAlignmentOptions.TopRight;
            fpsText.fontStyle = FontStyles.Bold;
        }
    }

    void Update()
    {
        // should FPS be visible?
        if (GameSettings.Instance != null)
        {
            bool shouldShow = GameSettings.Instance.GetShowFPS();
            if (fpsText != null)
                fpsText.gameObject.SetActive(shouldShow);
            
            if (!shouldShow)
                return;
        }

        // calc FPS (BTW if anyone joined the stream recently, calc stands for calculator, just using slang guys) https://www.youtube.com/shorts/48ZlFou5oyc
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        // update display on interval
        if (timeLeft <= 0.0f)
        {
            float fps = accum / frames;
            
            if (fpsText != null)
            {
                fpsText.text = string.Format("FPS: {0:F0}", fps);
                
                // set color based on performance
                if (fps >= 60)
                    fpsText.color = Color.green;
                else if (fps >= 30)
                    fpsText.color = Color.yellow;
                else
                    fpsText.color = Color.red;
            }

            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}
