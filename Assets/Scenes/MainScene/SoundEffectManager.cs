using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    [Header("効果音のAudioClip配列")]
    public AudioClip[] soundEffects;
    
    [Header("AudioSourceコンポーネント")]
    public AudioSource audioSource;
    
    // シングルトンパターンでインスタンスを管理
    public static SoundEffectManager Instance { get; private set; }
    
    void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // AudioSourceが設定されていない場合は自動で取得
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    /// <summary>
    /// 指定されたインデックスの効果音を再生する
    /// </summary>
    /// <param name="soundIndex">効果音のインデックス</param>
    public void PlaySoundEffect(int soundIndex)
    {
        if (soundEffects == null || soundIndex < 0 || soundIndex >= soundEffects.Length)
        {
            Debug.LogWarning($"SoundManager: 無効なサウンドインデックス {soundIndex}");
            return;
        }
        
        if (soundEffects[soundIndex] == null)
        {
            Debug.LogWarning($"SoundManager: インデックス {soundIndex} のAudioClipがnullです");
            return;
        }
        
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundEffects[soundIndex]);
            Debug.Log($"SoundManager: 効果音 {soundIndex} を再生しました");
        }
        else
        {
            Debug.LogError("SoundManager: AudioSourceが設定されていません");
        }
    }
    
    /// <summary>
    /// 指定されたインデックスの効果音を指定された音量で再生する
    /// </summary>
    /// <param name="soundIndex">効果音のインデックス</param>
    /// <param name="volume">音量（0.0f - 1.0f）</param>
    public void PlaySoundEffect(int soundIndex, float volume)
    {
        if (soundEffects == null || soundIndex < 0 || soundIndex >= soundEffects.Length)
        {
            Debug.LogWarning($"SoundManager: 無効なサウンドインデックス {soundIndex}");
            return;
        }
        
        if (soundEffects[soundIndex] == null)
        {
            Debug.LogWarning($"SoundManager: インデックス {soundIndex} のAudioClipがnullです");
            return;
        }
        
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundEffects[soundIndex], volume);
            Debug.Log($"SoundManager: 効果音 {soundIndex} を音量 {volume} で再生しました");
        }
        else
        {
            Debug.LogError("SoundManager: AudioSourceが設定されていません");
        }
    }
    
    /// <summary>
    /// 指定された名前の効果音を再生する（AudioClipの名前で検索）
    /// </summary>
    /// <param name="soundName">効果音の名前</param>
    public void PlaySoundEffect(string soundName)
    {
        if (soundEffects == null)
        {
            Debug.LogWarning("SoundManager: soundEffects配列がnullです");
            return;
        }
        
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i] != null && soundEffects[i].name == soundName)
            {
                PlaySoundEffect(i);
                return;
            }
        }
        
        Debug.LogWarning($"SoundManager: 名前 '{soundName}' の効果音が見つかりませんでした");
    }
    
    /// <summary>
    /// 現在再生中の効果音を停止する
    /// </summary>
    public void StopSoundEffect()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Debug.Log("SoundManager: 効果音を停止しました");
        }
    }
    
    /// <summary>
    /// 効果音の音量を設定する
    /// </summary>
    /// <param name="volume">音量（0.0f - 1.0f）</param>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            Debug.Log($"SoundManager: 音量を {audioSource.volume} に設定しました");
        }
    }
    
    /// <summary>
    /// 現在の音量を取得する
    /// </summary>
    /// <returns>現在の音量</returns>
    public float GetVolume()
    {
        if (audioSource != null)
        {
            return audioSource.volume;
        }
        return 0f;
    }
}
