using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;  // 用于控制音频播放的 AudioSource 组件
    public Slider volumeSlider;      // 控制音量的滑动条

    void Start()
    {
        // 设置 Slider 的初始值为当前音量
        volumeSlider.value = audioSource.volume;

        // 添加监听事件，当 Slider 的值变化时，调用 AdjustVolume 方法
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
    }

    // 调整音量的方法
    void AdjustVolume(float volume)
    {
        audioSource.volume = volume;  // 设置 AudioSource 的音量为 Slider 的值
    }

}