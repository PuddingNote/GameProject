using SlimeExperiment.Data;

namespace SlimeExperiment.Core
{
    public sealed class ScreenController
    {
        public ScreenState CurrentScreenState { get; private set; } = ScreenState.MainMenu;

        public void Show(ScreenState screenState)
        {
            CurrentScreenState = screenState;
        }
    }
}
