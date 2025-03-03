using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public AudioSource titleMusic;
	public AudioSource[] bgMusic;
	private int currentTrack;

	private bool isPaused;

	public AudioSource[] sfx;

	private void Start()
	{
		currentTrack = -1;
	}

	private void Update()
	{
		if (isPaused == false && currentTrack >= 0 && currentTrack < bgMusic.Length)
		{
			if (bgMusic[currentTrack].isPlaying == false)
			{
				PlayNextBGM();
			}
		}
	}

	public void StopMusic()
	{
		foreach (AudioSource track in bgMusic)
		{
			track.Stop();
		}

		titleMusic.Stop();
	}

	public void PlayTitle()
	{
		StopMusic();

		titleMusic.Play();
	}

	public void PlayNextBGM()
	{
		StopMusic();

		currentTrack++;

		if (currentTrack >= bgMusic.Length)
		{
			currentTrack = 0;
		}

		bgMusic[currentTrack].Play();
	}

	public void PauseMusic()
	{
		isPaused = true;

		bgMusic[currentTrack].Pause();
	}

	public void ResumeMusic()
	{
		isPaused = false;

		bgMusic[currentTrack].Play();
	}

	public void PlaySFX(int sfxToPlay)
	{
		sfx[sfxToPlay].Stop();
		sfx[sfxToPlay].Play();
	}

	public void PlaySFXPitchAdjusted(int sfxToPlay)
	{
		sfx[sfxToPlay].pitch = Random.Range(.8f, 1.2f);

		PlaySFX(sfxToPlay);
	}

	public void PlayTitleMusic()
	{
		titleMusic.Play();
	}

	public void StopAllMusic()
	{
		// Dừng tất cả AudioSource đang phát nhạc nền
		foreach (AudioSource track in bgMusic)
		{
			track.Stop();
		}

		// Dừng nhạc title
		if (titleMusic != null)
		{
			titleMusic.Stop();
		}
	}

	// Thêm vào AudioManager.cs
	private void OnEnable()
	{
		// Đăng ký sự kiện khi scene được tải
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		// Hủy đăng ký khi không cần thiết nữa
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Tự động chọn nhạc phù hợp cho mỗi scene
		StopAllMusic();

		switch (scene.name)
		{
			case "MainMenu":
				PlayTitle();
				break;
			case "StoryInfo":
				PlayTitleMusic(); // hoặc nhạc riêng cho story
				break;
			case "Main": // Game chính
				PlayNextBGM();
				break;
				// Thêm các scene khác nếu cần
		}
	}
	public void SetMusicVolume(float volume)
	{
		foreach (AudioSource track in bgMusic)
		{
			track.volume = volume;
		}
		titleMusic.volume = volume;
	}

	public void SetSFXVolume(float volume)
	{
		foreach (AudioSource sound in sfx)
		{
			sound.volume = volume;
		}
	}
}
