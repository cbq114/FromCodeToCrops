using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GeminiAPIClient : MonoBehaviour
{
	public static GeminiAPIClient instance;

	[SerializeField] private string apiKey;
	private readonly HttpClient client = new HttpClient();
	private const string API_URL = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";

	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public async Task<string> SendRequest(string prompt)
	{
		try
		{
			string requestBody = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{EscapeJsonString(prompt)}\"}}]}}]}}";

			var request = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}?key={apiKey}");
			request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();

			string responseContent = await response.Content.ReadAsStringAsync();
			return ExtractTextFromGeminiResponse(responseContent);
		}
		catch (Exception e)
		{
			Debug.LogError($"Error with Gemini API: {e.Message}");
			return "Không thể kết nối với API.";
		}
	}

	private string ExtractTextFromGeminiResponse(string jsonResponse)
	{
		try
		{
			GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(jsonResponse);
			if (response != null && response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
			{
				return response.candidates[0].content.parts[0].text;
			}
			return "Không có phản hồi từ API.";
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to parse Gemini response: {e.Message}");
			return "Không thể phân tích phản hồi.";
		}
	}

	private string EscapeJsonString(string text)
	{
		return text.Replace("\"", "\\\"").Replace("\n", "\\n");
	}

	public async Task<string> GetWeatherDescription(string weatherType, string season, int day)
	{
		string prompt = $"Provide a short weather description for a farming game: Weather: {weatherType}, Season: {season}, Day: {day}. (1 sentence)";
		return await SendRequest(prompt);
	}
}

// Định nghĩa các lớp để ánh xạ JSON
[System.Serializable]
public class GeminiResponse
{
	public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
{
	public Content content;
}

[System.Serializable]
public class Content
{
	public Part[] parts;
}

[System.Serializable]
public class Part
{
	public string text;
}