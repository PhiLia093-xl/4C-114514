using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadSceneManager : MonoBehaviour
{
    int currentBook;

    void Start()
    {
        currentBook = PlayerPrefs.GetInt("ReadingBook");
    }

    public void FinishReading()
    {
        PlayerPrefs.SetInt("BookFinished_" + currentBook, 1);

        SceneManager.LoadScene("BookSelectScene");
    }
}