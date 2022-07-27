using UnityEngine;

using com.jbg.asset;
using com.jbg.asset.control;
using com.jbg.asset.data;
using com.jbg.content.popup;
using com.jbg.content.scene.view;
using com.jbg.core.manager;
using com.jbg.core.scene;

namespace com.jbg.content.scene
{
    public class MainScene : SceneEx
    {
        private MainView sceneView;

        public enum STATE
        {
            CheckAsset,
            DownloadAsset,
            WaitDone,
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            this.sceneView = (MainView)this.SceneView;

            this.sceneView.BindEvent(MainView.Event.LottoSelect, this.OnClickLottoSelect);
            this.sceneView.BindEvent(MainView.Event.RefreshAsset, this.OnClickRefreshAsset);

            MainView.Params p = new();
            p.lottoBtnTxt = LocaleControl.GetString(LocaleCodes.MAIN_SCENE_LOTTO_BTN_TEXT);
            p.checkAssetTxt = LocaleControl.GetString(LocaleCodes.MAIN_SCENE_ASSET_CHECKING_TEXT);
            p.downloadAssetTxt = LocaleControl.GetString(LocaleCodes.MAIN_SCENE_ASSET_LOADING_TEXT);
            p.refreshBtnTxt = LocaleControl.GetString(LocaleCodes.MAIN_SCENE_ASSET_LOADING_BTN_TEXT);

            this.sceneView.OnOpen(p);

            this.SetStateCheckAsset();
        }

        protected override void OnClose()
        {
            base.OnClose();

            this.sceneView.RemoveEvent(MainView.Event.LottoSelect);
            this.sceneView.RemoveEvent(MainView.Event.RefreshAsset);
        }

        private void SetStateCheckAsset()
        {
            this.SetState((int)STATE.CheckAsset);

            this.sceneView.SetStateCheckAsset();

            // ���� üũ ����
            Coroutine task = CoroutineManager.AddTask(TableVersionControl.CheckAsset());

            this.AddUpdateFunc(() =>
            {
                if (TableVersionControl.CheckDone)
                {
                    // ���� üũ �Ϸ���
                    if (task != null)
                        CoroutineManager.RemoveTask(task);

                    this.SetStateWaitDone();    // TODO[jbg]
                }
                else
                {
                    // ���� üũ ��
                    this.sceneView.UpdateCheckAsset();
                }
            });
        }

        private void SetStateDownloadAsset()
        {
            this.SetState((int)STATE.DownloadAsset);

            // ���� �ε� ����
            Coroutine task = CoroutineManager.AddTask(AssetManager.LoadAsync());

            this.AddUpdateFunc(() =>
            {
                if (AssetManager.LoadingDone)
                {
                    // ���� �ε� �Ϸ���
                    if (task != null)
                        CoroutineManager.RemoveTask(task);

                    this.SetStateWaitDone();
                }
                else
                {
                    // ���� �ε���
                    string currentAsset = AssetManager.CurrentAsset;
                    float currentProgress = AssetManager.CurrentProgress;

                    this.sceneView.UpdateDownloadAsset(currentAsset, currentProgress);
                }
            });
        }

        private void SetStateWaitDone()
        {
            this.SetState((int)STATE.WaitDone);

            this.sceneView.SetStateWaitDone();
        }

        private void OnClickLottoSelect(int eventNum, object obj)
        {
            SoundManager.Inst.Play(SoundManager.SOUND_YES);

            LottoPopupAssist.Open(() =>
            {

            });
        }

        private void OnClickRefreshAsset(int eventNum, object obj)
        {
            SoundManager.Inst.Play(SoundManager.SOUND_YES);

            // ���� ���� ����
            this.SetStateCheckAsset();
        }
    }
}
