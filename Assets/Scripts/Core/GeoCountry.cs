using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class GeoData
{
    public string status;
    public string country;
    public string query;
    public float lat; // ����
    public float lon; // �浵
}

[System.Serializable]
public class WeatherData
{
    public MainWeather main;
    public Weather[] weather;
}

[System.Serializable]
public class MainWeather
{
    public float temp;
}

[System.Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
}

public class GeoCountry : MonoBehaviour
{
    private string apiKey = "bb41774b64c4d57b0a4b24489e46cd69"; // ���⿡ OpenWeatherMap API Ű�� �Է��ϼ���.

    public List<Image> dayImages = new List<Image>();
    public List<Image> nightImages = new List<Image>();
    public GameObject objWeather;
    public Coroutine weatherCoroutine = null; // ���� ���� ���� �ڷ�ƾ�� ����

    private GeoData geoData; // ������� ������ �����͸� �����ϴ� ����

    public static GeoCountry Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(LateStartCoroutine());
    }

    IEnumerator LateStartCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (weatherCoroutine == null && SettingManager.weatherUse == 1)
            weatherCoroutine = StartCoroutine(GetGeoData());
        else
            objWeather.SetActive(false);
    }

    public void SaveWeatherUseData(int p_weatherUse)
    {
        switch (p_weatherUse)
        {
            case 0:
                if (weatherCoroutine != null)
                    StopUpdateWeather();
                break;
            case 1:
                if (weatherCoroutine == null)
                {
                    weatherCoroutine = StartCoroutine(GetGeoData());
                    objWeather.SetActive(true);
                }                
                break;
        }
    }

    public void StopUpdateWeather()
    {
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
            objWeather.SetActive(false);
        }
    }

    IEnumerator GetGeoData()
    {
        AllImagesHide();
        string geoUrl = "http://ip-api.com/json/?fields=status,country,query,lat,lon";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(geoUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                //Debug.LogError("GeoCountry : GeoData Error: " + webRequest.error);
            }
            else
            {
                geoData = JsonUtility.FromJson<GeoData>(webRequest.downloadHandler.text);
                if (geoData.status == "success")
                {
                    //Debug.Log($"GeoCountry : User's Country: \"{geoData.country}\"; IPAddress: \"{geoData.query}\"; Latitude: {geoData.lat}; Longitude: {geoData.lon}");
                    StartCoroutine(GetWeather(geoData.lat, geoData.lon)); // ���� ������ ó������ �ҷ���
                    StartCoroutine(RefreshWeatherAtSpecificTimes()); // ������ �ð��� ���� ������ ���ΰ�ħ
                }
                else
                {
                    //Debug.LogError("GeoCountry : Unsuccessful geo data request.");
                }
            }
        }
    }

    IEnumerator GetWeather(float lat, float lon)
    {
        string weatherUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(weatherUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                //Debug.LogError("GeoCountry : Weather Error: " + webRequest.error);
            }
            else
            {
                WeatherData weatherData = JsonUtility.FromJson<WeatherData>(webRequest.downloadHandler.text);
                //Debug.Log($"GeoCountry : Weather: {weatherData.weather[0].main}, Temperature: {weatherData.main.temp}��C");
                ActivateImagesBasedOnWeatherAndTime(weatherData.weather[0].id);
            }
        }
    }

    IEnumerator RefreshWeatherAtSpecificTimes()
    {
        while (true)
        {
            // ���� �ð��� Ȯ��
            System.DateTime now = System.DateTime.Now;
            System.DateTime nextUpdateTime = now.AddHours(4 - now.Hour % 4).AddMinutes(-now.Minute).AddSeconds(-now.Second);

            // ���� ������Ʈ �ð����� ���
            yield return new WaitForSeconds((float)(nextUpdateTime - now).TotalSeconds);

            // ���� ������Ʈ �ð��� ���� ������ ���ΰ�ħ
            StartCoroutine(GetWeather(geoData.lat, geoData.lon));
        }
    }

    private void ActivateImagesBasedOnWeatherAndTime(int weatherId)
    {
        AllImagesHide(); // ��� �̹����� ����

        // ���� �ð� Ȯ��
        bool isDaytime = System.DateTime.Now.Hour >= 7 && System.DateTime.Now.Hour < 18;
        int index = -1; // Ȱ��ȭ�� �̹��� �ε���

        // ���� ���� �ڵ� ù ��° ���ڿ� ���� �ε��� ����
        if (weatherId == 800)
        {
            index = 5;
        }
        else
        {
            int firstDigit = weatherId / 100;
            switch (firstDigit)
            {
                case 2: index = 0; break;
                case 3: index = 1; break;
                case 5: index = 2; break;
                case 6: index = 3; break;
                case 7: index = 4; break;
                case 8: index = 6; break;
            }
        }

        // ������ �ε����� �������� ������ �̹��� Ȱ��ȭ
        if (index != -1)
        {
            if (isDaytime)
            {
                dayImages[index].gameObject.SetActive(true);
            }
            else
            {
                nightImages[index].gameObject.SetActive(true);
            }
        }
    }


    private void AllImagesHide()
    {
        for (int i = 0; i < dayImages.Count; i++)
        {
            dayImages[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < nightImages.Count; i++)
        {
            nightImages[i].gameObject.SetActive(false);
        }
    }
}