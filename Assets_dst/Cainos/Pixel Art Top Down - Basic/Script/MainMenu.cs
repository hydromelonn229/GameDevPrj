using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Name of your gameplay scene
    public string gameplaySceneName = "Scene1";

    // AudioSource and AudioClip for button click
    public AudioSource buttonAudioSource;
    public AudioClip buttonClickClip;

    void Start()
    {
        // Main menu initialization (no automatic scene loading)
        // Scene1 will only load when the player clicks the Play button
    }

    // Call this from your Play button's OnClick event
    public void OnPlayButton()
    {
        StartCoroutine(PlaySoundAndLoadScene(gameplaySceneName));
    }

    // Call this from your Credits button's OnClick event
    public void OnCreditsButton()
    {
        StartCoroutine(PlaySoundAndLoadScene("Credits"));
    }

    private System.Collections.IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
            yield return new WaitForSeconds(buttonClickClip.length);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // Call this from your Quit button's OnClick event (optional)
    public void OnQuitButton()
    {
        StartCoroutine(PlaySoundAndQuit());
    }

    private System.Collections.IEnumerator PlaySoundAndQuit()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
            yield return new WaitForSeconds(buttonClickClip.length);
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}