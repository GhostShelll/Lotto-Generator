using com.jbg.content.popup;
using com.jbg.core.scene;

namespace com.jbg.content.scene
{
    public class MainScene : SceneEx
    {
        // TODO[jbg] : �ʿ��� �͸� ����ؾ���

        protected override void OnOpen()
        {
            base.OnOpen();

            LottoPopupAssist.Open(() =>
            {

            });
        }
    }
}
