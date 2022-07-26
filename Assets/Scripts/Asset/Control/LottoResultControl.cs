using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GoogleSheetsToUnity;

using com.jbg.asset.data;
using com.jbg.core;
using com.jbg.core.manager;

namespace com.jbg.asset.control
{
    using Control = LottoResultControl;

    public class LottoResultControl
    {
        public static bool IsOpened { get; private set; }
        public static bool LoadingDone { get; private set; }
        public static int RecentPeriod { get { return Control.assetData.Count; } }        // ���� �ֱ� ������ ȸ��

        private static Dictionary<int, LottoResultData> assetData = new();
        private static Dictionary<int, List<int>> lottoNumberMap = new();       // ��÷ ���� �� �ζǹ�ȣ ���� Ƚ��

        private const string CLASSNAME = "LottoResultControl";
        public const string ASSOCIATED_SHEET_NAME = "LottoResultData";
        private const string START_CELL = "A1";
        private const string END_CELL = "H1500";

        public static void Open()
        {
            Control.Close();
            Control.IsOpened = true;

            SystemManager.AddOpenList(CLASSNAME);

            Control.LoadingDone = false;

            Control.assetData = new();
            Control.lottoNumberMap = new();
        }

        public static IEnumerator LoadAsync()
        {
            Control.LoadingDone = false;

            // �ε� ���� ����
            SpreadsheetManager.Read(new GSTU_Search(AssetManager.ASSOCIATED_SHEET, Control.ASSOCIATED_SHEET_NAME, Control.START_CELL, Control.END_CELL), (spreadSheet) =>
            {
                Dictionary<int, List<GSTU_Cell>>.Enumerator enumerator = spreadSheet.rows.primaryDictionary.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int rowNum = enumerator.Current.Key;
                    if (rowNum == 1)        // 1�� ���� '���� ����' ���̱� ������ ����
                        continue;

                    List<GSTU_Cell> cellList = enumerator.Current.Value;        // code, num1, num2, num3, num4, num5, num6, bonus ������ ���ĵ�

                    bool codeIsCorrect = int.TryParse(cellList[0].value, out int code);
                    if (codeIsCorrect == false)
                    {
                        DebugEx.LogColor(string.Format("[LOTTO CHECK] {0}��° ���� Code �� int.Parse()�� �����߽��ϴ�.", rowNum), "red");
                        continue;
                    }

                    if (cellList.Count != 8)
                    {
                        DebugEx.LogColor(string.Format("[LOTTO CHECK] Code {0}�� �� ������ ���� �ʽ��ϴ�.", code), "red");
                        continue;
                    }

                    bool numberCheckFail = false;
                    for (int i = 0; i < cellList.Count; i++)
                    {
                        GSTU_Cell cellInfo = cellList[i];

                        bool isNum = int.TryParse(cellInfo.value, out int num);
                        if (isNum == false)
                        {
                            numberCheckFail = true;

                            DebugEx.LogColor(string.Format("[LOTTO CHECK] Code {0}�� ��ȣ�� �߿��� ���ڰ� �ƴ� ���� �ֽ��ϴ�. ������ : {1}", code, cellInfo.value), "red");
                            continue;
                        }
                    }

                    if (numberCheckFail)
                        continue;

                    if (Control.assetData.ContainsKey(code) == false)
                    {
                        Control.assetData.Add(code, new LottoResultData()
                        {
                            code = code,
                            num1 = int.Parse(cellList[1].value),
                            num2 = int.Parse(cellList[2].value),
                            num3 = int.Parse(cellList[3].value),
                            num4 = int.Parse(cellList[4].value),
                            num5 = int.Parse(cellList[5].value),
                            num6 = int.Parse(cellList[6].value),
                            bonus = int.Parse(cellList[7].value),
                        });
                    }
                }

                // ��÷ ���� �� ��ȣ ���� Ƚ���� 0���� �ʱ�ȭ
                Control.lottoNumberMap.Clear();
                Control.lottoNumberMap.Add(1, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(2, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(3, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(4, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(5, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(6, Enumerable.Repeat(0, 45).ToList());
                Control.lottoNumberMap.Add(7, Enumerable.Repeat(0, 45).ToList());

                Dictionary<int, LottoResultData>.Enumerator enumerator2 = Control.assetData.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    int code = enumerator2.Current.Key;
                    LottoResultData data = enumerator2.Current.Value;

                    Control.lottoNumberMap[1][data.num1 - 1]++;
                    Control.lottoNumberMap[2][data.num2 - 1]++;
                    Control.lottoNumberMap[3][data.num3 - 1]++;
                    Control.lottoNumberMap[4][data.num4 - 1]++;
                    Control.lottoNumberMap[5][data.num5 - 1]++;
                    Control.lottoNumberMap[6][data.num6 - 1]++;
                    Control.lottoNumberMap[7][data.bonus - 1]++;
                }

                Control.LoadingDone = true;
            });

            while (Control.LoadingDone == false)
                yield return null;

            yield break;
        }

        public static void Close()
        {
            if (Control.IsOpened)
            {
                Control.IsOpened = false;

                if (Control.assetData != null)
                    Control.assetData.Clear();
                Control.assetData = null;

                if (Control.lottoNumberMap != null)
                    Control.lottoNumberMap.Clear();
                Control.lottoNumberMap = null;

                SystemManager.RemoveOpenList(CLASSNAME);
            }
        }

        public static List<int> GetLottoNumbers(int choiceNum)
        {
            if (Control.lottoNumberMap.ContainsKey(choiceNum) == false)
            {
                DebugEx.LogColor(string.Format("�ζ� ��÷ ���� �߿��� {0}��° ������ �����ϴ�.", choiceNum), "red");
                return null;
            }

            return Control.lottoNumberMap[choiceNum].ToList();
        }
    }
}
