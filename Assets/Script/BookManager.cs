using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour
{
    public Button[] books = new Button[8];
    private bool[] unlocked = new bool[8];
    private bool[] finished = new bool[8];

    void Start()
    {
        RefreshAll();
    }

    // 封装刷新逻辑，方便初始化和重置时调用
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
            // 读取每本书的完成状态 (0: 未完成, 1: 已完成)
            finished[i] = PlayerPrefs.GetInt("BookFinished_" + i, 0) == 1;
        }
    }

    void UpdateUnlockStatus()
    {
        // 第一本书默认永远解锁
        unlocked[0] = true;

        // 从第二本书开始循环：如果前一本书完成了，则当前书解锁
        for (int i = 1; i < books.Length; i++)
        {
            if (finished[i - 1])
            {
                unlocked[i] = true;
            }
            else
            {
                unlocked[i] = false;
            }
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < books.Length; i++)
        {
            // 设置按钮是否可点击
            books[i].interactable = unlocked[i];

            // 进阶：你可以根据解锁状态改变颜色或透明度
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

        PlayerPrefs.SetInt("ReadingBook", index);
        SceneManager.LoadScene("ReadScene");
    }

    // --- 新增功能：重置按钮调用此方法 ---
    public void ResetProgress()
    {
        // 删除所有相关的 PlayerPrefs 键
        for (int i = 0; i < books.Length; i++)
        {
            PlayerPrefs.DeleteKey("BookFinished_" + i);
        }

        PlayerPrefs.DeleteKey("ReadingBook");
        PlayerPrefs.Save(); // 强制保存更改

        Debug.Log("进度已重置");

        // 重新刷新数据和UI
        RefreshAll();
    }
}