using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace com.jbg.core.manager
{
    public class Game : MonoBehaviour
    {
        // Singleton
        private Game() { }
        private static readonly System.Lazy<Game> instance = new(() => new());
        public static Game Instance { get { return instance.Value; } }

        private void Awake()
        {
            DebugEx.Log("GAME::AWAKE");

            System.Text.StringBuilder pathLog = new();
            pathLog.AppendLine();
            pathLog.Append("Application.dataPath\t\t: ").AppendLine(Application.dataPath);
            pathLog.Append("Application.streamingAssetsPath\t: ").AppendLine(Application.streamingAssetsPath);
            pathLog.Append("Application.persistentDataPath\t: ").AppendLine(Application.persistentDataPath);
            pathLog.Append("Application.temporaryCachePath\t: ").Append(Application.temporaryCachePath);
            DebugEx.LogColor(pathLog, "#F699CD");

            GameObject.DontDestroyOnLoad(this.gameObject);

            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }

        private void Start()
        {
            SystemManager.Open();
            // TODO[jbg] : ù��° �� ����
        }

        private void OnDestroy()
        {
            SystemManager.Close();
        }

        private void Update()
        {
            SystemManager.Update();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                SystemManager.OnApplicationPause();
            else
                SystemManager.OnApplicationResume();
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            // �ش� �Լ��� �ڵ� �����. ���� �� ���� �ε�.
            if (SceneManager.GetActiveScene().name.CompareTo("Game") != 0)
                SceneManager.LoadScene("Game");
        }
#endif  // UNITY_EDITOR
    }
}