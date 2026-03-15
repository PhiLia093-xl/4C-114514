using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour
{
    public Button[] books = new Button[8];

    bool[] unlocked = new bool[8];
    bool[] finished = new bool[8];

    void Start()
    {
        LoadData();
        UpdateUnlock();
        RefreshUI();
    }

    void LoadData()
    {
        for (int i = 0; i < 8; i++)
        {
            finished[i] = PlayerPrefs.GetInt("BookFinished_" + i, 0) == 1;
        }
    }

    void UpdateUnlock()
    {
        unlocked[0] = true;

        if (finished[0])
            unlocked[1] = true;

        if (finished[1])
        {
            unlocked[2] = true;
            unlocked[3] = true;
        }

        if (finished[2] && finished[3])
        {
            unlocked[4] = true;
            unlocked[5] = true;
        }

        if (finished[4] && finished[5])
        {
            unlocked[6] = true;
            unlocked[7] = true;
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < books.Length; i++)
        {
            books[i].interactable = unlocked[i];
        }
    }

    public void OpenBook(int index)
    {
        if (!unlocked[index])
        {
            Debug.Log("这本书还没解锁");
            return;
        }

        PlayerPrefs.SetInt("ReadingBook", index);

        SceneManager.LoadScene("ReadScene");
    }
}