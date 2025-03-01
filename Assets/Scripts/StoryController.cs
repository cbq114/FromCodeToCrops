using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI storyText;
    public Button continueButton;
    public Button skipButton;
    public Image fadePanel;

    [Header("Story Settings")]
    public float typingSpeed = 0.05f;
    public float delayBetweenPages = 1.0f;
    public string nextSceneName = "Main"; // Tên scene trò chơi chính

    [TextArea(5, 10)]
    public string[] storyPages;

    private int currentPage = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    [Header("Background Images")]
    public Image[] backgroundImages;
    public float imageTransitionSpeed = 1.0f;
    [Header("Audio")]
    public AudioClip[] pageSounds;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private void Start()
    {
        // Ẩn nút Continue ban đầu
        continueButton.gameObject.SetActive(false);

        continueButton.transform.SetAsLastSibling();
        skipButton.transform.SetAsLastSibling();

        // Gán các sự kiện cho nút
        continueButton.onClick.AddListener(NextPage);
        skipButton.onClick.AddListener(SkipToGame);

        // Bắt đầu hiển thị trang đầu tiên
        ShowCurrentPage();

        // Thêm hai dòng này để hiển thị hình nền và âm thanh ban đầu
        TransitionBackground(0);
        PlayPageAudio(0);

        // Phát nhạc nếu có
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayTitleMusic();
        }
    }

    private void ShowCurrentPage()
    {
        if (currentPage < storyPages.Length)
        {
            isTyping = true;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(storyPages[currentPage]));
        }
        else
        {
            StartCoroutine(FadeAndLoadGame());
        }
    }

    private IEnumerator TypeText(string text)
    {
        storyText.text = "";
        continueButton.gameObject.SetActive(false);

        bool isTag = false;
        bool isItalic = false;
        bool isBold = false;

        foreach (char c in text)
        {
            // Xử lý các tag đặc biệt
            if (c == '<')
                isTag = true;
            else if (c == '>')
                isTag = false;

            // Chỉ thêm chữ và phát âm thanh nếu không phải tag
            if (!isTag)
            {
                storyText.text += c;

                // Phát âm thanh đánh máy
                if (audioSource != null && typingSound != null && c != ' ' && !char.IsPunctuation(c))
                {
                    audioSource.PlayOneShot(typingSound, Random.Range(0.1f, 0.2f));
                }

                // Tạm dừng lâu hơn sau câu
                if (c == '.' || c == '!' || c == '?')
                    yield return new WaitForSeconds(typingSpeed * 8);
                else if (c == ',')
                    yield return new WaitForSeconds(typingSpeed * 4);
                else
                    yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
        continueButton.gameObject.SetActive(true);
    }

    public void NextPage()
    {
        if (isTyping)
        {
            // Nếu đang đánh chữ, hiển thị toàn bộ văn bản ngay lập tức
            StopCoroutine(typingCoroutine);
            storyText.text = storyPages[currentPage];
            isTyping = false;
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            currentPage++;

            // Hiệu ứng fade giữa các trang
            StartCoroutine(FadeTextOut(() =>
            {
                ShowCurrentPage();
                PlayPageAudio(currentPage);
                TransitionBackground(currentPage);
            }));
        }
    }
    private IEnumerator FadeTextOut(System.Action onComplete)
    {
        float fadeTime = 0.5f;
        float startAlpha = storyText.color.a;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, time / fadeTime);
            storyText.color = new Color(storyText.color.r, storyText.color.g, storyText.color.b, alpha);
            yield return null;
        }

        if (onComplete != null)
            onComplete();

        time = 0;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, startAlpha, time / fadeTime);
            storyText.color = new Color(storyText.color.r, storyText.color.g, storyText.color.b, alpha);
            yield return null;
        }
    }
    private void SkipToGame()
    {
        StartCoroutine(FadeAndLoadGame());
    }

    private IEnumerator FadeAndLoadGame()
    {
        // Kiểm tra null trước khi sử dụng
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float alpha = 0;

            while (alpha < 1)
            {
                alpha += Time.deltaTime;
                fadePanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
        }
        else
        {
            // Nếu không có fadePanel, đợi một khoảng thời gian ngắn
            yield return new WaitForSeconds(0.5f);
            Debug.LogWarning("fadePanel không được gán trong Inspector");
        }

        // Vẫn chuyển scene ngay cả khi không có fadePanel
        SceneManager.LoadScene(nextSceneName);
    }
private void TransitionBackground(int pageIndex)
{
    StartCoroutine(FadeBackground(pageIndex));
    
    // Đặt thứ tự đúng sau khi chuyển hình nền
    foreach (Image img in backgroundImages)
    {
        // Đặt hình nền xuống dưới cùng trong hierarchy
        img.transform.SetAsFirstSibling();
    }
    
    // Đảm bảo text và nút luôn ở trên cùng
    storyText.transform.SetAsLastSibling();
    continueButton.transform.SetAsLastSibling();
    skipButton.transform.SetAsLastSibling();
}

    private IEnumerator FadeBackground(int index)
    {
        // Ẩn tất cả hình nền
        foreach (Image img in backgroundImages)
        {
            img.CrossFadeAlpha(0, 0, true);
        }

        // Hiện hình nền mới
        if (index < backgroundImages.Length)
        {
            backgroundImages[index].gameObject.SetActive(true);
            backgroundImages[index].CrossFadeAlpha(1, imageTransitionSpeed, true);
        }

        yield return null;
    }
    private void PlayPageAudio(int pageIndex)
    {
        if (audioSource != null && pageIndex < pageSounds.Length && pageSounds[pageIndex] != null)
        {
            audioSource.Stop();
            audioSource.clip = pageSounds[pageIndex];
            audioSource.Play();
        }
    }
}