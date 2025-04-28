using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [Header("FPS")]
    public TextMeshProUGUI fPSText;
    public float updateRate = 4.0f;

    private float frameCount;
    private float deltaTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        deltaTime += Time.deltaTime;
        if (deltaTime > 1.0 / updateRate)
        {
            fPSText.text = "FPS: " + (frameCount / deltaTime).ToString("0.00");
            frameCount = 0;
            deltaTime -= 1.0f / updateRate;
        }
        

    }
}
