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
            AssetLoad,
            WaitDone,
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            this.sceneView = (MainView)this.SceneView;

            this.sceneView.BindEvent(MainView.Event.LottoSelect, this.OnClickLottoSelect);
            this.sceneView.BindEvent(MainView.Event.RefreshAsset, this.OnClickRefreshAsset);

            MainView.Params p = new();
            p.lottoBtnTxt = LocaleControl.GetString(LocaleCodes.LOTTO_POPUP_TITLE_TEXT);
            p.progressTxt = "@@{0} ���� �ٿ�ε� ������";
            p.refreshBtnTxt = "@@���� ����";

            this.sceneView.OnOpen(p);

            this.SetStateAssetLoad();
        }

        protected override void OnClose()
        {
            base.OnClose();

            this.sceneView.RemoveEvent(MainView.Event.LottoSelect);
            this.sceneView.RemoveEvent(MainView.Event.RefreshAsset);
        }

        private void SetStateAssetLoad()
        {
            this.SetState((int)STATE.AssetLoad);

            this.sceneView.SetStateAssetLoad();

            // TODO[jbg] : ���� �ε� ����

            this.AddUpdateFunc(() =>
            {
                // TODO[jbg] : ���� �ε� �Ϸ� ��
                //this.SetStateWaitDone();
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
            this.SetStateAssetLoad();
        }
    }
}
