using System.Collections;
using System.Collections.Generic;
using System.Text;
using SlimeExperiment.Battle;
using SlimeExperiment.Core;
using SlimeExperiment.Data;
using SlimeExperiment.Logbook;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SlimeExperiment.UI
{
    public sealed class PrototypeGameView : MonoBehaviour
    {
        private readonly List<CardDefinition> currentChoices = new List<CardDefinition>();
        private readonly List<Image> relicSlotImages = new List<Image>();
        private readonly List<Image> relicSlotIconImages = new List<Image>();
        private readonly List<RectTransform> relicSlotRects = new List<RectTransform>();
        private readonly List<RectTransform> relicSlotIconRects = new List<RectTransform>();
        private readonly List<Text> relicSlotTexts = new List<Text>();
        private readonly List<Button> relicSlotButtons = new List<Button>();
        private readonly List<Button> codexEntryButtons = new List<Button>();
        private readonly List<Image> codexEntryImages = new List<Image>();
        private readonly List<Text> codexEntryTexts = new List<Text>();
        private readonly List<Button> logEntryButtons = new List<Button>();
        private readonly List<Text> logEntryTitleTexts = new List<Text>();
        private readonly List<Text> logEntryResultTexts = new List<Text>();
        private readonly List<ExperimentRecord> logEntryRecords = new List<ExperimentRecord>();
        private readonly List<Button> logDetailRelicButtons = new List<Button>();
        private readonly List<Image> logDetailRelicImages = new List<Image>();
        private readonly List<Text> logDetailRelicTexts = new List<Text>();

        private PrototypeSpriteCatalog spriteCatalog;
        private PrototypeContentLibrary contentLibrary;
        private GameSession gameSession;
        private ScreenController screenController;
        private DraftService draftService;
        private MonsterFactory monsterFactory;
        private RunProgressController runProgressController;
        private ExperimentLogService experimentLogService;

        private Font defaultFont;
        private RectTransform canvasRootRect;
        private GameObject menuScreen;
        private GameObject logbookScreen;
        private GameObject logbookDetailOverlayRoot;
        private GameObject codexScreen;
        private GameObject runScreen;
        private GameObject choiceRowContainer;
        private GameObject actionButtonContainer;
        private GameObject relicPanelRoot;
        private GameObject statePanelRoot;
        private GameObject enemyInfoPanelRoot;
        private GameObject detailOverlayRoot;
        private GameObject rewardOverlayRoot;
        private CanvasGroup draftOverlayCanvasGroup;
        private CanvasGroup detailOverlayCanvasGroup;
        private CanvasGroup rewardOverlayCanvasGroup;
        private RectTransform draftOverlayCardsRowRect;
        private RectTransform rewardOverlayCardRect;
        private Text detailOverlayTitleText;
        private Text detailOverlayNameText;
        private Image rewardOverlayFrameImage;
        private Image rewardOverlayIconImage;
        private Image detailOverlayRelicImage;
        private GameObject detailOverlayRelicRoot;
        private Text rewardOverlayLabelText;
        private Image[] optionButtonImages;
        private Button primaryButton;
        private Text primaryButtonText;
        private Button logbookMenuButton;
        private Text logbookMenuButtonText;
        private Button codexMenuButton;
        private Text codexMenuButtonText;
        private Button logbookBackButton;
        private Text logbookBackButtonText;
        private Button codexBackButton;
        private Text codexBackButtonText;
        private Button actionButton;
        private Text actionButtonText;
        private Button stateLogButton;
        private Text stateLogButtonText;
        private Button detailOverlayCloseButton;
        private Text detailOverlayCloseButtonText;
        private Button resultTouchOverlayButton;
        private Text resultTouchOverlayText;
        private Text headerText;
        private Text subHeaderText;
        private Text headerStageBadgeText;
        private RectTransform playerActorRootRect;
        private RectTransform enemyActorRootRect;
        private RectTransform playerActorVisualRootRect;
        private RectTransform enemyActorVisualRootRect;
        private Image playerPortraitImage;
        private Image playerHandImage;
        private Image enemyPortraitImage;
        private Image playerHealthFillImage;
        private Image enemyHealthFillImage;
        private Text playerPortraitText;
        private Text enemyPortraitText;
        private Text playerHealthValueText;
        private Text enemyHealthValueText;
        private Text playerInfoText;
        private Text monsterInfoText;
        private Text centerStateText;
        private Text selectedCardsText;
        private Text relicsText;
        private Text stateInfoText;
        private Text logbookEmptyText;
        private Text logDetailTitleText;
        private Text logDetailSummaryText;
        private Text logDetailCardsText;
        private Button[] optionButtons;
        private Text[] optionButtonTexts;
        private bool isInitialized;
        private bool isSelectionLocked;
        private int hiddenRewardSlotIndex = -1;
        private int displayedPlayerHealth;
        private int displayedEnemyHealth;
        private ExperimentRecord activeLogDetailRecord;
        private bool isDetailOverlayShowingRelic;
        private string currentDetailLogTitle = "전투 로그";
        private string currentDetailLogContent = "아직 전투 로그가 없습니다.";

        public void Initialize(
            PrototypeSpriteCatalog spriteCatalog,
            PrototypeContentLibrary contentLibrary,
            GameSession gameSession,
            ScreenController screenController,
            DraftService draftService,
            MonsterFactory monsterFactory,
            RunProgressController runProgressController,
            ExperimentLogService experimentLogService)
        {
            if (isInitialized)
            {
                return;
            }

            this.spriteCatalog = spriteCatalog;
            this.contentLibrary = contentLibrary;
            this.gameSession = gameSession;
            this.screenController = screenController;
            this.draftService = draftService;
            this.monsterFactory = monsterFactory;
            this.runProgressController = runProgressController;
            this.experimentLogService = experimentLogService;
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            BuildUi();
            ShowMainMenu();
            isInitialized = true;
        }

        private void BuildUi()
        {
            EnsureEventSystem();

            Transform existingCanvas = transform.Find("PrototypeCanvas");
            if (existingCanvas != null)
            {
                Destroy(existingCanvas.gameObject);
            }

            GameObject canvasObject = new GameObject("PrototypeCanvas");
            canvasObject.transform.SetParent(transform, false);
            canvasRootRect = canvasObject.AddComponent<RectTransform>();

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            Image background = canvasObject.AddComponent<Image>();
            background.color = new Color32(15, 19, 29, 255);

            RectTransform safeArea = CreatePanel("SafeArea", canvasObject.transform, new Color32(0, 0, 0, 0));
            safeArea.gameObject.AddComponent<SafeAreaFitter>();

            RectTransform framePanel = CreatePanel("FramePanel", safeArea, new Color32(28, 34, 48, 255));
            Stretch(framePanel, 28f);
            AddOutline(framePanel.gameObject, new Color32(233, 240, 255, 65), new Vector2(4f, -4f));

            VerticalLayoutGroup frameLayout = framePanel.gameObject.AddComponent<VerticalLayoutGroup>();
            frameLayout.padding = new RectOffset(28, 28, 28, 28);
            frameLayout.spacing = 18f;
            frameLayout.childControlWidth = true;
            frameLayout.childControlHeight = true;
            frameLayout.childForceExpandWidth = true;
            frameLayout.childForceExpandHeight = false;

            RectTransform headerPanel = CreatePanel("HeaderPanel", framePanel, new Color32(40, 48, 67, 255));
            LayoutElement headerLayout = headerPanel.gameObject.AddComponent<LayoutElement>();
            headerLayout.preferredHeight = 150f;
            AddOutline(headerPanel.gameObject, new Color32(255, 255, 255, 18), new Vector2(2f, -2f));

            headerText = CreateText("HeaderText", headerPanel, 46, TextAnchor.UpperLeft);
            headerText.fontStyle = FontStyle.Bold;
            headerText.rectTransform.anchorMin = new Vector2(0f, 0.38f);
            headerText.rectTransform.anchorMax = new Vector2(1f, 1f);
            headerText.rectTransform.offsetMin = new Vector2(24f, -4f);
            headerText.rectTransform.offsetMax = new Vector2(-24f, -12f);

            subHeaderText = CreateText("SubHeaderText", headerPanel, 24, TextAnchor.LowerLeft);
            subHeaderText.color = new Color32(197, 208, 229, 255);
            subHeaderText.rectTransform.anchorMin = new Vector2(0f, 0f);
            subHeaderText.rectTransform.anchorMax = new Vector2(1f, 0.45f);
            subHeaderText.rectTransform.offsetMin = new Vector2(24f, 12f);
            subHeaderText.rectTransform.offsetMax = new Vector2(-24f, 0f);

            RectTransform contentRoot = CreatePanel("ContentRoot", framePanel, new Color32(0, 0, 0, 0));
            LayoutElement contentLayout = contentRoot.gameObject.AddComponent<LayoutElement>();
            contentLayout.flexibleHeight = 1f;

            menuScreen = BuildMenuScreen(contentRoot);
            logbookScreen = BuildLogbookScreen(contentRoot);
            codexScreen = BuildCodexScreen(contentRoot);
            runScreen = BuildRunScreen(contentRoot);
            BuildDetailOverlay(canvasObject.transform);
            BuildRewardOverlay(canvasObject.transform);

            RectTransform resultOverlayRect = CreateButton("ResultTouchOverlay", canvasObject.transform, out resultTouchOverlayButton, out resultTouchOverlayText);
            Stretch(resultOverlayRect, 0f);
            Image overlayImage = resultTouchOverlayButton.GetComponent<Image>();
            overlayImage.color = new Color(0f, 0f, 0f, 0.58f);
            resultTouchOverlayText.fontSize = 34;
            resultTouchOverlayText.alignment = TextAnchor.MiddleCenter;
            resultTouchOverlayButton.onClick.AddListener(ShowMainMenu);
            resultTouchOverlayButton.gameObject.SetActive(false);
        }

        private GameObject BuildMenuScreen(Transform parent)
        {
            RectTransform screenRoot = CreatePanel("MenuScreen", parent, new Color32(0, 0, 0, 0));
            Stretch(screenRoot, 0f);

            VerticalLayoutGroup layout = screenRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 30f;
            layout.padding = new RectOffset(0, 0, 90, 90);

            CreateSpacer(screenRoot, 180f);

            RectTransform gridWrapper = CreatePanel("MenuGridWrapper", screenRoot, new Color32(0, 0, 0, 0));
            LayoutElement gridWrapperLayout = gridWrapper.gameObject.AddComponent<LayoutElement>();
            gridWrapperLayout.preferredHeight = 780f;

            GridLayoutGroup grid = gridWrapper.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(260f, 260f);
            grid.spacing = new Vector2(38f, 38f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.padding = new RectOffset(180, 180, 40, 40);

            RectTransform logbookTile = CreateButton("LogbookMenuButton", gridWrapper, out logbookMenuButton, out logbookMenuButtonText);
            Image logbookTileImage = logbookMenuButton.GetComponent<Image>();
            logbookTileImage.color = new Color32(56, 64, 84, 255);
            SetButtonColors(logbookMenuButton, new Color32(56, 64, 84, 255), new Color32(74, 84, 109, 255), new Color32(44, 52, 72, 255));
            AddOutline(logbookTile.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));
            logbookMenuButtonText.text = "실험 로그";
            logbookMenuButtonText.fontSize = 34;
            logbookMenuButtonText.fontStyle = FontStyle.Bold;
            logbookMenuButtonText.color = Color.white;
            logbookMenuButton.onClick.AddListener(ShowLogbookScreen);

            RectTransform codexTile = CreateButton("CodexMenuButton", gridWrapper, out codexMenuButton, out codexMenuButtonText);
            Image codexTileImage = codexMenuButton.GetComponent<Image>();
            codexTileImage.color = new Color32(56, 64, 84, 255);
            SetButtonColors(codexMenuButton, new Color32(56, 64, 84, 255), new Color32(74, 84, 109, 255), new Color32(44, 52, 72, 255));
            AddOutline(codexTile.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));
            codexMenuButtonText.text = "도감";
            codexMenuButtonText.fontSize = 34;
            codexMenuButtonText.fontStyle = FontStyle.Bold;
            codexMenuButtonText.color = Color.white;
            codexMenuButton.onClick.AddListener(ShowCodexScreen);

            CreateMenuTile(gridWrapper, "MenuTileLab", "연구실", false, new Color32(56, 64, 84, 255));

            RectTransform startTile = CreateButton("PrimaryButton", gridWrapper, out primaryButton, out primaryButtonText);
            SetButtonColors(primaryButton, new Color32(90, 153, 255, 255), new Color32(118, 176, 255, 255), new Color32(60, 121, 229, 255));
            AddOutline(startTile.gameObject, new Color32(255, 255, 255, 35), new Vector2(4f, -4f));
            primaryButtonText.fontSize = 40;
            primaryButtonText.fontStyle = FontStyle.Bold;
            primaryButton.onClick.AddListener(StartExperiment);

            CreateSpacer(screenRoot, 0f).gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            return screenRoot.gameObject;
        }

        private GameObject BuildLogbookScreen(Transform parent)
        {
            RectTransform screenRoot = CreatePanel("LogbookScreen", parent, new Color32(0, 0, 0, 0));
            Stretch(screenRoot, 0f);
            screenRoot.gameObject.SetActive(false);

            VerticalLayoutGroup layout = screenRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 18f;

            RectTransform scrollRoot = CreatePanel("LogbookScrollRoot", screenRoot, new Color32(30, 37, 52, 252));
            LayoutElement scrollLayout = scrollRoot.gameObject.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;
            AddOutline(scrollRoot.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 36f;

            RectTransform viewport = CreatePanel("LogbookViewport", scrollRoot, new Color32(0, 0, 0, 0));
            Stretch(viewport, 8f);
            Mask viewportMask = viewport.gameObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            viewport.GetComponent<Image>().color = new Color32(0, 0, 0, 6);
            scrollRect.viewport = viewport;

            RectTransform content = CreatePanel("LogbookContent", viewport, new Color32(0, 0, 0, 0));
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = new Vector2(24f, 0f);
            content.offsetMax = new Vector2(-24f, 0f);
            VerticalLayoutGroup contentLayout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = -36f;
            contentLayout.padding = new RectOffset(0, 0, 20, 40);
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            scrollRect.content = content;

            for (int index = 0; index < 120; index++)
            {
                RectTransform itemRoot = CreatePanel($"LogbookEntryRoot{index}", content, new Color32(0, 0, 0, 0));
                LayoutElement itemLayout = itemRoot.gameObject.AddComponent<LayoutElement>();
                itemLayout.preferredHeight = 180f;

                RectTransform shadowRect = CreatePanel($"LogbookEntryShadow{index}", itemRoot, new Color32(0, 0, 0, 255));
                ApplyLogbookEntryOffset(shadowRect, index, 46f);

                RectTransform entryRect = CreateButton($"LogbookEntryButton{index}", itemRoot, out Button entryButton, out Text entryTitleText);
                ApplyLogbookEntryOffset(entryRect, index, 0f);
                SetButtonColors(entryButton, new Color32(232, 42, 42, 255), new Color32(242, 73, 73, 255), new Color32(201, 31, 31, 255));
                AddOutline(entryRect.gameObject, new Color32(0, 0, 0, 30), new Vector2(2f, -2f));
                int capturedIndex = index;
                entryButton.onClick.AddListener(() => OnLogEntrySelected(capturedIndex));

                entryTitleText.fontSize = 34;
                entryTitleText.fontStyle = FontStyle.Bold;
                entryTitleText.alignment = TextAnchor.MiddleLeft;
                entryTitleText.rectTransform.anchorMin = new Vector2(0f, 0f);
                entryTitleText.rectTransform.anchorMax = new Vector2(0.66f, 1f);
                entryTitleText.rectTransform.offsetMin = new Vector2(28f, 0f);
                entryTitleText.rectTransform.offsetMax = new Vector2(-10f, 0f);
                entryTitleText.raycastTarget = false;

                Text resultText = CreateText($"LogbookEntryResultText{index}", entryRect, 34, TextAnchor.MiddleRight);
                resultText.fontStyle = FontStyle.Bold;
                resultText.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                resultText.rectTransform.anchorMax = new Vector2(1f, 1f);
                resultText.rectTransform.offsetMin = new Vector2(10f, 0f);
                resultText.rectTransform.offsetMax = new Vector2(-28f, 0f);
                resultText.raycastTarget = false;

                logEntryButtons.Add(entryButton);
                logEntryTitleTexts.Add(entryTitleText);
                logEntryResultTexts.Add(resultText);
            }

            logbookEmptyText = CreateText("LogbookEmptyText", viewport, 34, TextAnchor.MiddleCenter);
            logbookEmptyText.fontStyle = FontStyle.Bold;
            logbookEmptyText.text = "아직 기록된 실험이 없습니다.";
            Stretch(logbookEmptyText.rectTransform, 60f);
            logbookEmptyText.raycastTarget = false;

            RectTransform backButtonRect = CreateButton("LogbookBackButton", screenRoot, out logbookBackButton, out logbookBackButtonText);
            LayoutElement backLayout = backButtonRect.gameObject.AddComponent<LayoutElement>();
            backLayout.preferredHeight = 120f;
            logbookBackButtonText.text = "메인으로";
            logbookBackButtonText.fontSize = 34;
            logbookBackButtonText.fontStyle = FontStyle.Bold;
            logbookBackButton.onClick.AddListener(ShowMainMenu);

            RectTransform detailOverlay = CreatePanel("LogbookDetailOverlay", screenRoot, new Color(0f, 0f, 0f, 0.72f));
            logbookDetailOverlayRoot = detailOverlay.gameObject;
            LayoutElement detailOverlayLayout = detailOverlay.gameObject.AddComponent<LayoutElement>();
            detailOverlayLayout.ignoreLayout = true;
            Stretch(detailOverlay, 0f);
            Button detailOverlayCloseButton = detailOverlay.gameObject.AddComponent<Button>();
            SetButtonColors(detailOverlayCloseButton, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0f));
            detailOverlayCloseButton.onClick.AddListener(CloseLogDetailOverlay);

            RectTransform detailPanel = CreatePanel("LogbookDetailPanel", detailOverlay, new Color32(39, 47, 67, 255));
            detailPanel.anchorMin = new Vector2(0.06f, 0.12f);
            detailPanel.anchorMax = new Vector2(0.94f, 0.88f);
            detailPanel.offsetMin = Vector2.zero;
            detailPanel.offsetMax = Vector2.zero;
            AddOutline(detailPanel.gameObject, new Color32(255, 255, 255, 20), new Vector2(3f, -3f));
            detailPanel.GetComponent<Image>().raycastTarget = false;

            logDetailTitleText = CreateText("LogDetailTitleText", detailPanel, 42, TextAnchor.UpperLeft);
            logDetailTitleText.fontStyle = FontStyle.Bold;
            logDetailTitleText.rectTransform.anchorMin = new Vector2(0f, 0.88f);
            logDetailTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            logDetailTitleText.rectTransform.offsetMin = new Vector2(28f, 10f);
            logDetailTitleText.rectTransform.offsetMax = new Vector2(-28f, -10f);
            logDetailTitleText.raycastTarget = false;

            logDetailSummaryText = CreateText("LogDetailSummaryText", detailPanel, 28, TextAnchor.UpperLeft);
            logDetailSummaryText.rectTransform.anchorMin = new Vector2(0f, 0.66f);
            logDetailSummaryText.rectTransform.anchorMax = new Vector2(1f, 0.86f);
            logDetailSummaryText.rectTransform.offsetMin = new Vector2(28f, 0f);
            logDetailSummaryText.rectTransform.offsetMax = new Vector2(-28f, 0f);
            logDetailSummaryText.raycastTarget = false;

            RectTransform cardsPanel = CreatePanel("LogDetailCardsPanel", detailPanel, new Color32(49, 57, 79, 255));
            cardsPanel.anchorMin = new Vector2(0.05f, 0.35f);
            cardsPanel.anchorMax = new Vector2(0.95f, 0.64f);
            cardsPanel.offsetMin = Vector2.zero;
            cardsPanel.offsetMax = Vector2.zero;
            AddOutline(cardsPanel.gameObject, new Color32(255, 255, 255, 14), new Vector2(2f, -2f));
            cardsPanel.GetComponent<Image>().raycastTarget = false;

            Text cardsTitleText = CreateText("LogDetailCardsTitleText", cardsPanel, 28, TextAnchor.UpperLeft);
            cardsTitleText.fontStyle = FontStyle.Bold;
            cardsTitleText.text = "선택한 카드 능력";
            cardsTitleText.rectTransform.anchorMin = new Vector2(0f, 0.76f);
            cardsTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            cardsTitleText.rectTransform.offsetMin = new Vector2(20f, 8f);
            cardsTitleText.rectTransform.offsetMax = new Vector2(-20f, -8f);
            cardsTitleText.raycastTarget = false;

            logDetailCardsText = CreateText("LogDetailCardsText", cardsPanel, 24, TextAnchor.UpperLeft);
            logDetailCardsText.rectTransform.anchorMin = new Vector2(0f, 0f);
            logDetailCardsText.rectTransform.anchorMax = new Vector2(1f, 0.8f);
            logDetailCardsText.rectTransform.offsetMin = new Vector2(20f, 18f);
            logDetailCardsText.rectTransform.offsetMax = new Vector2(-20f, -6f);
            logDetailCardsText.raycastTarget = false;

            RectTransform relicPanel = CreatePanel("LogDetailRelicPanel", detailPanel, new Color32(49, 57, 79, 255));
            relicPanel.anchorMin = new Vector2(0.05f, 0.08f);
            relicPanel.anchorMax = new Vector2(0.95f, 0.31f);
            relicPanel.offsetMin = Vector2.zero;
            relicPanel.offsetMax = Vector2.zero;
            AddOutline(relicPanel.gameObject, new Color32(255, 255, 255, 14), new Vector2(2f, -2f));
            relicPanel.GetComponent<Image>().raycastTarget = false;

            Text relicTitleText = CreateText("LogDetailRelicTitleText", relicPanel, 28, TextAnchor.UpperLeft);
            relicTitleText.fontStyle = FontStyle.Bold;
            relicTitleText.text = "획득한 유물";
            relicTitleText.rectTransform.anchorMin = new Vector2(0f, 0.74f);
            relicTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            relicTitleText.rectTransform.offsetMin = new Vector2(20f, 6f);
            relicTitleText.rectTransform.offsetMax = new Vector2(-20f, -8f);
            relicTitleText.raycastTarget = false;

            RectTransform relicGrid = CreatePanel("LogDetailRelicGrid", relicPanel, new Color32(0, 0, 0, 0));
            relicGrid.anchorMin = new Vector2(0f, 0f);
            relicGrid.anchorMax = new Vector2(1f, 0.78f);
            relicGrid.offsetMin = new Vector2(18f, 12f);
            relicGrid.offsetMax = new Vector2(-18f, -6f);
            GridLayoutGroup relicGridLayout = relicGrid.gameObject.AddComponent<GridLayoutGroup>();
            relicGridLayout.cellSize = new Vector2(112f, 112f);
            relicGridLayout.spacing = new Vector2(14f, 14f);
            relicGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            relicGridLayout.constraintCount = 5;
            relicGridLayout.childAlignment = TextAnchor.MiddleLeft;

            for (int index = 0; index < 10; index++)
            {
                RectTransform relicButtonRect = CreateButton($"LogDetailRelicButton{index}", relicGrid, out Button relicButton, out Text relicLabel);
                Image relicButtonImage = relicButton.GetComponent<Image>();
                relicButtonImage.color = new Color32(57, 69, 94, 255);
                SetButtonColors(relicButton, new Color32(57, 69, 94, 255), new Color32(74, 87, 114, 255), new Color32(44, 56, 76, 255));
                AddOutline(relicButtonRect.gameObject, new Color32(255, 255, 255, 16), new Vector2(2f, -2f));
                int capturedIndex = index;
                relicButton.onClick.AddListener(() => OnLogDetailRelicSelected(capturedIndex));

                Image relicImage = CreatePanel($"LogDetailRelicImage{index}", relicButtonRect, new Color32(0, 0, 0, 0)).GetComponent<Image>();
                relicImage.rectTransform.anchorMin = new Vector2(0.18f, 0.18f);
                relicImage.rectTransform.anchorMax = new Vector2(0.82f, 0.82f);
                relicImage.rectTransform.offsetMin = Vector2.zero;
                relicImage.rectTransform.offsetMax = Vector2.zero;
                relicImage.preserveAspect = true;
                relicImage.raycastTarget = false;

                relicLabel.fontSize = 18;
                relicLabel.fontStyle = FontStyle.Bold;
                relicLabel.alignment = TextAnchor.MiddleCenter;
                relicLabel.resizeTextForBestFit = true;
                relicLabel.resizeTextMinSize = 10;
                relicLabel.resizeTextMaxSize = 18;
                Stretch(relicLabel.rectTransform, 6f);
                relicLabel.raycastTarget = false;

                logDetailRelicButtons.Add(relicButton);
                logDetailRelicImages.Add(relicImage);
                logDetailRelicTexts.Add(relicLabel);
            }

            logbookDetailOverlayRoot.SetActive(false);
            return screenRoot.gameObject;
        }

        private GameObject BuildCodexScreen(Transform parent)
        {
            RectTransform screenRoot = CreatePanel("CodexScreen", parent, new Color32(0, 0, 0, 0));
            Stretch(screenRoot, 0f);
            screenRoot.gameObject.SetActive(false);

            VerticalLayoutGroup layout = screenRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 18f;

            RectTransform scrollRoot = CreatePanel("CodexScrollRoot", screenRoot, new Color32(30, 37, 52, 252));
            LayoutElement scrollLayout = scrollRoot.gameObject.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;
            AddOutline(scrollRoot.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 40f;

            RectTransform viewport = CreatePanel("CodexViewport", scrollRoot, new Color32(0, 0, 0, 0));
            Stretch(viewport, 8f);
            Mask viewportMask = viewport.gameObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            viewport.GetComponent<Image>().color = new Color32(0, 0, 0, 6);
            scrollRect.viewport = viewport;

            RectTransform content = CreatePanel("CodexContent", viewport, new Color32(0, 0, 0, 0));
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = new Vector2(18f, 0f);
            content.offsetMax = new Vector2(-18f, 0f);
            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(288f, 352f);
            grid.spacing = new Vector2(20f, 20f);
            grid.padding = new RectOffset(8, 8, 10, 10);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            scrollRect.content = content;

            IReadOnlyList<RelicDefinition> relics = contentLibrary.Relics;
            for (int index = 0; index < relics.Count; index++)
            {
                RectTransform entry = CreateButton($"CodexEntryButton{index}", content, out Button entryButton, out Text entryLabel);
                Image entryImage = entryButton.GetComponent<Image>();
                entryImage.color = new Color32(39, 47, 67, 255);
                SetButtonColors(entryButton, new Color32(39, 47, 67, 255), new Color32(54, 65, 89, 255), new Color32(29, 36, 51, 255));
                AddOutline(entry.gameObject, new Color32(255, 255, 255, 18), new Vector2(2f, -2f));
                int capturedIndex = index;
                entryButton.onClick.AddListener(() => OnCodexRelicSelected(capturedIndex));

                RectTransform iconRoot = CreatePanel($"CodexEntryIcon{index}", entry, new Color32(25, 31, 44, 255));
                iconRoot.anchorMin = new Vector2(0.1f, 0.34f);
                iconRoot.anchorMax = new Vector2(0.9f, 0.92f);
                iconRoot.offsetMin = Vector2.zero;
                iconRoot.offsetMax = Vector2.zero;
                AddOutline(iconRoot.gameObject, new Color32(255, 255, 255, 10), new Vector2(2f, -2f));
                iconRoot.GetComponent<Image>().raycastTarget = false;

                Image iconImage = CreatePanel($"CodexEntryIconImage{index}", iconRoot, new Color32(0, 0, 0, 0)).GetComponent<Image>();
                iconImage.rectTransform.anchorMin = new Vector2(0.12f, 0.12f);
                iconImage.rectTransform.anchorMax = new Vector2(0.88f, 0.88f);
                iconImage.rectTransform.offsetMin = Vector2.zero;
                iconImage.rectTransform.offsetMax = Vector2.zero;
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;

                entryLabel.fontSize = 36;
                entryLabel.fontStyle = FontStyle.Bold;
                entryLabel.alignment = TextAnchor.UpperCenter;
                entryLabel.color = Color.white;
                entryLabel.resizeTextForBestFit = true;
                entryLabel.resizeTextMinSize = 20;
                entryLabel.resizeTextMaxSize = 36;
                entryLabel.rectTransform.anchorMin = new Vector2(0.08f, 0.08f);
                entryLabel.rectTransform.anchorMax = new Vector2(0.92f, 0.3f);
                entryLabel.rectTransform.offsetMin = Vector2.zero;
                entryLabel.rectTransform.offsetMax = Vector2.zero;
                entryLabel.raycastTarget = false;

                codexEntryButtons.Add(entryButton);
                codexEntryImages.Add(iconImage);
                codexEntryTexts.Add(entryLabel);
            }

            RectTransform backButtonRect = CreateButton("CodexBackButton", screenRoot, out codexBackButton, out codexBackButtonText);
            LayoutElement backLayout = backButtonRect.gameObject.AddComponent<LayoutElement>();
            backLayout.preferredHeight = 120f;
            codexBackButtonText.text = "메인으로";
            codexBackButtonText.fontSize = 48;
            codexBackButtonText.fontStyle = FontStyle.Bold;
            codexBackButton.onClick.AddListener(ShowMainMenu);

            return screenRoot.gameObject;
        }

        private GameObject BuildRunScreen(Transform parent)
        {
            RectTransform screenRoot = CreatePanel("RunScreen", parent, new Color32(0, 0, 0, 0));
            Stretch(screenRoot, 0f);

            VerticalLayoutGroup layout = screenRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 18f;

            RectTransform topStagePanel = CreatePanel("TopStagePanel", screenRoot, new Color32(34, 41, 58, 255));
            LayoutElement topStageLayout = topStagePanel.gameObject.AddComponent<LayoutElement>();
            topStageLayout.preferredHeight = 560f;
            AddOutline(topStagePanel.gameObject, new Color32(255, 255, 255, 20), new Vector2(3f, -3f));

            RectTransform battleBackdrop = CreatePanel("BattleBackdrop", topStagePanel, new Color32(20, 24, 35, 255));
            Stretch(battleBackdrop, 0f);

            RectTransform battleFloor = CreatePanel("BattleFloor", topStagePanel, new Color32(38, 33, 44, 255));
            battleFloor.anchorMin = new Vector2(0f, 0f);
            battleFloor.anchorMax = new Vector2(1f, 0.24f);
            battleFloor.offsetMin = Vector2.zero;
            battleFloor.offsetMax = Vector2.zero;

            headerStageBadgeText = CreateText("StageBadgeText", topStagePanel, 30, TextAnchor.UpperCenter);
            headerStageBadgeText.fontStyle = FontStyle.Bold;
            headerStageBadgeText.color = new Color32(255, 217, 120, 255);
            headerStageBadgeText.rectTransform.anchorMin = new Vector2(0f, 0.88f);
            headerStageBadgeText.rectTransform.anchorMax = new Vector2(1f, 1f);
            headerStageBadgeText.rectTransform.offsetMin = new Vector2(16f, 0f);
            headerStageBadgeText.rectTransform.offsetMax = new Vector2(-16f, -18f);

            centerStateText = CreateText("CenterStateText", topStagePanel, 28, TextAnchor.MiddleCenter);
            centerStateText.color = new Color32(232, 237, 246, 255);
            centerStateText.rectTransform.anchorMin = new Vector2(0.24f, 0.65f);
            centerStateText.rectTransform.anchorMax = new Vector2(0.76f, 0.84f);
            centerStateText.rectTransform.offsetMin = new Vector2(10f, 0f);
            centerStateText.rectTransform.offsetMax = new Vector2(-10f, 0f);

            playerActorRootRect = CreatePanel("PlayerActorRoot", topStagePanel, new Color32(0, 0, 0, 0));
            playerActorRootRect.anchorMin = new Vector2(0.02f, 0.12f);
            playerActorRootRect.anchorMax = new Vector2(0.44f, 0.86f);
            playerActorRootRect.offsetMin = new Vector2(12f, 0f);
            playerActorRootRect.offsetMax = new Vector2(-10f, 0f);

            playerActorVisualRootRect = CreatePanel("PlayerActorVisualRoot", playerActorRootRect, new Color32(0, 0, 0, 0));
            playerActorVisualRootRect.anchorMin = new Vector2(0f, 0f);
            playerActorVisualRootRect.anchorMax = new Vector2(1f, 1f);
            playerActorVisualRootRect.offsetMin = Vector2.zero;
            playerActorVisualRootRect.offsetMax = Vector2.zero;

            playerHandImage = CreatePanel("PlayerHandImage", playerActorVisualRootRect, new Color32(0, 0, 0, 0)).GetComponent<Image>();
            playerHandImage.rectTransform.anchorMin = new Vector2(0.34f, 0.22f);
            playerHandImage.rectTransform.anchorMax = new Vector2(0.82f, 0.62f);
            playerHandImage.rectTransform.offsetMin = Vector2.zero;
            playerHandImage.rectTransform.offsetMax = Vector2.zero;
            playerHandImage.preserveAspect = true;

            playerPortraitImage = CreatePanel("PlayerPortraitImage", playerActorVisualRootRect, new Color32(0, 0, 0, 0)).GetComponent<Image>();
            playerPortraitImage.rectTransform.anchorMin = new Vector2(0.06f, 0.24f);
            playerPortraitImage.rectTransform.anchorMax = new Vector2(0.58f, 0.76f);
            playerPortraitImage.rectTransform.offsetMin = Vector2.zero;
            playerPortraitImage.rectTransform.offsetMax = Vector2.zero;
            playerPortraitImage.preserveAspect = true;

            playerPortraitText = CreateText("PlayerPortraitText", playerActorRootRect, 24, TextAnchor.MiddleLeft);
            playerPortraitText.fontStyle = FontStyle.Bold;
            playerPortraitText.rectTransform.anchorMin = new Vector2(0.02f, 0.8f);
            playerPortraitText.rectTransform.anchorMax = new Vector2(0.42f, 0.92f);
            playerPortraitText.rectTransform.offsetMin = Vector2.zero;
            playerPortraitText.rectTransform.offsetMax = Vector2.zero;

            RectTransform playerHealthBar = CreateHealthBar("PlayerHealthBar", topStagePanel, TextAnchor.MiddleLeft, out playerHealthFillImage, out playerHealthValueText);
            playerHealthBar.anchorMin = new Vector2(0.06f, 0.06f);
            playerHealthBar.anchorMax = new Vector2(0.44f, 0.14f);
            playerHealthBar.offsetMin = Vector2.zero;
            playerHealthBar.offsetMax = Vector2.zero;

            enemyActorRootRect = CreatePanel("EnemyActorRoot", topStagePanel, new Color32(0, 0, 0, 0));
            enemyActorRootRect.anchorMin = new Vector2(0.56f, 0.12f);
            enemyActorRootRect.anchorMax = new Vector2(0.98f, 0.86f);
            enemyActorRootRect.offsetMin = new Vector2(10f, 0f);
            enemyActorRootRect.offsetMax = new Vector2(-12f, 0f);

            enemyActorVisualRootRect = CreatePanel("EnemyActorVisualRoot", enemyActorRootRect, new Color32(0, 0, 0, 0));
            enemyActorVisualRootRect.anchorMin = new Vector2(0f, 0f);
            enemyActorVisualRootRect.anchorMax = new Vector2(1f, 1f);
            enemyActorVisualRootRect.offsetMin = Vector2.zero;
            enemyActorVisualRootRect.offsetMax = Vector2.zero;

            enemyPortraitImage = CreatePanel("EnemyPortraitImage", enemyActorVisualRootRect, new Color32(0, 0, 0, 0)).GetComponent<Image>();
            enemyPortraitImage.rectTransform.anchorMin = new Vector2(0.38f, 0.2f);
            enemyPortraitImage.rectTransform.anchorMax = new Vector2(0.86f, 0.78f);
            enemyPortraitImage.rectTransform.offsetMin = Vector2.zero;
            enemyPortraitImage.rectTransform.offsetMax = Vector2.zero;
            enemyPortraitImage.preserveAspect = true;

            enemyPortraitText = CreateText("EnemyPortraitText", enemyActorRootRect, 24, TextAnchor.MiddleRight);
            enemyPortraitText.fontStyle = FontStyle.Bold;
            enemyPortraitText.rectTransform.anchorMin = new Vector2(0.56f, 0.8f);
            enemyPortraitText.rectTransform.anchorMax = new Vector2(0.98f, 0.92f);
            enemyPortraitText.rectTransform.offsetMin = Vector2.zero;
            enemyPortraitText.rectTransform.offsetMax = Vector2.zero;

            RectTransform enemyHealthBar = CreateHealthBar("EnemyHealthBar", topStagePanel, TextAnchor.MiddleRight, out enemyHealthFillImage, out enemyHealthValueText);
            enemyHealthBar.anchorMin = new Vector2(0.56f, 0.06f);
            enemyHealthBar.anchorMax = new Vector2(0.94f, 0.14f);
            enemyHealthBar.offsetMin = Vector2.zero;
            enemyHealthBar.offsetMax = Vector2.zero;

            playerInfoText = CreateText("HiddenPlayerInfoText", topStagePanel, 1, TextAnchor.UpperLeft);
            playerInfoText.gameObject.SetActive(false);

            RectTransform lowerPanel = CreatePanel("LowerPanel", screenRoot, new Color32(0, 0, 0, 0));
            LayoutElement lowerPanelLayout = lowerPanel.gameObject.AddComponent<LayoutElement>();
            lowerPanelLayout.flexibleHeight = 1f;

            HorizontalLayoutGroup lowerGroup = lowerPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            lowerGroup.spacing = 20f;
            lowerGroup.childControlWidth = true;
            lowerGroup.childControlHeight = true;
            lowerGroup.childForceExpandWidth = false;
            lowerGroup.childForceExpandHeight = true;

            RectTransform selectedPanel = CreatePanel("SelectedPanel", lowerPanel, new Color32(37, 44, 61, 255));
            LayoutElement selectedPanelLayout = selectedPanel.gameObject.AddComponent<LayoutElement>();
            selectedPanelLayout.preferredWidth = 470f;
            selectedPanelLayout.minWidth = 430f;
            AddOutline(selectedPanel.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            Text selectedTitle = CreateText("SelectedPanelTitleText", selectedPanel, 28, TextAnchor.UpperLeft);
            selectedTitle.fontStyle = FontStyle.Bold;
            selectedTitle.text = "선택한 카드 효과";
            selectedTitle.rectTransform.anchorMin = new Vector2(0f, 0.82f);
            selectedTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            selectedTitle.rectTransform.offsetMin = new Vector2(24f, -2f);
            selectedTitle.rectTransform.offsetMax = new Vector2(-24f, -16f);

            selectedCardsText = CreateText("SelectedCardsText", selectedPanel, 30, TextAnchor.MiddleCenter);
            selectedCardsText.color = new Color32(240, 244, 250, 255);
            selectedCardsText.fontStyle = FontStyle.Bold;
            selectedCardsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            selectedCardsText.verticalOverflow = VerticalWrapMode.Overflow;
            selectedCardsText.lineSpacing = 1.1f;
            selectedCardsText.resizeTextForBestFit = true;
            selectedCardsText.resizeTextMinSize = 18;
            selectedCardsText.resizeTextMaxSize = 30;
            selectedCardsText.rectTransform.anchorMin = new Vector2(0f, 0f);
            selectedCardsText.rectTransform.anchorMax = new Vector2(1f, 0.82f);
            selectedCardsText.rectTransform.offsetMin = new Vector2(28f, 24f);
            selectedCardsText.rectTransform.offsetMax = new Vector2(-28f, -8f);

            RectTransform sidePanel = CreatePanel("SidePanel", lowerPanel, new Color32(0, 0, 0, 0));
            LayoutElement sidePanelLayout = sidePanel.gameObject.AddComponent<LayoutElement>();
            sidePanelLayout.preferredWidth = 420f;
            sidePanelLayout.minWidth = 390f;

            VerticalLayoutGroup sideGroup = sidePanel.gameObject.AddComponent<VerticalLayoutGroup>();
            sideGroup.spacing = 18f;
            sideGroup.childControlWidth = true;
            sideGroup.childControlHeight = true;
            sideGroup.childForceExpandWidth = true;
            sideGroup.childForceExpandHeight = false;

            RectTransform enemyInfoPanel = CreatePanel("EnemyInfoPanel", sidePanel, new Color32(245, 223, 188, 255));
            enemyInfoPanelRoot = enemyInfoPanel.gameObject;
            LayoutElement enemyInfoLayout = enemyInfoPanel.gameObject.AddComponent<LayoutElement>();
            enemyInfoLayout.flexibleHeight = 1f;
            enemyInfoLayout.preferredHeight = 0f;
            enemyInfoLayout.minHeight = 0f;
            enemyInfoPanel.GetComponent<Image>().color = new Color32(37, 44, 61, 255);
            AddOutline(enemyInfoPanel.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            Text enemyInfoTitle = CreateText("EnemyInfoTitleText", enemyInfoPanel, 28, TextAnchor.UpperLeft);
            enemyInfoTitle.fontStyle = FontStyle.Bold;
            enemyInfoTitle.text = "몬스터 능력치";
            enemyInfoTitle.rectTransform.anchorMin = new Vector2(0f, 0.82f);
            enemyInfoTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            enemyInfoTitle.rectTransform.offsetMin = new Vector2(24f, -2f);
            enemyInfoTitle.rectTransform.offsetMax = new Vector2(-24f, -16f);

            monsterInfoText = CreateText("EnemyInfoText", enemyInfoPanel, 30, TextAnchor.MiddleCenter);
            monsterInfoText.color = new Color32(240, 244, 250, 255);
            monsterInfoText.fontStyle = FontStyle.Bold;
            monsterInfoText.resizeTextForBestFit = true;
            monsterInfoText.resizeTextMinSize = 18;
            monsterInfoText.resizeTextMaxSize = 30;
            monsterInfoText.lineSpacing = 1.1f;
            monsterInfoText.rectTransform.anchorMin = new Vector2(0f, 0f);
            monsterInfoText.rectTransform.anchorMax = new Vector2(1f, 0.82f);
            monsterInfoText.rectTransform.offsetMin = new Vector2(28f, 20f);
            monsterInfoText.rectTransform.offsetMax = new Vector2(-28f, -8f);

            RectTransform relicPanel = CreatePanel("RelicPanel", sidePanel, new Color32(37, 44, 61, 255));
            relicPanelRoot = relicPanel.gameObject;
            LayoutElement relicLayout = relicPanel.gameObject.AddComponent<LayoutElement>();
            relicLayout.flexibleHeight = 1f;
            relicLayout.preferredHeight = 0f;
            relicLayout.minHeight = 0f;
            AddOutline(relicPanel.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            Text relicTitle = CreateText("RelicTitleText", relicPanel, 28, TextAnchor.UpperLeft);
            relicTitle.fontStyle = FontStyle.Bold;
            relicTitle.text = "획득한 유물";
            relicTitle.rectTransform.anchorMin = new Vector2(0f, 0.78f);
            relicTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            relicTitle.rectTransform.offsetMin = new Vector2(24f, -4f);
            relicTitle.rectTransform.offsetMax = new Vector2(-24f, -18f);

            RectTransform relicGrid = CreatePanel("RelicGrid", relicPanel, new Color32(0, 0, 0, 0));
            relicGrid.anchorMin = new Vector2(0f, 0f);
            relicGrid.anchorMax = new Vector2(1f, 0.82f);
            relicGrid.offsetMin = new Vector2(18f, 18f);
            relicGrid.offsetMax = new Vector2(-18f, -4f);

            VerticalLayoutGroup relicRowsLayout = relicGrid.gameObject.AddComponent<VerticalLayoutGroup>();
            relicRowsLayout.spacing = 18f;
            relicRowsLayout.padding = new RectOffset(10, 10, 22, 10);
            relicRowsLayout.childAlignment = TextAnchor.UpperCenter;
            relicRowsLayout.childControlWidth = true;
            relicRowsLayout.childControlHeight = true;
            relicRowsLayout.childForceExpandWidth = true;
            relicRowsLayout.childForceExpandHeight = false;

            RectTransform topRow = CreatePanel("RelicRowTop", relicGrid, new Color32(0, 0, 0, 0));
            LayoutElement topRowLayout = topRow.gameObject.AddComponent<LayoutElement>();
            topRowLayout.preferredHeight = 148f;
            HorizontalLayoutGroup topRowGroup = topRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            topRowGroup.spacing = 14f;
            topRowGroup.childAlignment = TextAnchor.MiddleCenter;
            topRowGroup.childControlWidth = false;
            topRowGroup.childControlHeight = false;
            topRowGroup.childForceExpandWidth = false;
            topRowGroup.childForceExpandHeight = false;

            RectTransform bottomRow = CreatePanel("RelicRowBottom", relicGrid, new Color32(0, 0, 0, 0));
            LayoutElement bottomRowLayout = bottomRow.gameObject.AddComponent<LayoutElement>();
            bottomRowLayout.preferredHeight = 148f;
            HorizontalLayoutGroup bottomRowGroup = bottomRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            bottomRowGroup.spacing = 18f;
            bottomRowGroup.childAlignment = TextAnchor.MiddleCenter;
            bottomRowGroup.childControlWidth = false;
            bottomRowGroup.childControlHeight = false;
            bottomRowGroup.childForceExpandWidth = false;
            bottomRowGroup.childForceExpandHeight = false;

            for (int index = 0; index < 5; index++)
            {
                Transform rowParent = index < 3 ? topRow : bottomRow;
                RectTransform slot = CreatePanel($"RelicSlot{index}", rowParent, new Color32(57, 69, 94, 255));
                LayoutElement slotLayout = slot.gameObject.AddComponent<LayoutElement>();
                slotLayout.preferredWidth = 132f;
                slotLayout.preferredHeight = 132f;
                slotLayout.minWidth = 132f;
                slotLayout.minHeight = 132f;
                slot.sizeDelta = new Vector2(132f, 132f);
                Image slotImage = slot.GetComponent<Image>();
                AddOutline(slot.gameObject, new Color32(255, 255, 255, 24), new Vector2(2f, -2f));
                ApplyRelicFrameSprite(slotImage);
                Button slotButton = slot.gameObject.AddComponent<Button>();
                SetButtonColors(slotButton, slotImage.color, slotImage.color, slotImage.color);
                int capturedSlotIndex = index;
                slotButton.onClick.AddListener(() => OnRelicSlotSelected(capturedSlotIndex));

                RectTransform iconHolder = CreatePanel($"RelicSlotIconHolder{index}", slot, new Color32(0, 0, 0, 0));
                iconHolder.anchorMin = new Vector2(0.18f, 0.18f);
                iconHolder.anchorMax = new Vector2(0.82f, 0.82f);
                iconHolder.offsetMin = Vector2.zero;
                iconHolder.offsetMax = Vector2.zero;
                Image iconImage = iconHolder.GetComponent<Image>();
                iconImage.preserveAspect = true;

                Text slotText = CreateText($"RelicSlotText{index}", slot, 22, TextAnchor.MiddleCenter);
                slotText.fontStyle = FontStyle.Bold;
                Stretch(slotText.rectTransform, 10f);
                relicSlotRects.Add(slot);
                relicSlotIconRects.Add(iconHolder);
                relicSlotImages.Add(slotImage);
                relicSlotIconImages.Add(iconImage);
                relicSlotTexts.Add(slotText);
                relicSlotButtons.Add(slotButton);
            }

            statePanelRoot = CreatePanel("HiddenStatePanel", sidePanel, new Color32(0, 0, 0, 0)).gameObject;
            statePanelRoot.SetActive(false);

            actionButtonContainer = CreatePanel("ActionButtonContainer", screenRoot, new Color32(0, 0, 0, 0)).gameObject;
            LayoutElement actionLayout = actionButtonContainer.AddComponent<LayoutElement>();
            actionLayout.preferredHeight = 170f;

            RectTransform actionButtonRect = CreateButton("ActionButton", actionButtonContainer.transform, out actionButton, out actionButtonText);
            Stretch(actionButtonRect, 0f);
            actionButtonText.fontSize = 34;
            actionButtonText.fontStyle = FontStyle.Bold;
            actionButtonText.color = Color.white;
            SetButtonColors(actionButton, new Color32(91, 127, 196, 255), new Color32(118, 151, 219, 255), new Color32(65, 99, 171, 255));
            AddOutline(actionButtonRect.gameObject, new Color32(255, 255, 255, 28), new Vector2(4f, -4f));

            BuildDraftOverlay(screenRoot);

            relicsText = CreateText("RelicsText", screenRoot.transform, 1, TextAnchor.MiddleCenter);
            relicsText.gameObject.SetActive(false);
            ApplyNeutralPortraitSprites();
            return screenRoot.gameObject;
        }

        private void BuildDraftOverlay(RectTransform screenRoot)
        {
            RectTransform overlayRoot = CreatePanel("DraftOverlay", screenRoot, new Color(0f, 0f, 0f, 0.52f));
            choiceRowContainer = overlayRoot.gameObject;
            LayoutElement overlayLayoutElement = overlayRoot.gameObject.AddComponent<LayoutElement>();
            overlayLayoutElement.ignoreLayout = true;
            Stretch(overlayRoot, 0f);
            overlayRoot.SetAsLastSibling();
            draftOverlayCanvasGroup = overlayRoot.gameObject.AddComponent<CanvasGroup>();

            RectTransform cardsRow = CreatePanel("DraftOverlayCardsRow", overlayRoot, new Color32(0, 0, 0, 0));
            draftOverlayCardsRowRect = cardsRow;
            cardsRow.anchorMin = new Vector2(0.08f, 0.34f);
            cardsRow.anchorMax = new Vector2(0.92f, 0.72f);
            cardsRow.offsetMin = Vector2.zero;
            cardsRow.offsetMax = Vector2.zero;

            HorizontalLayoutGroup choiceGroup = cardsRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            choiceGroup.spacing = 18f;
            choiceGroup.padding = new RectOffset(0, 0, 0, 0);
            choiceGroup.childControlWidth = true;
            choiceGroup.childControlHeight = true;
            choiceGroup.childForceExpandWidth = true;
            choiceGroup.childForceExpandHeight = true;

            optionButtons = new Button[3];
            optionButtonTexts = new Text[3];
            optionButtonImages = new Image[3];
            for (int index = 0; index < optionButtons.Length; index++)
            {
                RectTransform cardButtonRect = CreateButton($"OptionButton{index}", cardsRow.transform, out optionButtons[index], out optionButtonTexts[index]);
                LayoutElement cardLayout = cardButtonRect.gameObject.AddComponent<LayoutElement>();
                cardLayout.flexibleWidth = 1f;
                cardLayout.preferredHeight = 0f;
                AddOutline(cardButtonRect.gameObject, new Color32(255, 255, 255, 30), new Vector2(4f, -4f));
                SetButtonColors(optionButtons[index], new Color32(63, 76, 102, 255), new Color32(86, 101, 131, 255), new Color32(49, 61, 84, 255));
                optionButtonImages[index] = optionButtons[index].GetComponent<Image>();
                optionButtonTexts[index].fontSize = 30;
                optionButtonTexts[index].alignment = TextAnchor.MiddleCenter;
                optionButtonTexts[index].fontStyle = FontStyle.Bold;
                optionButtonTexts[index].color = Color.white;
                optionButtonTexts[index].horizontalOverflow = HorizontalWrapMode.Wrap;
                optionButtonTexts[index].verticalOverflow = VerticalWrapMode.Overflow;
                optionButtonTexts[index].lineSpacing = 1.15f;
                optionButtonTexts[index].resizeTextForBestFit = true;
                optionButtonTexts[index].resizeTextMinSize = 18;
                optionButtonTexts[index].resizeTextMaxSize = 30;
                optionButtonTexts[index].rectTransform.anchorMin = new Vector2(0f, 0f);
                optionButtonTexts[index].rectTransform.anchorMax = new Vector2(1f, 1f);
                optionButtonTexts[index].rectTransform.offsetMin = new Vector2(34f, 34f);
                optionButtonTexts[index].rectTransform.offsetMax = new Vector2(-34f, -34f);
                int capturedIndex = index;
                optionButtons[index].onClick.AddListener(() => OnOptionSelected(capturedIndex));
            }

            choiceRowContainer.SetActive(false);
        }

        private void BuildDetailOverlay(Transform canvasTransform)
        {
            RectTransform overlayRoot = CreatePanel("DetailOverlay", canvasTransform, new Color(0f, 0f, 0f, 0.68f));
            detailOverlayRoot = overlayRoot.gameObject;
            Stretch(overlayRoot, 0f);
            detailOverlayCanvasGroup = overlayRoot.gameObject.AddComponent<CanvasGroup>();
            Button overlayCloseButton = overlayRoot.gameObject.AddComponent<Button>();
            SetButtonColors(overlayCloseButton, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0f));
            overlayCloseButton.onClick.AddListener(CloseDetailOverlay);

            RectTransform modalPanel = CreatePanel("DetailOverlayPanel", overlayRoot, new Color32(30, 37, 52, 252));
            modalPanel.anchorMin = new Vector2(0.08f, 0.12f);
            modalPanel.anchorMax = new Vector2(0.92f, 0.88f);
            modalPanel.offsetMin = Vector2.zero;
            modalPanel.offsetMax = Vector2.zero;
            modalPanel.GetComponent<Image>().enabled = false;

            detailOverlayTitleText = CreateText("DetailOverlayTitleText", modalPanel, 34, TextAnchor.UpperLeft);
            detailOverlayTitleText.fontStyle = FontStyle.Bold;
            detailOverlayTitleText.rectTransform.anchorMin = new Vector2(0f, 0.9f);
            detailOverlayTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            detailOverlayTitleText.rectTransform.offsetMin = new Vector2(24f, 8f);
            detailOverlayTitleText.rectTransform.offsetMax = new Vector2(-24f, -14f);
            detailOverlayTitleText.raycastTarget = false;
            detailOverlayTitleText.gameObject.SetActive(false);

            RectTransform contentPanel = CreatePanel("DetailOverlayContentPanel", modalPanel, new Color32(39, 47, 67, 255));
            contentPanel.anchorMin = new Vector2(0.05f, 0.14f);
            contentPanel.anchorMax = new Vector2(0.95f, 0.92f);
            contentPanel.offsetMin = Vector2.zero;
            contentPanel.offsetMax = Vector2.zero;
            AddOutline(contentPanel.gameObject, new Color32(255, 255, 255, 16), new Vector2(2f, -2f));
            contentPanel.GetComponent<Image>().raycastTarget = false;

            RectTransform relicRoot = CreatePanel("DetailOverlayRelicRoot", contentPanel, new Color32(0, 0, 0, 0));
            detailOverlayRelicRoot = relicRoot.gameObject;
            Stretch(relicRoot, 0f);
            relicRoot.GetComponent<Image>().raycastTarget = false;
            detailOverlayRelicRoot.SetActive(false);

            RectTransform relicIconPanel = CreatePanel("DetailOverlayRelicIconPanel", relicRoot, new Color32(57, 69, 94, 255));
            relicIconPanel.anchorMin = new Vector2(0.32f, 0.5f);
            relicIconPanel.anchorMax = new Vector2(0.68f, 0.92f);
            relicIconPanel.offsetMin = Vector2.zero;
            relicIconPanel.offsetMax = Vector2.zero;
            AddOutline(relicIconPanel.gameObject, new Color32(255, 255, 255, 20), new Vector2(2f, -2f));
            relicIconPanel.GetComponent<Image>().raycastTarget = false;

            RectTransform relicIcon = CreatePanel("DetailOverlayRelicImage", relicIconPanel, new Color32(0, 0, 0, 0));
            relicIcon.anchorMin = new Vector2(0.16f, 0.16f);
            relicIcon.anchorMax = new Vector2(0.84f, 0.84f);
            relicIcon.offsetMin = Vector2.zero;
            relicIcon.offsetMax = Vector2.zero;
            detailOverlayRelicImage = relicIcon.GetComponent<Image>();
            detailOverlayRelicImage.preserveAspect = true;
            detailOverlayRelicImage.raycastTarget = false;

            detailOverlayNameText = CreateText("DetailOverlayNameText", relicRoot, 64, TextAnchor.MiddleCenter);
            detailOverlayNameText.fontStyle = FontStyle.Bold;
            detailOverlayNameText.resizeTextForBestFit = true;
            detailOverlayNameText.resizeTextMinSize = 36;
            detailOverlayNameText.resizeTextMaxSize = 64;
            detailOverlayNameText.rectTransform.anchorMin = new Vector2(0.08f, 0.34f);
            detailOverlayNameText.rectTransform.anchorMax = new Vector2(0.92f, 0.46f);
            detailOverlayNameText.rectTransform.offsetMin = Vector2.zero;
            detailOverlayNameText.rectTransform.offsetMax = Vector2.zero;
            detailOverlayNameText.raycastTarget = false;

            stateInfoText = CreateText("StateInfoText", contentPanel, 48, TextAnchor.UpperLeft);
            stateInfoText.color = new Color32(236, 240, 247, 255);
            stateInfoText.resizeTextForBestFit = true;
            stateInfoText.resizeTextMinSize = 24;
            stateInfoText.resizeTextMaxSize = 48;
            Stretch(stateInfoText.rectTransform, 20f);
            stateInfoText.raycastTarget = false;

            RectTransform closeButtonRect = CreateButton("DetailOverlayCloseButton", modalPanel, out detailOverlayCloseButton, out detailOverlayCloseButtonText);
            closeButtonRect.anchorMin = new Vector2(0.18f, 0f);
            closeButtonRect.anchorMax = new Vector2(0.82f, 1f);
            closeButtonRect.offsetMin = new Vector2(0f, 245f);
            closeButtonRect.offsetMax = new Vector2(0f, -245f);
            detailOverlayCloseButtonText.fontStyle = FontStyle.Bold;
            detailOverlayCloseButtonText.text = "닫기";
            detailOverlayCloseButtonText.raycastTarget = false;
            detailOverlayCloseButton.onClick.AddListener(CloseDetailOverlay);
            detailOverlayCloseButton.gameObject.SetActive(false);

            detailOverlayRoot.SetActive(false);
        }

        private void BuildRewardOverlay(Transform canvasTransform)
        {
            RectTransform overlayRoot = CreatePanel("RewardOverlay", canvasTransform, new Color(0f, 0f, 0f, 0.2f));
            rewardOverlayRoot = overlayRoot.gameObject;
            Stretch(overlayRoot, 0f);
            rewardOverlayCanvasGroup = overlayRoot.gameObject.AddComponent<CanvasGroup>();

            RectTransform rewardCard = CreatePanel("RewardOverlayCard", overlayRoot, new Color32(0, 0, 0, 0));
            rewardOverlayCardRect = rewardCard;
            rewardCard.anchorMin = new Vector2(0.5f, 0.5f);
            rewardCard.anchorMax = new Vector2(0.5f, 0.5f);
            rewardCard.sizeDelta = new Vector2(280f, 280f);
            rewardCard.anchoredPosition = Vector2.zero;

            rewardOverlayFrameImage = rewardCard.GetComponent<Image>();
            rewardOverlayFrameImage.sprite = null;
            rewardOverlayFrameImage.color = new Color32(0, 0, 0, 0);
            rewardOverlayFrameImage.raycastTarget = false;

            RectTransform iconRect = CreatePanel("RewardOverlayIcon", rewardCard, new Color32(0, 0, 0, 0));
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            rewardOverlayIconImage = iconRect.GetComponent<Image>();
            rewardOverlayIconImage.preserveAspect = true;

            rewardOverlayLabelText = CreateText("RewardOverlayLabelText", rewardCard, 28, TextAnchor.LowerCenter);
            rewardOverlayLabelText.fontStyle = FontStyle.Bold;
            rewardOverlayLabelText.color = Color.white;
            rewardOverlayLabelText.rectTransform.anchorMin = new Vector2(0f, -0.24f);
            rewardOverlayLabelText.rectTransform.anchorMax = new Vector2(1f, -0.02f);
            rewardOverlayLabelText.rectTransform.offsetMin = new Vector2(-60f, 0f);
            rewardOverlayLabelText.rectTransform.offsetMax = new Vector2(60f, 0f);

            rewardOverlayRoot.SetActive(false);
        }

        private RectTransform CreatePortraitCard(
            Transform parent,
            string name,
            string badge,
            Color accentColor,
            out Image portraitImage,
            out Text portraitText,
            out Text infoText)
        {
            RectTransform card = CreatePanel(name, parent, new Color32(44, 53, 74, 255));
            LayoutElement layout = card.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            AddOutline(card.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));

            Text badgeText = CreateText($"{name}BadgeText", card, 26, TextAnchor.UpperLeft);
            badgeText.fontStyle = FontStyle.Bold;
            badgeText.color = accentColor;
            badgeText.text = badge;
            badgeText.rectTransform.anchorMin = new Vector2(0f, 0.78f);
            badgeText.rectTransform.anchorMax = new Vector2(1f, 1f);
            badgeText.rectTransform.offsetMin = new Vector2(18f, 4f);
            badgeText.rectTransform.offsetMax = new Vector2(-18f, -18f);

            RectTransform portraitPanel = CreatePanel($"{name}PortraitPanel", card, accentColor);
            portraitPanel.anchorMin = new Vector2(0.16f, 0.42f);
            portraitPanel.anchorMax = new Vector2(0.84f, 0.76f);
            portraitPanel.offsetMin = Vector2.zero;
            portraitPanel.offsetMax = Vector2.zero;
            AddOutline(portraitPanel.gameObject, new Color32(255, 255, 255, 25), new Vector2(2f, -2f));

            portraitImage = CreatePanel($"{name}PortraitImage", portraitPanel, new Color32(0, 0, 0, 0)).GetComponent<Image>();
            portraitImage.preserveAspect = true;
            Stretch(portraitImage.rectTransform, 10f);

            portraitText = CreateText($"{name}PortraitText", portraitPanel, 26, TextAnchor.LowerCenter);
            portraitText.fontStyle = FontStyle.Bold;
            portraitText.color = new Color32(241, 245, 255, 255);
            portraitText.rectTransform.anchorMin = new Vector2(0f, 0f);
            portraitText.rectTransform.anchorMax = new Vector2(1f, 0.28f);
            portraitText.rectTransform.offsetMin = new Vector2(8f, 6f);
            portraitText.rectTransform.offsetMax = new Vector2(-8f, -6f);

            infoText = CreateText($"{name}InfoText", card, 23, TextAnchor.UpperLeft);
            infoText.color = new Color32(227, 232, 241, 255);
            infoText.rectTransform.anchorMin = new Vector2(0f, 0f);
            infoText.rectTransform.anchorMax = new Vector2(1f, 0.42f);
            infoText.rectTransform.offsetMin = new Vector2(18f, 18f);
            infoText.rectTransform.offsetMax = new Vector2(-18f, -8f);
            return card;
        }

        private void ShowMainMenu()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.MainMenu);
            menuScreen.SetActive(true);
            if (logbookScreen != null)
            {
                logbookScreen.SetActive(false);
            }
            if (codexScreen != null)
            {
                codexScreen.SetActive(false);
            }
            runScreen.SetActive(false);
            SetResultTouchOverlay(false, string.Empty);
            CloseDetailOverlay();
            CloseLogDetailOverlay();

            headerText.text = "슬라임 실험 연구실";
            subHeaderText.text = "실험체를 조합하고 결과를 수집하는 모바일 세로형 실험 시뮬레이터";
            primaryButton.gameObject.SetActive(true);
            primaryButtonText.text = "실험 시작";
        }

        private void ShowLogbookScreen()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Logbook);
            menuScreen.SetActive(false);
            if (codexScreen != null)
            {
                codexScreen.SetActive(false);
            }

            logbookScreen.SetActive(true);
            runScreen.SetActive(false);
            SetResultTouchOverlay(false, string.Empty);
            CloseDetailOverlay();
            CloseLogDetailOverlay();
            UpdateLogbookEntries();

            headerText.text = "실험 로그";
            subHeaderText.text = "최근 실험부터 과거 기록까지 스크롤하며 실험 이력을 확인할 수 있습니다.";
        }

        private void ShowCodexScreen()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Codex);
            menuScreen.SetActive(false);
            if (logbookScreen != null)
            {
                logbookScreen.SetActive(false);
            }
            codexScreen.SetActive(true);
            runScreen.SetActive(false);
            SetResultTouchOverlay(false, string.Empty);
            CloseDetailOverlay();
            CloseLogDetailOverlay();
            UpdateCodexEntries();

            headerText.text = "변수 도감";
            subHeaderText.text = "실험 중 획득했던 변수와 아직 발견하지 못한 변수를 한 곳에서 확인합니다.";
        }

        private void StartExperiment()
        {
            gameSession.Begin(experimentLogService.GetNextExperimentNumber());
            ResetRunViewForNewExperiment();
            DrawCurrentDraftChoices();
        }

        private void DrawCurrentDraftChoices()
        {
            StopAllCoroutines();
            currentChoices.Clear();
            currentChoices.AddRange(draftService.DrawChoices(gameSession.DraftPhase));

            screenController.Show(ScreenState.Draft);
            menuScreen.SetActive(false);
            if (logbookScreen != null)
            {
                logbookScreen.SetActive(false);
            }
            if (codexScreen != null)
            {
                codexScreen.SetActive(false);
            }
            runScreen.SetActive(true);
            primaryButton.gameObject.SetActive(false);
            SetResultTouchOverlay(false, string.Empty);
            CloseDetailOverlay();
            SetActionButton("다음 단계 대기", null, false);
            choiceRowContainer.SetActive(true);
            relicPanelRoot.SetActive(true);
            if (enemyInfoPanelRoot != null)
            {
                enemyInfoPanelRoot.SetActive(true);
            }
            ResetBattleActorPresentation();

            headerText.text = $"실험 #{gameSession.ExperimentNumber:000}";
            subHeaderText.text = BuildDraftPromptText();
            headerStageBadgeText.text = $"카드 선택 {GetDraftStepIndex()}/3";
            centerStateText.text = "세 장 중 하나를 선택해 슬라임 실험체를 조합하세요.";
            ApplyDraftVisualTheme();

            playerPortraitText.text = "SLIME";
            enemyPortraitText.text = "???";

            UpdatePersistentPanels();
            SetCurrentDetailLog("전투 로그", "아직 전투 로그가 없습니다.\n카드를 선택하고 실험을 진행하면 전투 기록이 여기에 쌓입니다.");
            StartCoroutine(RevealChoiceButtons());
        }

        private IEnumerator RevealChoiceButtons()
        {
            isSelectionLocked = false;
            HideOptionButtons();
            draftOverlayCanvasGroup.alpha = 0f;
            draftOverlayCardsRowRect.localScale = new Vector3(0.92f, 0.92f, 1f);

            for (int index = 0; index < optionButtons.Length; index++)
            {
                bool hasChoice = index < currentChoices.Count;
                optionButtons[index].gameObject.SetActive(hasChoice);
                if (hasChoice)
                {
                    optionButtons[index].interactable = true;
                    optionButtonTexts[index].text = BuildCardButtonText(currentChoices[index]);
                    CanvasGroup canvasGroup = optionButtons[index].GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = optionButtons[index].gameObject.AddComponent<CanvasGroup>();
                    }

                    canvasGroup.alpha = 0f;
                    optionButtons[index].transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                }
            }

            IEnumerator fadeOverlay = FadeCanvasGroup(draftOverlayCanvasGroup, 0f, 1f, 0.12f);
            IEnumerator scaleCards = ScaleRectTransform(draftOverlayCardsRowRect, new Vector3(0.92f, 0.92f, 1f), Vector3.one, 0.14f);
            IEnumerator[] cardAnimations = new IEnumerator[optionButtons.Length];
            for (int index = 0; index < optionButtons.Length; index++)
            {
                cardAnimations[index] = AnimateCardIn(optionButtons[index]);
            }

            yield return StartCoroutine(RunAnimationsTogether(CombineAnimations(fadeOverlay, scaleCards, cardAnimations)));
        }

        private void OnOptionSelected(int index)
        {
            if (index >= currentChoices.Count || isSelectionLocked)
            {
                return;
            }

            StartCoroutine(HandleOptionSelection(index));
        }

        private IEnumerator HandleOptionSelection(int index)
        {
            isSelectionLocked = true;

            for (int buttonIndex = 0; buttonIndex < optionButtons.Length; buttonIndex++)
            {
                optionButtons[buttonIndex].interactable = false;
            }

            Button selectedButton = optionButtons[index];
            Image selectedImage = optionButtonImages[index];
            Color originalColor = selectedImage.color;
            selectedImage.color = GetDraftAccentColor();

            yield return StartCoroutine(ScaleRectTransform(selectedButton.GetComponent<RectTransform>(), Vector3.one, new Vector3(1.05f, 1.05f, 1f), 0.1f));
            yield return StartCoroutine(ScaleRectTransform(selectedButton.GetComponent<RectTransform>(), new Vector3(1.05f, 1.05f, 1f), new Vector3(0.98f, 0.98f, 1f), 0.08f));
            yield return StartCoroutine(HideChoiceButtonsTogether());

            selectedImage.color = originalColor;
            gameSession.ApplySelectedCard(currentChoices[index], draftService);

            if (gameSession.DraftPhase == DraftPhase.Complete)
            {
                UpdatePersistentPanels();
                ShowRoulette();
                yield break;
            }

            DrawCurrentDraftChoices();
        }

        private void ShowRoulette()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Roulette);
            choiceRowContainer.SetActive(false);
            CloseDetailOverlay();
            SetActionButton("자동 전투 시작 대기", null, false);
            SetResultTouchOverlay(false, string.Empty);
            ResetBattleActorPresentation();

            MonsterEncounter encounter = runProgressController.PrepareEncounter();
            headerText.text = gameSession.IsBossStage() ? "보스 실험 개시" : $"실험 #{gameSession.ExperimentNumber:000}";
            subHeaderText.text = gameSession.IsBossStage() ? "최종 보스의 특성이 룰렛으로 결정됩니다." : $"스테이지 {gameSession.StageIndex} 실험 대상 생성 중";
            headerStageBadgeText.text = gameSession.IsBossStage() ? "BOSS" : $"STAGE {gameSession.StageIndex}";
            centerStateText.text = "룰렛이 공격, 유틸, 속성을 차례로 확정합니다.";
            playerPortraitText.text = "SLIME";
            enemyPortraitText.text = "???";
            UpdatePersistentPanels();
            SetCurrentDetailLog("전투 로그", "아직 전투가 시작되지 않았습니다.\n룰렛이 완료되면 자동 전투를 시작할 수 있습니다.");

            StartCoroutine(PlayRouletteSequence(encounter));
        }

        private IEnumerator PlayRouletteSequence(MonsterEncounter finalEncounter)
        {
            enemyPortraitText.text = "ROLL";
            ApplyPortraitSprite(enemyPortraitImage, GetMonsterSprite(AttributeType.Neutral));

            for (int index = 0; index < 9; index++)
            {
                AttributeType rolledAttribute = (AttributeType)(index % 6);
                MonsterUtilityType rolledUtility = (MonsterUtilityType)(index % 5);
                string attributeLabel = monsterFactory.GetAttributeLabel(rolledAttribute);
                string utilityLabel = monsterFactory.GetMonsterUtilityLabel(rolledUtility, finalEncounter.Rule.IsBoss);
                monsterInfoText.text = BuildRollingMonsterPreviewText(finalEncounter, attributeLabel, utilityLabel);
                centerStateText.text = $"룰렛 진행 중 {index + 1}/9";
                ApplyPortraitSprite(enemyPortraitImage, GetMonsterSprite(rolledAttribute));
                SetHealthBarState(enemyHealthFillImage, enemyHealthValueText, finalEncounter.Rule.Health, finalEncounter.Rule.Health, true);
                yield return new WaitForSeconds(0.12f);
            }

            enemyPortraitText.text = finalEncounter.Rule.IsBoss ? "BOSS" : "TEST";
            ApplyPortraitSprite(enemyPortraitImage, GetMonsterSprite(finalEncounter.AttributeType));
            monsterInfoText.text = BuildEncounterEffectText(finalEncounter);
            centerStateText.text = "룰렛 완료. 자동 전투를 시작하세요.";
            SetCurrentDetailLog("전투 로그", $"전투 시작 준비 완료\n\n상대 정보\n{BuildEncounterDetailText(finalEncounter)}");
            SetActionButton("자동 전투 시작", StartBattle, true);
        }

        private void StartBattle()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Battle);
            choiceRowContainer.SetActive(false);
            CloseDetailOverlay();
            SetActionButton("전투 진행 중", null, false);
            SetResultTouchOverlay(false, string.Empty);
            ResetBattleActorPresentation();

            BattleReport report = runProgressController.StartBattle();

            headerText.text = gameSession.IsBossStage() ? "보스 전투 결과" : $"실험 #{gameSession.ExperimentNumber:000}";
            subHeaderText.text = gameSession.IsBossStage() ? "보스 전투가 진행됩니다." : $"스테이지 {gameSession.StageIndex} 자동 전투 진행";
            headerStageBadgeText.text = gameSession.IsBossStage() ? "BOSS BATTLE" : $"STAGE {gameSession.StageIndex}";
            centerStateText.text = "전투 시작";
            playerPortraitText.text = "SLIME";
            enemyPortraitText.text = gameSession.IsBossStage() ? "BOSS" : "TEST";
            UpdatePersistentPanels();
            ResetBattleActorPresentation();
            displayedPlayerHealth = report.InitialPlayerState.CurrentHealth;
            displayedEnemyHealth = report.InitialMonsterState.CurrentHealth;
            SetHealthBarState(playerHealthFillImage, playerHealthValueText, report.InitialPlayerState.CurrentHealth, report.InitialPlayerState.MaxHealth, true);
            SetHealthBarState(enemyHealthFillImage, enemyHealthValueText, report.InitialMonsterState.CurrentHealth, report.InitialMonsterState.MaxHealth, true);

            StartCoroutine(PresentBattleReport(report));
        }

        private IEnumerator PresentBattleReport(BattleReport report)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(report.Outcome == BattleOutcome.Victory ? "전투 승리" : "전투 패배");
            builder.AppendLine();

            int startIndex = 0;
            if (report.Events.Count > 14)
            {
                startIndex = report.Events.Count - 14;
            }

            for (int index = startIndex; index < report.Events.Count; index++)
            {
                builder.Append("- ");
                builder.AppendLine(report.Events[index].Message);
                yield return StartCoroutine(PlayBattleEventPresentation(report.Events[index], report));
                SetCurrentDetailLog("전투 로그", builder.ToString());
            }

            SetHealthBarState(playerHealthFillImage, playerHealthValueText, report.FinalPlayerState.CurrentHealth, report.FinalPlayerState.MaxHealth, true);
            SetHealthBarState(enemyHealthFillImage, enemyHealthValueText, report.FinalMonsterState.CurrentHealth, report.FinalMonsterState.MaxHealth, true);
            displayedPlayerHealth = report.FinalPlayerState.CurrentHealth;
            displayedEnemyHealth = report.FinalMonsterState.CurrentHealth;
            centerStateText.text = report.Outcome == BattleOutcome.Victory ? "슬라임 승리" : "슬라임 붕괴";

            if (report.Outcome == BattleOutcome.Victory)
            {
                if (gameSession.IsBossStage())
                {
                    SetActionButton("실험 결과 보기", ShowResultScreen, true);
                }
                else
                {
                    SetActionButton("랜덤 유물 획득", ShowRewardScreen, true);
                }
            }
            else
            {
                SetActionButton("실험 종료", ShowResultScreen, true);
            }
        }

        private void ShowRewardScreen()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Reward);
            SetResultTouchOverlay(false, string.Empty);
            choiceRowContainer.SetActive(false);
            CloseDetailOverlay();

            RelicDefinition rewardedRelic = runProgressController.GrantVictoryRelic();
            int rewardedSlotIndex = rewardedRelic == null ? -1 : gameSession.OwnedRelics.Count - 1;
            hiddenRewardSlotIndex = rewardedSlotIndex;
            headerText.text = $"실험 #{gameSession.ExperimentNumber:000}";
            subHeaderText.text = $"스테이지 {gameSession.StageIndex} 보상 획득";
            headerStageBadgeText.text = "RELIC";
            centerStateText.text = "몬스터 처치로 새로운 변수가 발견되었습니다.";
            playerPortraitText.text = "SLIME";
            enemyPortraitText.text = "CLEAR";
            monsterInfoText.text = "다음 전투에 적용될\n새 유물이 확보되었습니다.";
            UpdatePersistentPanels();
            ResetBattleActorPresentation();

            if (rewardedRelic == null)
            {
                SetCurrentDetailLog("전투 로그", "획득 가능한 유물이 더 없어 이번 스테이지에서는 새 변수가 추가되지 않았습니다.");
                SetActionButton("다음 스테이지", AdvanceToNextStage, true);
            }
            else
            {
                SetCurrentDetailLog("전투 로그", $"새 유물 발견\n\n{rewardedRelic.DisplayName}\n{rewardedRelic.Description}\n\n오른쪽 슬롯에 반영되며 다음 스테이지부터 적용됩니다.");
                StartCoroutine(PlayRewardAcquisitionSequence(rewardedRelic, rewardedSlotIndex));
            }
        }

        private void AdvanceToNextStage()
        {
            runProgressController.AdvanceAfterReward();
            ShowRoulette();
        }

        private void ShowResultScreen()
        {
            StopAllCoroutines();
            screenController.Show(ScreenState.Result);
            SetActionButton("결과 확인 중", null, false);
            choiceRowContainer.SetActive(false);
            CloseDetailOverlay();

            string title = gameSession.RunSucceeded ? "실험 성공" : "실험 실패";
            headerText.text = $"실험 #{gameSession.ExperimentNumber:000} 결과";
            subHeaderText.text = gameSession.RunSucceeded ? "보스 처치에 성공했습니다." : "실험체가 전투 중 붕괴했습니다.";
            headerStageBadgeText.text = title;
            centerStateText.text = "결과를 기록했습니다.";
            UpdatePersistentPanels();

            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendLine(title);
            resultBuilder.AppendLine();
            resultBuilder.AppendLine($"도달 스테이지: {gameSession.StageIndex}");
            resultBuilder.AppendLine();
            resultBuilder.AppendLine("선택 카드");
            AppendNamedLines(resultBuilder, gameSession.GetSelectedCardNames());
            resultBuilder.AppendLine();
            resultBuilder.AppendLine("획득 유물");
            AppendNamedLines(resultBuilder, gameSession.GetOwnedRelicNames());
            SetCurrentDetailLog("전투 로그", resultBuilder.ToString());

            SetResultTouchOverlay(true, "화면을 터치하면 메인 화면으로 복귀합니다.");
        }

        private void UpdatePersistentPanels()
        {
            selectedCardsText.text = BuildSelectedCardsPanelText();
            UpdateRelicSlots();
            UpdatePortraitSprites();
            UpdateBattleStagePresentation();
            monsterInfoText.text = BuildEnemyInfoPanelText();
        }

        private void UpdateRelicSlots()
        {
            for (int index = 0; index < relicSlotImages.Count; index++)
            {
                bool isFilled = index < gameSession.OwnedRelics.Count && index != hiddenRewardSlotIndex;
                if (index < relicSlotButtons.Count)
                {
                    relicSlotButtons[index].interactable = isFilled;
                }

                Sprite relicFrameSprite = GetRelicFrameSprite();
                relicSlotImages[index].color = relicFrameSprite != null
                    ? Color.white
                    : (isFilled ? new Color32(118, 166, 255, 255) : new Color32(57, 69, 94, 255));

                if (isFilled)
                {
                    relicSlotIconImages[index].enabled = true;
                    relicSlotTexts[index].enabled = true;
                    relicSlotIconImages[index].sprite = GetRelicSprite(gameSession.OwnedRelics[index].Id);
                    relicSlotIconImages[index].color = relicSlotIconImages[index].sprite == null ? new Color32(0, 0, 0, 0) : Color.white;
                    relicSlotTexts[index].text = relicSlotIconImages[index].sprite == null ? BuildRelicSlotLabel(gameSession.OwnedRelics[index].DisplayName) : string.Empty;
                }
                else
                {
                    relicSlotIconImages[index].sprite = null;
                    relicSlotIconImages[index].color = new Color32(0, 0, 0, 0);
                    relicSlotIconImages[index].enabled = false;
                    relicSlotTexts[index].enabled = true;
                    relicSlotTexts[index].text = "+";
                }
            }
        }

        private IEnumerator PlayRewardAcquisitionSequence(RelicDefinition rewardedRelic, int targetSlotIndex)
        {
            SetActionButton("유물 획득 중", null, false);
            yield return null;

            if (targetSlotIndex >= 0 && targetSlotIndex < relicSlotIconImages.Count)
            {
                relicSlotIconImages[targetSlotIndex].enabled = false;
                relicSlotTexts[targetSlotIndex].enabled = false;
            }

            Sprite relicSprite = GetRelicSprite(rewardedRelic.Id);
            rewardOverlayFrameImage.sprite = null;
            rewardOverlayFrameImage.color = new Color32(0, 0, 0, 0);
            rewardOverlayIconImage.sprite = relicSprite;
            rewardOverlayIconImage.color = relicSprite == null ? new Color32(0, 0, 0, 0) : Color.white;
            rewardOverlayLabelText.text = rewardedRelic.DisplayName;
            rewardOverlayLabelText.color = Color.white;
            rewardOverlayCanvasGroup.alpha = 1f;
            rewardOverlayRoot.SetActive(true);
            rewardOverlayRoot.transform.SetAsLastSibling();
            rewardOverlayCardRect.anchoredPosition = Vector2.zero;
            rewardOverlayCardRect.localScale = new Vector3(1.85f, 1.85f, 1f);

            yield return new WaitForSeconds(1f);

            rewardOverlayLabelText.text = string.Empty;

            Vector2 targetPosition = GetRelicSlotIconAnchoredPosition(targetSlotIndex);
            float elapsed = 0f;
            float duration = 0.48f;
            Vector2 startPosition = rewardOverlayCardRect.anchoredPosition;
            Vector3 startScale = rewardOverlayCardRect.localScale;
            Vector3 endScale = GetRewardTargetScale(targetSlotIndex);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                rewardOverlayCardRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, eased);
                rewardOverlayCardRect.localScale = Vector3.Lerp(startScale, endScale, eased);
                rewardOverlayCanvasGroup.alpha = Mathf.Lerp(1f, 0.92f, eased);
                yield return null;
            }

            rewardOverlayRoot.SetActive(false);
            rewardOverlayCardRect.anchoredPosition = Vector2.zero;
            rewardOverlayCardRect.localScale = Vector3.one;
            hiddenRewardSlotIndex = -1;

            if (targetSlotIndex >= 0 && targetSlotIndex < relicSlotIconImages.Count)
            {
                relicSlotIconImages[targetSlotIndex].enabled = true;
                relicSlotTexts[targetSlotIndex].enabled = true;
            }

            UpdateRelicSlots();
            SetActionButton("다음 스테이지", AdvanceToNextStage, true);
        }

        private Vector2 GetRelicSlotIconAnchoredPosition(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= relicSlotIconRects.Count || canvasRootRect == null)
            {
                return Vector2.zero;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, relicSlotIconRects[slotIndex].position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRootRect, screenPoint, null, out Vector2 localPoint);
            return localPoint;
        }

        private Vector3 GetRewardTargetScale(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= relicSlotIconRects.Count)
            {
                return Vector3.one;
            }

            float overlayBaseSize = rewardOverlayCardRect.rect.width * 0.8f;
            float targetSize = relicSlotIconRects[slotIndex].rect.width;
            if (overlayBaseSize <= 0.01f || targetSize <= 0.01f)
            {
                return Vector3.one;
            }

            float uniformScale = targetSize / overlayBaseSize;
            return new Vector3(uniformScale, uniformScale, 1f);
        }

        private void UpdatePortraitSprites()
        {
            AttributeType playerAttribute = AttributeType.Neutral;
            if (gameSession.BuildResult != null)
            {
                playerAttribute = gameSession.BuildResult.AttributeType;
            }

            AttributeType monsterAttribute = AttributeType.Neutral;
            if (gameSession.CurrentEncounter != null)
            {
                monsterAttribute = gameSession.CurrentEncounter.AttributeType;
            }

            ApplyPortraitSprite(playerPortraitImage, GetCharacterSprite(playerAttribute));
            ApplyPortraitSprite(playerHandImage, GetCharacterHandSprite(playerAttribute));
            ApplyPortraitSprite(enemyPortraitImage, GetMonsterSprite(monsterAttribute));
        }

        private void ApplyNeutralPortraitSprites()
        {
            ApplyPortraitSprite(playerPortraitImage, GetCharacterSprite(AttributeType.Neutral));
            ApplyPortraitSprite(playerHandImage, GetCharacterHandSprite(AttributeType.Neutral));
            ApplyPortraitSprite(enemyPortraitImage, GetMonsterSprite(AttributeType.Neutral));
        }

        private void ApplyPortraitSprite(Image portraitImage, Sprite sprite)
        {
            portraitImage.sprite = sprite;
            portraitImage.color = sprite == null ? new Color32(0, 0, 0, 0) : Color.white;
        }

        private void ApplyCardFrameSprite(Image image)
        {
            Sprite cardFrameSprite = GetCardFrameSprite();
            if (cardFrameSprite == null)
            {
                return;
            }

            image.sprite = cardFrameSprite;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 4f;
            image.color = Color.white;
        }

        private void ApplyRelicFrameSprite(Image image)
        {
            Sprite relicFrameSprite = GetRelicFrameSprite();
            if (relicFrameSprite == null)
            {
                return;
            }

            image.sprite = relicFrameSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
        }

        private void ApplyInfoFrameSprite(Image image)
        {
            Sprite infoFrameSprite = GetInfoFrameSprite();
            if (infoFrameSprite == null)
            {
                image.color = new Color32(245, 223, 188, 255);
                return;
            }

            image.sprite = infoFrameSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.pixelsPerUnitMultiplier = 2.5f;
        }

        private Sprite GetCardFrameSprite()
        {
            if (spriteCatalog != null && spriteCatalog.CardFrameSprite != null)
            {
                return spriteCatalog.CardFrameSprite;
            }

            return PrototypeSpriteFallbackLibrary.GetCardFrameSprite();
        }

        private Sprite GetRelicFrameSprite()
        {
            if (spriteCatalog != null && spriteCatalog.RelicFrameSprite != null)
            {
                return spriteCatalog.RelicFrameSprite;
            }

            return PrototypeSpriteFallbackLibrary.GetRelicFrameSprite();
        }

        private Sprite GetInfoFrameSprite()
        {
            if (spriteCatalog != null && spriteCatalog.InfoFrameSprite != null)
            {
                return spriteCatalog.InfoFrameSprite;
            }

            return PrototypeSpriteFallbackLibrary.GetInfoFrameSprite();
        }

        private Sprite GetCharacterSprite(AttributeType attributeType)
        {
            Sprite sprite = spriteCatalog != null ? spriteCatalog.GetCharacterSprite(attributeType) : null;
            return sprite ?? PrototypeSpriteFallbackLibrary.GetCharacterSprite(attributeType);
        }

        private Sprite GetCharacterHandSprite(AttributeType attributeType)
        {
            Sprite sprite = spriteCatalog != null ? spriteCatalog.GetCharacterHandSprite(attributeType) : null;
            return sprite ?? PrototypeSpriteFallbackLibrary.GetCharacterHandSprite(attributeType);
        }

        private Sprite GetMonsterSprite(AttributeType attributeType)
        {
            Sprite sprite = spriteCatalog != null ? spriteCatalog.GetMonsterSprite(attributeType) : null;
            return sprite ?? PrototypeSpriteFallbackLibrary.GetMonsterSprite(attributeType);
        }

        private Sprite GetRelicSprite(string relicId)
        {
            Sprite sprite = spriteCatalog != null ? spriteCatalog.GetRelicSprite(relicId) : null;
            return sprite ?? PrototypeSpriteFallbackLibrary.GetRelicSprite(relicId);
        }

        private string BuildRelicSlotLabel(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return "+";
            }

            if (displayName.Length <= 3)
            {
                return displayName;
            }

            return $"{displayName.Substring(0, 2)}\n{displayName.Substring(2, 1)}";
        }

        private string FormatRelicDetailDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return "효과 정보가 없습니다.";
            }

            string formatted = description.Trim();
            formatted = formatted.Replace(", ", ",\n");
            formatted = formatted.Replace(" 공격력 ", "\n공격력 ");
            formatted = formatted.Replace(" 회피율 ", "\n회피율 ");
            formatted = formatted.Replace(" 최대 체력 ", "\n최대 체력 ");
            formatted = formatted.Replace(" 현재 체력 ", "\n현재 체력 ");
            formatted = formatted.Replace(" 추가 피해 ", "\n추가 피해 ");
            formatted = formatted.Replace(" 공격 실패 시 ", "\n공격 실패 시 ");
            formatted = formatted.Replace(" 회피 성공 시 ", "\n회피 성공 시 ");
            formatted = formatted.Replace(" 피해 감소 성공 시 ", "\n피해 감소 성공 시 ");

            while (formatted.Contains("\n\n"))
            {
                formatted = formatted.Replace("\n\n", "\n");
            }

            return formatted;
        }

        private void OnRelicSlotSelected(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= gameSession.OwnedRelics.Count)
            {
                return;
            }

            RelicDefinition relic = gameSession.OwnedRelics[slotIndex];
            if (relic == null)
            {
                return;
            }

            ShowRelicDetailOverlay(relic, true);
        }

        private void OnCodexRelicSelected(int codexIndex)
        {
            if (contentLibrary == null || codexIndex < 0 || codexIndex >= contentLibrary.Relics.Count)
            {
                return;
            }

            RelicDefinition relic = contentLibrary.Relics[codexIndex];
            ShowRelicDetailOverlay(relic, IsRelicDiscovered(relic));
        }

        private void UpdateCodexEntries()
        {
            if (contentLibrary == null)
            {
                return;
            }

            IReadOnlyList<RelicDefinition> relics = contentLibrary.Relics;
            for (int index = 0; index < codexEntryButtons.Count; index++)
            {
                if (index >= relics.Count)
                {
                    codexEntryButtons[index].gameObject.SetActive(false);
                    continue;
                }

                RelicDefinition relic = relics[index];
                bool isDiscovered = IsRelicDiscovered(relic);
                codexEntryButtons[index].gameObject.SetActive(true);
                codexEntryImages[index].sprite = GetRelicSprite(relic.Id);
                codexEntryImages[index].color = codexEntryImages[index].sprite == null
                    ? new Color32(0, 0, 0, 0)
                    : (isDiscovered ? Color.white : new Color32(0, 0, 0, 255));
                codexEntryTexts[index].text = isDiscovered ? relic.DisplayName : "?";
            }
        }

        private void UpdateLogbookEntries()
        {
            List<ExperimentRecord> records = experimentLogService != null
                ? experimentLogService.GetRecords()
                : new List<ExperimentRecord>();

            logEntryRecords.Clear();
            bool hasRecords = records.Count > 0;
            if (logbookEmptyText != null)
            {
                logbookEmptyText.gameObject.SetActive(hasRecords == false);
            }

            for (int index = 0; index < logEntryButtons.Count; index++)
            {
                GameObject rootObject = logEntryButtons[index].transform.parent.gameObject;
                if (index >= records.Count)
                {
                    rootObject.SetActive(false);
                    continue;
                }

                ExperimentRecord record = records[records.Count - 1 - index];
                logEntryRecords.Add(record);
                rootObject.SetActive(true);
                Image entryImage = logEntryButtons[index].GetComponent<Image>();
                Color normalColor = record.Succeeded ? new Color32(54, 146, 72, 255) : new Color32(232, 42, 42, 255);
                Color highlightedColor = record.Succeeded ? new Color32(79, 170, 96, 255) : new Color32(242, 73, 73, 255);
                Color pressedColor = record.Succeeded ? new Color32(38, 118, 58, 255) : new Color32(201, 31, 31, 255);
                entryImage.color = normalColor;
                SetButtonColors(logEntryButtons[index], normalColor, highlightedColor, pressedColor);
                logEntryTitleTexts[index].text = $"실험 #{record.ExperimentNumber:000}";
                logEntryTitleTexts[index].color = Color.black;
                logEntryResultTexts[index].text = record.Succeeded ? "성공" : "실패";
                logEntryResultTexts[index].color = Color.black;
            }
        }

        private void OnLogEntrySelected(int entryIndex)
        {
            if (entryIndex < 0 || entryIndex >= logEntryRecords.Count)
            {
                return;
            }

            activeLogDetailRecord = logEntryRecords[entryIndex];
            if (activeLogDetailRecord == null || logbookDetailOverlayRoot == null)
            {
                return;
            }

            logDetailTitleText.text = $"실험 #{activeLogDetailRecord.ExperimentNumber:000}";
            logDetailSummaryText.text = BuildLogDetailSummaryText(activeLogDetailRecord);
            logDetailCardsText.text = BuildLogDetailCardEffectsText(activeLogDetailRecord);
            UpdateLogDetailRelicButtons(activeLogDetailRecord);
            logbookDetailOverlayRoot.SetActive(true);
            logbookDetailOverlayRoot.transform.SetAsLastSibling();
        }

        private void CloseLogDetailOverlay()
        {
            activeLogDetailRecord = null;
            if (logbookDetailOverlayRoot != null)
            {
                logbookDetailOverlayRoot.SetActive(false);
            }
        }

        private void OnLogDetailRelicSelected(int relicIndex)
        {
            if (activeLogDetailRecord == null)
            {
                return;
            }

            RelicDefinition relic = ResolveRecordRelic(activeLogDetailRecord, relicIndex);
            if (relic == null)
            {
                return;
            }

            ShowRelicDetailOverlay(relic, true);
        }

        private void UpdateLogDetailRelicButtons(ExperimentRecord record)
        {
            for (int index = 0; index < logDetailRelicButtons.Count; index++)
            {
                RelicDefinition relic = ResolveRecordRelic(record, index);
                bool hasRelic = relic != null;
                logDetailRelicButtons[index].gameObject.SetActive(hasRelic);
                logDetailRelicButtons[index].interactable = hasRelic;
                if (hasRelic == false)
                {
                    continue;
                }

                Sprite relicSprite = GetRelicSprite(relic.Id);
                logDetailRelicImages[index].sprite = relicSprite;
                logDetailRelicImages[index].color = relicSprite == null ? new Color32(0, 0, 0, 0) : Color.white;
                logDetailRelicTexts[index].text = relicSprite == null ? BuildRelicSlotLabel(relic.DisplayName) : string.Empty;
            }
        }

        private RelicDefinition ResolveRecordRelic(ExperimentRecord record, int relicIndex)
        {
            if (record == null || contentLibrary == null || relicIndex < 0)
            {
                return null;
            }

            if (relicIndex < record.AcquiredRelicIds.Count)
            {
                string relicId = record.AcquiredRelicIds[relicIndex];
                if (string.IsNullOrWhiteSpace(relicId) == false)
                {
                    return FindRelicDefinitionById(relicId);
                }
            }

            if (relicIndex < record.AcquiredRelics.Count)
            {
                string relicName = record.AcquiredRelics[relicIndex];
                if (string.IsNullOrWhiteSpace(relicName) == false)
                {
                    return FindRelicDefinitionByName(relicName);
                }
            }

            return null;
        }

        private RelicDefinition FindRelicDefinitionById(string relicId)
        {
            if (contentLibrary == null || string.IsNullOrWhiteSpace(relicId))
            {
                return null;
            }

            IReadOnlyList<RelicDefinition> relics = contentLibrary.Relics;
            for (int index = 0; index < relics.Count; index++)
            {
                if (relics[index].Id == relicId)
                {
                    return relics[index];
                }
            }

            return null;
        }

        private RelicDefinition FindRelicDefinitionByName(string displayName)
        {
            if (contentLibrary == null || string.IsNullOrWhiteSpace(displayName))
            {
                return null;
            }

            IReadOnlyList<RelicDefinition> relics = contentLibrary.Relics;
            for (int index = 0; index < relics.Count; index++)
            {
                if (relics[index].DisplayName == displayName)
                {
                    return relics[index];
                }
            }

            return null;
        }

        private string BuildLogDetailSummaryText(ExperimentRecord record)
        {
            StringBuilder builder = new StringBuilder();
            if (record.Succeeded)
            {
                builder.AppendLine("실험 성공");
                builder.Append($"보스 스테이지까지 돌파했습니다.");
                return builder.ToString();
            }

            builder.AppendLine("실험 실패");
            builder.Append($"{record.ReachedStage}번째 스테이지에서 종료되었습니다.");
            return builder.ToString();
        }

        private string BuildLogDetailCardEffectsText(ExperimentRecord record)
        {
            List<string> cardEffects = record != null ? record.SelectedCardEffects : null;
            if (cardEffects == null || cardEffects.Count == 0)
            {
                cardEffects = record != null ? record.SelectedCards : null;
            }

            if (cardEffects == null || cardEffects.Count == 0)
            {
                return "기록된 카드 정보가 없습니다.";
            }

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < cardEffects.Count; index++)
            {
                if (index > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("- ");
                builder.Append(FormatCardButtonText(cardEffects[index]));
            }

            return builder.ToString();
        }

        private void ApplyLogbookEntryOffset(RectTransform rectTransform, int index, float shadowOffset)
        {
            float[] xOffsets = { 0f, 96f, 34f, -26f, 72f, 8f };
            float xOffset = xOffsets[index % xOffsets.Length] + shadowOffset;
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = new Vector2(30f + xOffset, 18f);
            rectTransform.offsetMax = new Vector2(-220f + xOffset, -18f);
        }

        private bool IsRelicDiscovered(RelicDefinition relic)
        {
            if (relic == null)
            {
                return false;
            }

            if (gameSession != null)
            {
                for (int index = 0; index < gameSession.OwnedRelics.Count; index++)
                {
                    if (gameSession.OwnedRelics[index] != null && gameSession.OwnedRelics[index].Id == relic.Id)
                    {
                        return true;
                    }
                }
            }

            return experimentLogService != null && experimentLogService.IsRelicDiscovered(relic.Id);
        }

        private string BuildDraftPromptText()
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return "1단계: 공격 카드 3장 중 1장을 선택하세요.";
                case DraftPhase.Utility:
                    return "2단계: 유틸 카드 3장 중 1장을 선택하세요.";
                case DraftPhase.Attribute:
                    return "3단계: 속성 카드 3장 중 1장을 선택하세요.";
                default:
                    return "카드 선택이 완료되었습니다.";
            }
        }

        private string BuildDraftOverlayTitle()
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return "공격 카드 선택";
                case DraftPhase.Utility:
                    return "유틸 카드 선택";
                case DraftPhase.Attribute:
                    return "속성 카드 선택";
                default:
                    return "카드 선택";
            }
        }

        private void ApplyDraftVisualTheme()
        {
            for (int index = 0; index < optionButtons.Length; index++)
            {
                Color normalColor = GetDraftCardColor(index);
                Color highlightedColor = GetDraftCardHighlightColor(index);
                Color pressedColor = GetDraftCardPressedColor(index);
                optionButtonImages[index].color = normalColor;
                SetButtonColors(
                    optionButtons[index],
                    normalColor,
                    highlightedColor,
                    pressedColor);
            }
        }

        private Color GetDraftAccentColor()
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return new Color32(255, 198, 95, 255);
                case DraftPhase.Utility:
                    return new Color32(117, 218, 164, 255);
                case DraftPhase.Attribute:
                    return new Color32(164, 150, 255, 255);
                default:
                    return new Color32(255, 217, 120, 255);
            }
        }

        private Color GetDraftPanelColor()
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return new Color32(47, 40, 31, 250);
                case DraftPhase.Utility:
                    return new Color32(29, 45, 40, 250);
                case DraftPhase.Attribute:
                    return new Color32(37, 34, 57, 250);
                default:
                    return new Color32(29, 35, 50, 250);
            }
        }

        private Color GetDraftCardColor(int index)
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return index % 2 == 0 ? new Color32(108, 77, 34, 255) : new Color32(123, 89, 43, 255);
                case DraftPhase.Utility:
                    return index % 2 == 0 ? new Color32(45, 99, 74, 255) : new Color32(52, 112, 83, 255);
                case DraftPhase.Attribute:
                    return index % 2 == 0 ? new Color32(73, 62, 128, 255) : new Color32(86, 72, 144, 255);
                default:
                    return new Color32(63, 76, 102, 255);
            }
        }

        private Color GetDraftCardHighlightColor(int index)
        {
            Color color = GetDraftCardColor(index);
            return new Color(color.r + 0.08f, color.g + 0.08f, color.b + 0.08f, 1f);
        }

        private Color GetDraftCardPressedColor(int index)
        {
            Color color = GetDraftCardColor(index);
            return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, 1f);
        }

        private int GetDraftStepIndex()
        {
            switch (gameSession.DraftPhase)
            {
                case DraftPhase.Attack:
                    return 1;
                case DraftPhase.Utility:
                    return 2;
                case DraftPhase.Attribute:
                    return 3;
                default:
                    return 3;
            }
        }

        private string BuildCardButtonText(CardDefinition definition)
        {
            return FormatCardButtonText(GetCardSelectionText(definition));
        }

        private string GetCardSelectionText(CardDefinition definition)
        {
            if (definition.Category == CardCategory.Attribute)
            {
                return $"{definition.DisplayName} 속성";
            }

            return definition.Description;
        }

        private string FormatCardButtonText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            int firstSpaceIndex = text.IndexOf(' ');
            if (firstSpaceIndex <= 0 || firstSpaceIndex >= text.Length - 1)
            {
                return text;
            }

            string leadingToken = text.Substring(0, firstSpaceIndex).Trim();
            string remainder = text.Substring(firstSpaceIndex + 1).Trim();
            if (string.IsNullOrWhiteSpace(leadingToken) || string.IsNullOrWhiteSpace(remainder))
            {
                return text;
            }

            return $"{leadingToken}\n{remainder}";
        }

        private string BuildSelectedCardsPanelText()
        {
            StringBuilder builder = new StringBuilder();
            AppendSelectedEffectLine(builder, gameSession.SelectedAttackCard, "공격 카드 선택 전");
            AppendSelectedEffectLine(builder, gameSession.SelectedUtilityCard, "유틸 카드 선택 전");
            AppendSelectedEffectLine(builder, gameSession.SelectedAttributeCard, "속성 카드 선택 전");

            return builder.ToString();
        }

        private void AppendSelectedEffectLine(StringBuilder builder, CardDefinition definition, string emptyLabel)
        {
            if (definition == null)
            {
                builder.AppendLine(emptyLabel);
                return;
            }

            builder.AppendLine(GetCardSelectionText(definition));
        }

        private string BuildEnemyInfoPanelText()
        {
            if (screenController.CurrentScreenState == ScreenState.Draft)
            {
                return "적 정보 대기\n\n카드 조합이 끝나면\n실험 대상이 결정됩니다.";
            }

            if (gameSession.CurrentEncounter != null)
            {
                return BuildEncounterEffectText(gameSession.CurrentEncounter);
            }

            if (gameSession.LastBattleReport != null)
            {
                return BuildBattleEffectText(gameSession.LastBattleReport.FinalMonsterState);
            }

            return "아직 대상이 정해지지 않았습니다.";
        }

        private string BuildEncounterEffectText(MonsterEncounter encounter)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{encounter.Rule.MinAttack}~{encounter.Rule.MaxAttack} 랜덤 공격");
            builder.AppendLine(BuildMonsterUtilityEffectText(encounter.UtilityType, encounter.Rule.IsBoss));
            builder.Append(monsterFactory.GetAttributeLabel(encounter.AttributeType));
            builder.Append(" 속성");
            return builder.ToString();
        }

        private string BuildEncounterDetailText(MonsterEncounter encounter)
        {
            return $"체력: {encounter.Rule.Health}\n{BuildEncounterEffectText(encounter)}";
        }

        private string BuildRollingMonsterPreviewText(MonsterEncounter encounter, string attributeLabel, string utilityLabel)
        {
            return $"{encounter.Rule.MinAttack}~{encounter.Rule.MaxAttack} 랜덤 공격\n{utilityLabel}\n{attributeLabel} 속성";
        }

        private string BuildBattleEffectText(BattleUnitState unitState)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{unitState.MinAttack}~{unitState.MaxAttack} 랜덤 공격");
            builder.AppendLine(BuildUnitUtilityEffectText(unitState));
            builder.Append(monsterFactory.GetAttributeLabel(unitState.AttributeType));
            builder.Append(" 속성");
            return builder.ToString();
        }

        private string BuildMonsterUtilityEffectText(MonsterUtilityType utilityType, bool isBoss)
        {
            string label = monsterFactory.GetMonsterUtilityLabel(utilityType, isBoss);
            if (label == "없음")
            {
                return "특징 없음";
            }

            return label;
        }

        private string BuildUnitUtilityEffectText(BattleUnitState unitState)
        {
            if (unitState.ExecuteChance > 0f)
            {
                return $"즉사 공격 {unitState.ExecuteChance:0}%";
            }

            if (unitState.DodgeChance > 0f)
            {
                return $"{unitState.DodgeChance:0}% 확률 회피";
            }

            if (unitState.DamageReductionChance > 0f)
            {
                return $"{unitState.DamageReductionChance:0}% 확률 피해 감소";
            }

            if (unitState.ExtraAttackChance > 0f)
            {
                return $"{unitState.ExtraAttackChance:0}% 확률 추가 공격";
            }

            return "특징 없음";
        }

        private BattleUnitState BuildPlayerPreviewState()
        {
            if (gameSession.BuildResult == null)
            {
                return new BattleUnitState
                {
                    IsPlayer = true,
                    DisplayName = "슬라임",
                    AttributeType = AttributeType.Neutral,
                    MaxHealth = 20,
                    CurrentHealth = 20,
                    MinAttack = 0,
                    MaxAttack = 0
                };
            }

            return RelicEffectResolver.BuildPlayerState(
                gameSession.BuildResult,
                gameSession.OwnedRelics,
                gameSession.PersistentCurrentHealth,
                gameSession.PiggyBankStacks,
                gameSession.IsBossStage());
        }

        private void UpdateBattleStagePresentation()
        {
            BattleUnitState playerState = gameSession.LastBattleReport != null && screenController.CurrentScreenState == ScreenState.Result
                ? gameSession.LastBattleReport.FinalPlayerState
                : BuildPlayerPreviewState();
            displayedPlayerHealth = playerState.CurrentHealth;
            SetHealthBarState(playerHealthFillImage, playerHealthValueText, playerState.CurrentHealth, playerState.MaxHealth, true);

            if (gameSession.CurrentEncounter != null)
            {
                int enemyCurrentHealth = gameSession.LastBattleReport != null && screenController.CurrentScreenState != ScreenState.Roulette
                    ? gameSession.LastBattleReport.FinalMonsterState.CurrentHealth
                    : gameSession.CurrentEncounter.Rule.Health;
                int enemyMaxHealth = gameSession.LastBattleReport != null && screenController.CurrentScreenState != ScreenState.Roulette
                    ? gameSession.LastBattleReport.FinalMonsterState.MaxHealth
                    : gameSession.CurrentEncounter.Rule.Health;
                displayedEnemyHealth = enemyCurrentHealth;
                SetHealthBarState(enemyHealthFillImage, enemyHealthValueText, enemyCurrentHealth, enemyMaxHealth, true);
            }
            else
            {
                displayedEnemyHealth = 0;
                SetHealthBarState(enemyHealthFillImage, enemyHealthValueText, 0, 0, false);
            }
        }

        private void SetHealthBarState(Image fillImage, Text valueText, int current, int max, bool isVisible)
        {
            fillImage.transform.parent.gameObject.SetActive(isVisible);
            if (isVisible == false || max <= 0)
            {
                SetHealthBarFillWidth(fillImage, 0f);
                valueText.text = string.Empty;
                return;
            }

            SetHealthBarFillWidth(fillImage, Mathf.Clamp01((float)current / max));
            valueText.text = $"{current}/{max}";
        }

        private IEnumerator PlayBattleEventPresentation(BattleTurnEvent battleEvent, BattleReport report)
        {
            if (battleEvent.EventType == BattleEventType.TurnStart)
            {
                centerStateText.text = battleEvent.Message;
                yield return new WaitForSeconds(0.18f);
                yield break;
            }

            centerStateText.text = battleEvent.Message;

            if (battleEvent.EventType == BattleEventType.Damage)
            {
                yield return StartCoroutine(AnimateAttackExchange(battleEvent, report));
                yield break;
            }

            if (battleEvent.EventType == BattleEventType.Dodge)
            {
                yield return StartCoroutine(AnimateAttackMotion(battleEvent.AttackerIsPlayer));
                yield return StartCoroutine(AnimateDodgeReaction(battleEvent.DefenderIsPlayer));
                yield break;
            }

            if (battleEvent.EventType == BattleEventType.Reduction)
            {
                yield return StartCoroutine(AnimateReductionReaction(battleEvent.AttackerIsPlayer));
                yield break;
            }

            if (battleEvent.EventType == BattleEventType.Defeat)
            {
                yield return StartCoroutine(AnimateDefeatReaction(battleEvent.DefenderIsPlayer));
                yield break;
            }

            yield return new WaitForSeconds(0.14f);
        }

        private IEnumerator AnimateAttackExchange(BattleTurnEvent battleEvent, BattleReport report)
        {
            yield return StartCoroutine(AnimateAttackMotion(battleEvent.AttackerIsPlayer));
            yield return StartCoroutine(AnimateHitReaction(battleEvent.DefenderIsPlayer));

            if (battleEvent.DefenderIsPlayer)
            {
                yield return StartCoroutine(AnimateHealthBarTo(
                    playerHealthFillImage,
                    playerHealthValueText,
                    displayedPlayerHealth,
                    battleEvent.PlayerHealthAfter,
                    report.FinalPlayerState.MaxHealth));
                displayedPlayerHealth = battleEvent.PlayerHealthAfter;
            }
            else
            {
                yield return StartCoroutine(AnimateHealthBarTo(
                    enemyHealthFillImage,
                    enemyHealthValueText,
                    displayedEnemyHealth,
                    battleEvent.MonsterHealthAfter,
                    report.FinalMonsterState.MaxHealth));
                displayedEnemyHealth = battleEvent.MonsterHealthAfter;
            }

            yield return new WaitForSeconds(0.08f);
        }

        private IEnumerator AnimateAttackMotion(bool attackerIsPlayer)
        {
            RectTransform actorRect = attackerIsPlayer ? playerActorVisualRootRect : enemyActorVisualRootRect;
            Vector2 originalPosition = actorRect.anchoredPosition;
            Vector2 attackOffset = attackerIsPlayer ? new Vector2(36f, 0f) : new Vector2(-36f, 0f);

            if (attackerIsPlayer)
            {
                RectTransform handRect = playerHandImage.rectTransform;
                Vector2 handOriginal = handRect.anchoredPosition;
                yield return StartCoroutine(MoveRectTransform(handRect, handOriginal, handOriginal + new Vector2(28f, -10f), 0.08f));
                yield return StartCoroutine(MoveRectTransform(actorRect, originalPosition, originalPosition + attackOffset, 0.08f));
                yield return StartCoroutine(MoveRectTransform(handRect, handOriginal + new Vector2(28f, -10f), handOriginal, 0.08f));
                yield return StartCoroutine(MoveRectTransform(actorRect, originalPosition + attackOffset, originalPosition, 0.08f));
                yield break;
            }

            yield return StartCoroutine(MoveRectTransform(actorRect, originalPosition, originalPosition + attackOffset, 0.1f));
            yield return StartCoroutine(MoveRectTransform(actorRect, originalPosition + attackOffset, originalPosition, 0.1f));
        }

        private IEnumerator AnimateHitReaction(bool defenderIsPlayer)
        {
            Image targetImage = defenderIsPlayer ? playerPortraitImage : enemyPortraitImage;
            RectTransform targetRect = defenderIsPlayer ? playerActorVisualRootRect : enemyActorVisualRootRect;
            Vector2 originalPosition = targetRect.anchoredPosition;
            Color originalColor = targetImage.color;

            targetImage.color = new Color32(255, 170, 170, 255);
            yield return StartCoroutine(MoveRectTransform(targetRect, originalPosition, originalPosition + new Vector2(defenderIsPlayer ? -16f : 16f, 0f), 0.05f));
            yield return StartCoroutine(MoveRectTransform(targetRect, originalPosition + new Vector2(defenderIsPlayer ? -16f : 16f, 0f), originalPosition, 0.07f));
            targetImage.color = originalColor;
        }

        private IEnumerator AnimateDodgeReaction(bool defenderIsPlayer)
        {
            RectTransform targetRect = defenderIsPlayer ? playerActorVisualRootRect : enemyActorVisualRootRect;
            Vector2 originalPosition = targetRect.anchoredPosition;
            yield return StartCoroutine(MoveRectTransform(targetRect, originalPosition, originalPosition + new Vector2(0f, 18f), 0.06f));
            yield return StartCoroutine(MoveRectTransform(targetRect, originalPosition + new Vector2(0f, 18f), originalPosition, 0.06f));
        }

        private IEnumerator AnimateReductionReaction(bool defenderIsPlayer)
        {
            Image targetImage = defenderIsPlayer ? playerPortraitImage : enemyPortraitImage;
            Color originalColor = targetImage.color;
            targetImage.color = new Color32(180, 225, 255, 255);
            yield return new WaitForSeconds(0.12f);
            targetImage.color = originalColor;
        }

        private IEnumerator AnimateDefeatReaction(bool defeatedIsPlayer)
        {
            Image targetImage = defeatedIsPlayer ? playerPortraitImage : enemyPortraitImage;
            Color originalColor = targetImage.color;
            targetImage.color = new Color32(120, 120, 120, 255);
            yield return new WaitForSeconds(0.18f);
            targetImage.color = originalColor;
        }

        private void ResetBattleActorPresentation()
        {
            if (playerActorVisualRootRect != null)
            {
                playerActorVisualRootRect.anchoredPosition = Vector2.zero;
            }

            if (enemyActorVisualRootRect != null)
            {
                enemyActorVisualRootRect.anchoredPosition = Vector2.zero;
            }

            if (playerHandImage != null)
            {
                playerHandImage.rectTransform.anchoredPosition = Vector2.zero;
                playerHandImage.color = playerHandImage.sprite == null ? new Color32(0, 0, 0, 0) : Color.white;
            }

            if (playerPortraitImage != null)
            {
                playerPortraitImage.color = playerPortraitImage.sprite == null ? new Color32(0, 0, 0, 0) : Color.white;
            }

            if (enemyPortraitImage != null)
            {
                enemyPortraitImage.color = enemyPortraitImage.sprite == null ? new Color32(0, 0, 0, 0) : Color.white;
            }
        }

        private IEnumerator AnimateHealthBarTo(Image fillImage, Text valueText, int fromHealth, int toHealth, int maxHealth)
        {
            float elapsed = 0f;
            const float duration = 0.18f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                int currentHealth = Mathf.RoundToInt(Mathf.Lerp(fromHealth, toHealth, t));
                SetHealthBarFillWidth(fillImage, maxHealth <= 0 ? 0f : Mathf.Clamp01((float)currentHealth / maxHealth));
                valueText.text = $"{currentHealth}/{maxHealth}";
                yield return null;
            }

            SetHealthBarFillWidth(fillImage, maxHealth <= 0 ? 0f : Mathf.Clamp01((float)toHealth / maxHealth));
            valueText.text = $"{toHealth}/{maxHealth}";
        }

        private void SetHealthBarFillWidth(Image fillImage, float ratio)
        {
            RectTransform fillRect = fillImage.rectTransform;
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
            fillRect.offsetMin = new Vector2(0f, 4f);
            fillRect.offsetMax = new Vector2(0f, -4f);
        }

        private IEnumerator MoveRectTransform(RectTransform rectTransform, Vector2 startPosition, Vector2 endPosition, float duration)
        {
            float elapsed = 0f;
            rectTransform.anchoredPosition = startPosition;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            rectTransform.anchoredPosition = endPosition;
        }

        private void AppendNamedLines(StringBuilder builder, List<string> names)
        {
            if (names.Count == 0)
            {
                builder.AppendLine("- 없음");
                return;
            }

            for (int index = 0; index < names.Count; index++)
            {
                builder.Append("- ");
                builder.AppendLine(names[index]);
            }
        }

        private void HideOptionButtons()
        {
            for (int index = 0; index < optionButtons.Length; index++)
            {
                optionButtons[index].gameObject.SetActive(false);
            }
        }

        private void SetActionButton(string label, UnityAction action, bool isVisible)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButtonContainer.SetActive(true);
            actionButtonText.text = string.IsNullOrWhiteSpace(label) ? "대기 중" : label;
            actionButton.interactable = isVisible && action != null;
            if (actionButton.interactable)
            {
                actionButton.onClick.AddListener(action);
            }
        }

        private void ResetRunViewForNewExperiment()
        {
            CloseDetailOverlay();
            StopAllCoroutines();
            ResetRewardOverlayState();
            currentChoices.Clear();
            HideOptionButtons();
            choiceRowContainer.SetActive(false);
            playerPortraitText.text = "SLIME";
            enemyPortraitText.text = "???";
            playerInfoText.text = string.Empty;
            monsterInfoText.text = "적 정보 대기";
            selectedCardsText.text = "공격 카드 선택 전\n유틸 카드 선택 전\n속성 카드 선택 전";
            SetCurrentDetailLog("전투 로그", "아직 전투 로그가 없습니다.");
            ApplyNeutralPortraitSprites();
            UpdatePersistentPanels();
            SetActionButton("다음 단계 대기", null, false);
        }

        private void ResetRewardOverlayState()
        {
            hiddenRewardSlotIndex = -1;

            if (rewardOverlayRoot != null)
            {
                rewardOverlayRoot.SetActive(false);
            }

            if (rewardOverlayCanvasGroup != null)
            {
                rewardOverlayCanvasGroup.alpha = 0f;
            }

            if (rewardOverlayCardRect != null)
            {
                rewardOverlayCardRect.anchoredPosition = Vector2.zero;
                rewardOverlayCardRect.localScale = Vector3.one;
            }

            if (rewardOverlayIconImage != null)
            {
                rewardOverlayIconImage.sprite = null;
                rewardOverlayIconImage.color = new Color32(0, 0, 0, 0);
            }

            if (rewardOverlayLabelText != null)
            {
                rewardOverlayLabelText.text = string.Empty;
            }

            for (int index = 0; index < relicSlotIconImages.Count; index++)
            {
                relicSlotIconImages[index].enabled = true;
                relicSlotTexts[index].enabled = true;
            }
        }

        private void SetCurrentDetailLog(string title, string content)
        {
            currentDetailLogTitle = string.IsNullOrWhiteSpace(title) ? "전투 로그" : title;
            currentDetailLogContent = string.IsNullOrWhiteSpace(content) ? "아직 전투 로그가 없습니다." : content;

            if (detailOverlayRoot.activeSelf && isDetailOverlayShowingRelic == false)
            {
                detailOverlayTitleText.text = currentDetailLogTitle;
                stateInfoText.text = currentDetailLogContent;
            }
        }

        private void OpenDetailOverlay()
        {
            isDetailOverlayShowingRelic = false;
            if (detailOverlayRelicRoot != null)
            {
                detailOverlayRelicRoot.SetActive(false);
            }

            if (detailOverlayNameText != null)
            {
                detailOverlayNameText.text = string.Empty;
            }

            if (detailOverlayRelicImage != null)
            {
                detailOverlayRelicImage.sprite = null;
                detailOverlayRelicImage.color = new Color32(0, 0, 0, 0);
            }

            stateInfoText.alignment = TextAnchor.UpperLeft;
            stateInfoText.fontSize = 24;
            stateInfoText.resizeTextMinSize = 18;
            stateInfoText.resizeTextMaxSize = 24;
            Stretch(stateInfoText.rectTransform, 20f);
            detailOverlayTitleText.text = currentDetailLogTitle;
            stateInfoText.text = currentDetailLogContent;
            detailOverlayCanvasGroup.alpha = 1f;
            detailOverlayRoot.SetActive(true);
            detailOverlayRoot.transform.SetAsLastSibling();
        }

        private void ShowRelicDetailOverlay(RelicDefinition relic, bool isDiscovered)
        {
            if (relic == null)
            {
                return;
            }

            isDetailOverlayShowingRelic = true;
            detailOverlayTitleText.text = "변수 정보";
            if (detailOverlayRelicRoot != null)
            {
                detailOverlayRelicRoot.SetActive(true);
            }

            if (detailOverlayNameText != null)
            {
                detailOverlayNameText.text = isDiscovered ? relic.DisplayName : "?";
            }

            if (detailOverlayRelicImage != null)
            {
                detailOverlayRelicImage.sprite = GetRelicSprite(relic.Id);
                if (detailOverlayRelicImage.sprite == null)
                {
                    detailOverlayRelicImage.color = new Color32(0, 0, 0, 0);
                }
                else
                {
                    detailOverlayRelicImage.color = isDiscovered ? Color.white : new Color32(0, 0, 0, 255);
                }
            }

            stateInfoText.alignment = TextAnchor.UpperCenter;
            stateInfoText.fontSize = 48;
            stateInfoText.resizeTextMinSize = 28;
            stateInfoText.resizeTextMaxSize = 48;
            stateInfoText.rectTransform.anchorMin = new Vector2(0.08f, 0.08f);
            stateInfoText.rectTransform.anchorMax = new Vector2(0.92f, 0.3f);
            stateInfoText.rectTransform.offsetMin = Vector2.zero;
            stateInfoText.rectTransform.offsetMax = Vector2.zero;
            stateInfoText.text = isDiscovered
                ? FormatRelicDetailDescription(relic.Description)
                : "아직 발견하지 못한 변수입니다.";
            detailOverlayCanvasGroup.alpha = 1f;
            detailOverlayRoot.SetActive(true);
            detailOverlayRoot.transform.SetAsLastSibling();
        }

        private void CloseDetailOverlay()
        {
            isDetailOverlayShowingRelic = false;
            detailOverlayRoot.SetActive(false);
        }

        private void SetResultTouchOverlay(bool isVisible, string label)
        {
            resultTouchOverlayButton.gameObject.SetActive(isVisible);
            if (isVisible == false)
            {
                return;
            }

            resultTouchOverlayText.text = label;
            resultTouchOverlayButton.transform.SetAsLastSibling();
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;
            canvasGroup.alpha = startAlpha;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }

        private IEnumerator AnimateCardIn(Button button)
        {
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            IEnumerator fadeRoutine = FadeCanvasGroup(canvasGroup, 0f, 1f, 0.12f);
            IEnumerator scaleRoutine = ScaleRectTransform(rectTransform, new Vector3(0.9f, 0.9f, 1f), Vector3.one, 0.14f);
            yield return StartCoroutine(RunAnimationsTogether(fadeRoutine, scaleRoutine));
        }

        private IEnumerator HideChoiceButtonsTogether()
        {
            IEnumerator fadeOverlay = FadeCanvasGroup(draftOverlayCanvasGroup, draftOverlayCanvasGroup.alpha, 0f, 0.12f);
            IEnumerator scaleCards = ScaleRectTransform(draftOverlayCardsRowRect, Vector3.one, new Vector3(0.92f, 0.92f, 1f), 0.12f);
            IEnumerator[] cardAnimations = new IEnumerator[optionButtons.Length];

            for (int index = 0; index < optionButtons.Length; index++)
            {
                CanvasGroup canvasGroup = optionButtons[index].GetComponent<CanvasGroup>();
                RectTransform rectTransform = optionButtons[index].GetComponent<RectTransform>();
                cardAnimations[index] = RunAnimationsTogether(
                    FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, 0.1f),
                    ScaleRectTransform(rectTransform, rectTransform.localScale, new Vector3(0.88f, 0.88f, 1f), 0.1f));
            }

            yield return StartCoroutine(RunAnimationsTogether(CombineAnimations(fadeOverlay, scaleCards, cardAnimations)));
        }

        private IEnumerator RunAnimationsTogether(params IEnumerator[] animations)
        {
            List<Coroutine> routines = new List<Coroutine>();
            for (int index = 0; index < animations.Length; index++)
            {
                if (animations[index] != null)
                {
                    routines.Add(StartCoroutine(animations[index]));
                }
            }

            for (int index = 0; index < routines.Count; index++)
            {
                yield return routines[index];
            }
        }

        private IEnumerator[] CombineAnimations(IEnumerator firstAnimation, IEnumerator secondAnimation, IEnumerator[] otherAnimations)
        {
            List<IEnumerator> animations = new List<IEnumerator>
            {
                firstAnimation,
                secondAnimation
            };

            for (int index = 0; index < otherAnimations.Length; index++)
            {
                animations.Add(otherAnimations[index]);
            }

            return animations.ToArray();
        }

        private IEnumerator ScaleRectTransform(RectTransform rectTransform, Vector3 startScale, Vector3 endScale, float duration)
        {
            float elapsed = 0f;
            rectTransform.localScale = startScale;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, normalized);
                yield return null;
            }

            rectTransform.localScale = endScale;
        }

        private void EnsureEventSystem()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            if (inputModule.actionsAsset == null)
            {
                inputModule.AssignDefaultActions();
            }
        }

        private RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            Image image = panelObject.AddComponent<Image>();
            image.color = color;
            return panelObject.GetComponent<RectTransform>();
        }

        private RectTransform CreateHealthBar(string name, Transform parent, TextAnchor textAnchor, out Image fillImage, out Text valueText)
        {
            RectTransform root = CreatePanel(name, parent, new Color32(32, 36, 45, 255));
            AddOutline(root.gameObject, new Color32(0, 0, 0, 140), new Vector2(2f, -2f));

            RectTransform fillRect = CreatePanel($"{name}Fill", root, new Color32(222, 71, 77, 255));
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = new Vector2(0f, 4f);
            fillRect.offsetMax = new Vector2(0f, -4f);
            fillImage = fillRect.GetComponent<Image>();
            fillImage.type = Image.Type.Simple;
            SetHealthBarFillWidth(fillImage, 1f);

            valueText = CreateText($"{name}ValueText", root, 24, textAnchor);
            valueText.fontStyle = FontStyle.Bold;
            Stretch(valueText.rectTransform, 10f);
            return root;
        }

        private RectTransform CreateSpacer(Transform parent, float preferredHeight)
        {
            RectTransform spacer = CreatePanel("Spacer", parent, new Color32(0, 0, 0, 0));
            LayoutElement layoutElement = spacer.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredHeight;
            return spacer;
        }

        private void CreateMenuTile(Transform parent, string name, string label, bool isActive, Color color)
        {
            RectTransform tile = CreatePanel(name, parent, color);
            AddOutline(tile.gameObject, new Color32(255, 255, 255, 18), new Vector2(3f, -3f));
            Text labelText = CreateText($"{name}Label", tile, 34, TextAnchor.MiddleCenter);
            labelText.fontStyle = FontStyle.Bold;
            labelText.text = label;
            labelText.color = isActive ? Color.white : new Color32(179, 188, 206, 255);
            Stretch(labelText.rectTransform, 16f);
        }

        private Text CreateText(string name, Transform parent, int fontSize, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private RectTransform CreateButton(string name, Transform parent, out Button button, out Text label)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color32(91, 127, 196, 255);

            button = buttonObject.AddComponent<Button>();
            SetButtonColors(button, new Color32(91, 127, 196, 255), new Color32(110, 146, 220, 255), new Color32(65, 99, 171, 255));

            label = CreateText($"{name}Label", buttonObject.transform, 24, TextAnchor.MiddleCenter);
            Stretch(label.rectTransform, 16f);
            return buttonObject.GetComponent<RectTransform>();
        }

        private void SetButtonColors(Button button, Color normal, Color highlighted, Color pressed)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = normal;
            colors.highlightedColor = highlighted;
            colors.pressedColor = pressed;
            colors.selectedColor = highlighted;
            colors.disabledColor = new Color(normal.r * 0.5f, normal.g * 0.5f, normal.b * 0.5f, 0.8f);
            button.colors = colors;
        }

        private void AddOutline(GameObject target, Color effectColor, Vector2 distance)
        {
            Outline outline = target.AddComponent<Outline>();
            outline.effectColor = effectColor;
            outline.effectDistance = distance;
        }

        private void Stretch(RectTransform rectTransform, float padding)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(padding, padding);
            rectTransform.offsetMax = new Vector2(-padding, -padding);
        }
    }
}
