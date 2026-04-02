using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public Light Light;

    public TextMeshProUGUI clockText;

    [Range(0, 24)]
    public float timeofDay = 12f;

    public float dayLengthInSeconds = 120f;


    public float dayExposure = 1.3f;
    public float nightExposure = 0.2f;

    public float dayAtmosphere = 1.0f;
    public float nightAtmosphere = 0.3f;
    private void Update()
    {
        UpdateTime();
        UpdateSunRotation();
        UpdateClockUI();
        UpdateSkybox();
    }
    void UpdateTime()
    {
        // Advance time based on real time
        timeofDay += (24f / dayLengthInSeconds) * Time.deltaTime;

        // Keep time in 0–24 range
        if (timeofDay >= 24f)
            timeofDay -= 24f;
    }

    void UpdateSunRotation()
    {     
        float angle = (timeofDay / 24f) * 360f;
        // Rotate sun
        Light.transform.rotation = Quaternion.Euler(angle - 90f, 0f, 0f);

        float dot = Vector3.Dot(Light.transform.forward, Vector3.down);
        float intensity = Mathf.Clamp01(dot);
            Light.intensity = intensity;
    }

    void UpdateClockUI()
    {
        if (clockText == null) return;

        int hours = Mathf.FloorToInt(timeofDay);
        int minutes = Mathf.FloorToInt((timeofDay - hours) * 60f);

        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    void UpdateSkybox()
    {
        if (RenderSettings.skybox == null) return;

        // 0 = night, 1 = day
        float t = Light.intensity;

        float exposure = Mathf.Lerp(nightExposure, dayExposure, t);
        float atmosphere = Mathf.Lerp(nightAtmosphere, dayAtmosphere, t);

        RenderSettings.skybox.SetFloat("_Exposure", exposure);
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", atmosphere);

       
        DynamicGI.UpdateEnvironment();
    }
}
