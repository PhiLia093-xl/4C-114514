using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour
{
    public Button[] books = new Button[6]; // ⭐改成6本

    private bool[] unlocked = new bool[6];
    private bool[] finished = new bool[6];

    void Start()
    {
        RefreshAll();
    }

    void RefreshAll()
    {
        LoadData();
        UpdateUnlockStatus();
        RefreshUI();
    }

    void LoadData()
    {
        for (int i = 0; i < books.Length; i++)
        {
            // ⭐是否“读过”（点击进入过）
            finished[i] = PlayerPrefs.GetInt("BookVisited_" + i, 0) == 1;
        }
    }

    void UpdateUnlockStatus()
    {
        // 初始化全部锁定
        for (int i = 0; i < unlocked.Length; i++)
            unlocked[i] = false;

        // ⭐第1本永远解锁
        unlocked[0] = true;

        // ⭐读完第1本 → 解锁第2本
        if (finished[0])
        {
            unlocked[1] = true;
        }

        // ⭐读完第2本 → 解锁第3、4本
        if (finished[1])
        {
            unlocked[2] = true;
            unlocked[3] = true;
        }

        // ⭐读完第3和第4本 → 解锁第5、6本
        if (finished[2] && finished[3])
        {
            unlocked[4] = true;
            unlocked[5] = true;
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < books.Length; i++)
        {
            books[i].interactable = unlocked[i];

            Image img = books[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = unlocked[i] ? Color.white : Color.gray;
            }
        }
    }

    public void OpenBook(int index)
    {
        if (index < 0 || index >= books.Length || !unlocked[index])
        {
            Debug.Log("这本书还没解锁或索引错误");
            return;
        }

        // ⭐关键：点击就算“读过”
        PlayerPrefs.SetInt("BookVisited_" + index, 1);

        PlayerPrefs.SetInt("ReadingBook", index);
        PlayerPrefs.Save();

        SceneManager.LoadScene("ReadScene");
    }

    public void ResetProgress()
    {
        for (int i = 0; i < books.Length; i++)
        {
            PlayerPrefs.DeleteKey("BookVisited_" + i);
        }

        PlayerPrefs.DeleteKey("ReadingBook");
        PlayerPrefs.Save();

        Debug.Log("进度已重置");

        RefreshAll();
    }
}