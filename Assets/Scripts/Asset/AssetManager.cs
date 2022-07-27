using System.Collections;
using System.Collections.Generic;

using com.jbg.asset.control;
using com.jbg.core.manager;

namespace com.jbg.asset
{
    using Manager = AssetManager;

    public class AssetManager
    {
        public static bool IsOpened { get; private set; }

        public static bool LoadingDone { get; private set; }
        public static string CurrentAsset { get; private set; }
        public static float CurrentProgress { get; private set; }

        private const string CLASSNAME = "AssetManager";

        public static void Open()
        {
            Manager.Close();
            Manager.IsOpened = true;

            SystemManager.AddOpenList(CLASSNAME);

            Manager.LoadingDone = false;
            Manager.CurrentAsset = string.Empty;
            Manager.CurrentProgress = 0f;

            // ���� ���� ����
            TableVersionControl.Open();
            LocaleControl.Open();
            LottoResultControl.Open();
        }

        public static IEnumerator LoadAsync()
        {
            // �ε� ���� ����
            Manager.LoadingDone = false;

            // �ε��� �͵� ��� �����
            Dictionary<string, IEnumerator> loadList = new();
            loadList.Add(LocaleControl.TABLENAME, LocaleControl.LoadAsync());
            loadList.Add(LottoResultControl.TABLENAME, LottoResultControl.LoadAsync());

            float currentCount = 0f;
            float totalCount = loadList.Count;

            Dictionary<string, IEnumerator>.Enumerator enumerator = loadList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string assetName = enumerator.Current.Key;
                IEnumerator task = enumerator.Current.Value;

                Manager.CurrentAsset = assetName;
                Manager.CurrentProgress = currentCount / totalCount;

                yield return task;

                currentCount++;
            }

            Manager.CurrentAsset = string.Empty;
            Manager.CurrentProgress = currentCount / totalCount;

            Manager.LoadingDone = true;

            yield break;
        }

        public static void Close()
        {
            if (Manager.IsOpened)
            {
                Manager.IsOpened = false;

                SystemManager.RemoveOpenList(CLASSNAME);

                // ���� ���� Ŭ����
                TableVersionControl.Close();
                LocaleControl.Close();
                LottoResultControl.Close();
            }
        }
    }
}
