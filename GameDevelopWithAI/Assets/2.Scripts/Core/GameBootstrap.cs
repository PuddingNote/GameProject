using SlimeExperiment.Battle;
using SlimeExperiment.Data;
using SlimeExperiment.Logbook;
using SlimeExperiment.UI;
using UnityEngine;

namespace SlimeExperiment.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 60;

            PrototypeContentLibrary contentLibrary = new PrototypeContentLibrary();
            PrototypeSpriteCatalog spriteCatalog = Resources.Load<PrototypeSpriteCatalog>("PrototypeSpriteCatalog");
            SystemRandomSource randomSource = new SystemRandomSource();
            GameSession gameSession = new GameSession();
            ScreenController screenController = new ScreenController();
            DraftService draftService = new DraftService(contentLibrary, randomSource);
            MonsterFactory monsterFactory = new MonsterFactory(contentLibrary, randomSource);
            BattleSimulator battleSimulator = new BattleSimulator(randomSource);
            RelicPoolService relicPoolService = new RelicPoolService(contentLibrary);
            ExperimentLogService experimentLogService = new ExperimentLogService(contentLibrary);
            RunProgressController runProgressController = new RunProgressController(
                gameSession,
                monsterFactory,
                battleSimulator,
                relicPoolService,
                experimentLogService,
                randomSource);

            PrototypeGameView view = gameObject.GetComponent<PrototypeGameView>();
            if (view == null)
            {
                view = gameObject.AddComponent<PrototypeGameView>();
            }

            view.Initialize(
                spriteCatalog,
                contentLibrary,
                gameSession,
                screenController,
                draftService,
                monsterFactory,
                runProgressController,
                experimentLogService);
        }
    }
}
