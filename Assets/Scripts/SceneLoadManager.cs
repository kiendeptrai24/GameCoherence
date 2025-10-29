using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Coherence.Toolkit;
using System;
using UnityEngine.EventSystems;
namespace Coherence.Samples.Kien
{
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        [SerializeField]
        private string firstSceneToLoad = "MenuUI";
        public bool loadFisrtScene = true;
        private CoherenceBridge _bridge;
        private string curretnScene;
        protected override void Awake()
        {
            base.Awake();
            _bridge = FindAnyObjectByType<CoherenceBridge>();
            if (firstSceneToLoad.Length > 0 && loadFisrtScene)
                LoadRegularScene(firstSceneToLoad, false);

        }
        private void Start()
        {
            DontDestroyOnLoad(_bridge.gameObject);

            CoherenceSync.BridgeResolve += _ => _bridge;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == "LoadingScene") return;

            // Thông báo cho bridge rằng client đã ở scene mới
            _bridge.SceneManager.SetClientScene(scene.buildIndex);

            // Cho bridge spawn entity trong scene mới
            _bridge.InstantiationScene = scene;

            // Đặt scene mới làm active sau 1 frame
            StartCoroutine(SetActiveSceneNextFrame(scene));
        }
        
        private IEnumerator SetActiveSceneNextFrame(Scene scene)
        {
            yield return null; // đợi 1 frame để scene load hoàn toàn
            if (scene.IsValid() && scene.isLoaded && scene.name != "DontDestroyOnLoad")
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log($"✅ Active Scene: {SceneManager.GetActiveScene().name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Scene {scene.name} không hợp lệ để set active.");
            }
        }
        public void LoadRegularScene(string sceneName, bool useLoadScreen = true)
        {
            if (sceneName == curretnScene)
                return;
            StartCoroutine(ProcessRegularSceneLoading(sceneName, useLoadScreen));
        }

        private IEnumerator ProcessNetworkSceneLoading(AsyncOperation asyncOperation)
        {
            yield return asyncOperation;

            SceneManager.UnloadSceneAsync("LoadingScene");
        }

        private IEnumerator ProcessRegularSceneLoading(string sceneToLoad, bool useLoadScene = true)
        {
            if (useLoadScene)
            {
                SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
                yield return new WaitForSeconds(1f);
                SceneManager.UnloadSceneAsync("LoadingScene");
            }
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }
}
