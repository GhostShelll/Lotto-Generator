using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

using com.jbg.core.scene;

namespace com.jbg.core.manager
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        private void Awake()
        {
            DebugEx.Log("GAME::AWAKE");

            Game.Instance = this;

#if LOG_DEBUG
            System.Text.StringBuilder pathLog = new();
            pathLog.AppendLine();
            pathLog.Append("Application.dataPath\t\t: ").AppendLine(Application.dataPath);
            pathLog.Append("Application.streamingAssetsPath\t: ").AppendLine(Application.streamingAssetsPath);
            pathLog.Append("Application.persistentDataPath\t: ").AppendLine(Application.persistentDataPath);
            pathLog.Append("Application.temporaryCachePath\t: ").Append(Application.temporaryCachePath);
            DebugEx.LogColor(pathLog, "#F699CD");
#endif  // LOG_DEBUG

            GameObject.DontDestroyOnLoad(this.gameObject);

            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }

        private void Start()
        {
            SystemManager.Open();

            SceneExManager.OpenScene(SceneExManager.SceneType.Main);   // ù��° ���� Ÿ��Ʋ ����
        }

        private void OnDestroy()
        {
            SystemManager.Close();

            Game.Instance = null;
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
