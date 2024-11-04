using SadUtils;

namespace Game
{
    public class ExternalCoroutineExecutionManager : Singleton<ExternalCoroutineExecutionManager>
    {
        protected override void Awake()
        {
            SetInstance(this);
        }
    }
}
