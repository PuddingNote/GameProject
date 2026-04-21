using System.Collections;
using NUnit.Framework;
using SlimeExperiment.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SlimeExperiment.Tests.PlayMode
{
    public sealed class PrototypeUiFlowPlayModeTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            GameBootstrap[] bootstraps = Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None);
            for (int index = 0; index < bootstraps.Length; index++)
            {
                Object.Destroy(bootstraps[index].gameObject);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator StartButtonMovesToDraftSelectionScreen()
        {
            yield return SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameObject primaryButtonObject = GameObject.Find("PrimaryButton");
            Assert.IsNotNull(primaryButtonObject);

            Button primaryButton = primaryButtonObject.GetComponent<Button>();
            Assert.IsNotNull(primaryButton);
            Assert.IsTrue(primaryButton.gameObject.activeInHierarchy);

            Text primaryButtonLabel = GameObject.Find("PrimaryButtonLabel").GetComponent<Text>();
            Assert.AreEqual("실험 시작", primaryButtonLabel.text);

            primaryButton.onClick.Invoke();
            yield return null;

            Text subHeaderLabel = GameObject.Find("SubHeaderText").GetComponent<Text>();
            Assert.IsTrue(subHeaderLabel.text.Contains("카드"));

            GameObject draftOverlay = GameObject.Find("DraftOverlay");
            Assert.IsNotNull(draftOverlay);
            Assert.IsTrue(draftOverlay.activeInHierarchy);

            yield return new WaitForSeconds(0.5f);

            GameObject optionButton = GameObject.Find("OptionButton0");
            Assert.IsNotNull(optionButton);
            Assert.IsTrue(optionButton.activeInHierarchy);

            GameObject actionButton = GameObject.Find("ActionButton");
            Assert.IsNotNull(actionButton);
            Assert.IsTrue(actionButton.activeInHierarchy);
            Assert.IsFalse(actionButton.GetComponent<Button>().interactable);

            GameObject enemyInfoPanel = GameObject.Find("EnemyInfoPanel");
            Assert.IsNotNull(enemyInfoPanel);
            Assert.IsTrue(enemyInfoPanel.activeInHierarchy);

            GameObject playerHealthBar = GameObject.Find("PlayerHealthBar");
            Assert.IsNotNull(playerHealthBar);
            Assert.IsTrue(playerHealthBar.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator CodexButtonMovesToCodexScreen()
        {
            yield return SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameObject codexButtonObject = GameObject.Find("CodexMenuButton");
            Assert.IsNotNull(codexButtonObject);

            Button codexButton = codexButtonObject.GetComponent<Button>();
            Assert.IsNotNull(codexButton);
            codexButton.onClick.Invoke();
            yield return null;

            GameObject codexScreen = GameObject.Find("CodexScreen");
            Assert.IsNotNull(codexScreen);
            Assert.IsTrue(codexScreen.activeInHierarchy);

            Text headerLabel = GameObject.Find("HeaderText").GetComponent<Text>();
            Assert.AreEqual("변수 도감", headerLabel.text);

            GameObject codexEntry = GameObject.Find("CodexEntryButton0");
            Assert.IsNotNull(codexEntry);
            Assert.IsTrue(codexEntry.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator LogbookButtonMovesToLogbookScreen()
        {
            yield return SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameObject logbookButtonObject = GameObject.Find("LogbookMenuButton");
            Assert.IsNotNull(logbookButtonObject);

            Button logbookButton = logbookButtonObject.GetComponent<Button>();
            Assert.IsNotNull(logbookButton);
            logbookButton.onClick.Invoke();
            yield return null;

            GameObject logbookScreen = GameObject.Find("LogbookScreen");
            Assert.IsNotNull(logbookScreen);
            Assert.IsTrue(logbookScreen.activeInHierarchy);

            Text headerLabel = GameObject.Find("HeaderText").GetComponent<Text>();
            Assert.AreEqual("실험 로그", headerLabel.text);

            GameObject emptyTextObject = GameObject.Find("LogbookEmptyText");
            GameObject firstEntryObject = GameObject.Find("LogbookEntryButton0");
            Assert.IsTrue(
                (emptyTextObject != null && emptyTextObject.activeInHierarchy)
                || (firstEntryObject != null && firstEntryObject.activeInHierarchy));
        }
    }
}
