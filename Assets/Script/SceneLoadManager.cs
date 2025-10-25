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
        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _bridge = FindAnyObjectByType<CoherenceBridge>();
            if (firstSceneToLoad.Length > 0 && loadFisrtScene)
                LoadRegularScene(firstSceneToLoad, false);

        }
        private void Start() {
            
            // Giữ bridge khi load scene mới
            DontDestroyOnLoad(_bridge.gameObject);

            // Cho CoherenceSync biết bridge này là default
            CoherenceSync.BridgeResolve += _ => _bridge;

            // Gắn callback khi load scene mới
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == "LoadingScene") return;

            // Thông báo cho bridge rằng client đã ở scene mới
            _bridge.SceneManager.SetClientScene(scene.buildIndex);

            // Cho bridge spawn entity trong scene mới
            Debug.Log(scene.name);
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

        public void SubscribeOnNetworkEvents()
        {
            // //On host prepared scene to load
            // NetworkManager.Singleton.SceneManager.OnSynchronize += (clientId) =>
            // {
            //     //Works on client side only
            //     if (NetworkManager.Singleton.LocalClientId == clientId)
            //         SceneManager.LoadScene("LoadingScene");

            // };

            // //On host loading scene
            // NetworkManager.Singleton.SceneManager.OnLoad += (clientId, sceneName, mode, sceneLoadOperation) =>
            // {
            //     StartCoroutine(ProcessNetworkSceneLoading(sceneLoadOperation));
            // };
        }

        public void LoadNetworkScene(string sceneName)
        {
            //Switch to loading scene first
            //SceneManager.LoadScene("LoadingScene");

            //SubscribeOnNetworkEvents();
            // NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
            // NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void LoadRegularScene(string sceneName, bool useLoadScreen = true)
        {
            Debug.Log("Loading scene: " + sceneName);
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
