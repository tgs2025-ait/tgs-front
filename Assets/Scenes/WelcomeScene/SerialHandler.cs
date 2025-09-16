using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.IO;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    //ポート名
    //例
    //MacBookで有線接続: /dev/cu.usbserial-795292008B
    //MacBookで無線接続: /dev/cu.orca-m5stick-c-plus-dev 
    public string portName = "/dev/cu.orca-m5stick-c-plus-dev";
    public int baudRate = 115200;
    public bool skipIfPortMissing = true; // デバッグ時: ポート未検出なら接続処理をスキップ
    [Header("シリアル詳細設定")]
    public string newLine = "\n"; // デバイスが\rのみの場合は"\r"に変更
    public bool dtrEnable = false;  // 一部デバイスで必要
    public bool rtsEnable = false;  // 一部デバイスで必要

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    void Awake()
    {
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_) {
            OnDataReceived(message_);
        }
        isNewMessageReceived_ = false;
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        try {
            // macOS 等で /dev/* の仮想シリアルに対し、デバイス未接続時はスキップ
            if (skipIfPortMissing && !string.IsNullOrEmpty(portName))
            {
                // 物理ポートの存在チェック（/dev/* の場合）
                if (portName.StartsWith("/dev/") && !File.Exists(portName))
                {
                    Debug.LogWarning($"SerialHandler: ポートが見つかりませんでした（{portName}）。スキップします。");
                    return;
                }
            }

            serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            //または
            //serialPort_ = new SerialPort(portName, baudRate);
            // ブロッキング防止のためタイムアウト設定
            serialPort_.ReadTimeout = 100;   // ms
            serialPort_.WriteTimeout = 100;  // ms
            serialPort_.NewLine = newLine;
            serialPort_.DtrEnable = dtrEnable;
            serialPort_.RtsEnable = rtsEnable;
            serialPort_.Open();

            isRunning_ = true;

            thread_ = new Thread(Read);
            thread_.Start();
        } catch (System.Exception e) {
            Debug.LogWarning("SerialHandler: ポートを開けませんでした: " + e.Message);
            isRunning_ = false;
            serialPort_ = null;
        }
    }

    private void Close()
    {
        isNewMessageReceived_ = false;
        isRunning_ = false;
        // 先にポートを閉じて Read のブロックを解除
        try
        {
            if (serialPort_ != null)
            {
                if (serialPort_.IsOpen)
                {
                    serialPort_.Close();
                }
                serialPort_.Dispose();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("SerialHandler: クローズ時に例外: " + e.Message);
        }

        if (thread_ != null && thread_.IsAlive) {
            // 完了待ち（タイムアウト付き）
            if (!thread_.Join(200))
            {
                try { thread_.Interrupt(); } catch { }
            }
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
            try {
                // タイムアウト付きで行単位読み取り
                message_ = serialPort_.ReadLine();
                Debug.Log($"SerialHandler ReadLine: '{message_}'");
                isNewMessageReceived_ = true;
            }
            catch (System.TimeoutException)
            {
                // 無通信時はスルーしてループ継続（非ブロッキング）
            }
            catch (ThreadInterruptedException)
            {
                break;
            }
            catch (System.Exception e) {
                Debug.LogWarning(e.Message);
                // 予期せぬ例外時は少し待って継続
                Thread.Sleep(10);
            }
        }
    }

    public void Write(string message)
    {
        try {
            if (serialPort_ != null && serialPort_.IsOpen)
            {
                serialPort_.Write(message);
            }
        } catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }
    }
}
