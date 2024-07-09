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
    public float lat; // 위도
    public float lon; // 경도
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
    private string apiKey = "bb41774b64c4d57b0a4b24489e46cd69"; // 여기에 OpenWeatherMap API 키를 입력하세요.

    public List<Image> dayImages = new List<Image>();
    public List<Image> nightImages = new List<Image>();
    public GameObject objWeather;
    public Coroutine weatherCoroutine = null; // 현재 실행 중인 코루틴을 추적

    private GeoData geoData; // 사용자의 지리적 데이터를 저장하는 변수

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
                    StartCoroutine(GetWeather(geoData.lat, geoData.lon)); // 날씨 정보를 처음으로 불러옴
                    StartCoroutine(RefreshWeatherAtSpecificTimes()); // 지정된 시간에 날씨 정보를 새로고침
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
                //Debug.Log($"GeoCountry : Weather: {weatherData.weather[0].main}, Temperature: {weatherData.main.temp}°C");
                ActivateImagesBasedOnWeatherAndTime(weatherData.weather[0].id);
            }
        }
    }

    IEnumerator RefreshWeatherAtSpecificTimes()
    {
        while (true)
        {
            // 현재 시간을 확인
            System.DateTime now = System.DateTime.Now;
            System.DateTime nextUpdateTime = now.AddHours(4 - now.Hour % 4).AddMinutes(-now.Minute).AddSeconds(-now.Second);

            // 다음 업데이트 시간까지 대기
            yield return new WaitForSeconds((float)(nextUpdateTime - now).TotalSeconds);

            // 다음 업데이트 시간에 날씨 정보를 새로고침
            StartCoroutine(GetWeather(geoData.lat, geoData.lon));
        }
    }

    private void ActivateImagesBasedOnWeatherAndTime(int weatherId)
    {
        AllImagesHide(); // 모든 이미지를 숨김

        // 현재 시간 확인
        bool isDaytime = System.DateTime.Now.Hour >= 7 && System.DateTime.Now.Hour < 18;
        int index = -1; // 활성화할 이미지 인덱스

        // 날씨 상태 코드 첫 번째 숫자에 따라 인덱스 결정
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

        // 결정된 인덱스를 바탕으로 적절한 이미지 활성화
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