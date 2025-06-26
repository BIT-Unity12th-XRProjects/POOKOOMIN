using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using FoodyGo.Singletons;
using FoodyGo.Controllers;
using FoodyGo.UI;
using Pookoomin.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Pookoomin.Manager;

namespace FoodyGo.Managers
{
    public enum GameState
    {
        None,
        Lobby,
        Walk,
        ARCamera
    }

    public class GameManager : Singleton<GameManager>
    {
        [Header("Game Scenes")]
        public string MapSceneName;
        public string SplashSceneName;
        public string CatchSceneName;

        [Header("Layer Names")]
        public string MonsterLayerName = "Monster";

        private Scene SplashScene;
        private Scene MapScene;
        private Scene CatchScene;

        #region Test
        //@tk : user Data 임시로 넣음. 관리
        public UserWorkData userWorkData;
        public DataLoader<UserWorkData, UserWorkDataRaw> userWorkDataLoader;
        #endregion

        private Stack<GameState> stateStack;
        private GameState currentState;

        public Action<GameState> onChangeGameState;

        [Header("Title Scene References")]
        [SerializeField] private InputActionReference _touchStartAction;
        private bool _hasStarted = false;
        [SerializeField] private GameObject _titleUICanvas;
        [SerializeField] private Camera _titleCamera;


        private void Awake()
        {
            //화면 고정
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;

            userWorkDataLoader = new DataLoader<UserWorkData, UserWorkDataRaw>(typeof(UserWorkData).Name + ".json");
            userWorkData = userWorkDataLoader.Load();
        }

        private void OnEnable()
        {
            if (_touchStartAction != null)
            {
                _touchStartAction.action.Enable();
                _touchStartAction.action.performed += OnTouchStart;
            }
        }

        private void OnDisable()
        {
            if (_touchStartAction != null)
            {
                _touchStartAction.action.performed -= OnTouchStart;
                _touchStartAction.action.Disable();
            }
        }

        // Use this for initialization
        private void Start()
        {
            SoundManager.instance.PlayBGM(BGM.Title, 0);
            currentState = GameState.None;

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            stateStack = new Stack<GameState>();

            GoogleFitUtil.RequestGoogleFitOAuth();
        }

        private void OnTouchStart(InputAction.CallbackContext context)
        {
            if (_hasStarted)
                return;
            
            _hasStarted = true;

            if (_titleUICanvas != null)
            {
                _titleUICanvas.SetActive(false);
            }

            if (_titleCamera != null)
            {
                _titleCamera.gameObject.SetActive(false);
            }

            StartCoroutine(LoadScenes());
        }

        private IEnumerator LoadScenes()
        {
            yield return SceneManager.LoadSceneAsync(SplashSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(MapSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(CatchSceneName, LoadSceneMode.Additive);
            ActiveAdditiveScene(MapSceneName);
            yield return SceneManager.UnloadSceneAsync(SplashSceneName);

            SoundManager.instance.PlayBGM(BGM.Main);

            stateStack.Push(GameState.Walk);
            ChangeGameState(GameState.Walk);

            if (_touchStartAction != null)
            {
                _touchStartAction.action.Disable();
            }
        }

        //run when a new scene is loaded
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode lsm)
        {
            if (scene.name == MapSceneName)
            {
                MapScene = scene;
            }
            else if (scene.name == CatchSceneName)
            {
                CatchScene = scene;
            }
        }

        public void ActiveAdditiveScene(string sceneName)
        {
            bool activeMapScene = sceneName.Equals(MapSceneName);
            bool activeCatchScene = sceneName.Equals(CatchSceneName);

            GameObject[] roots;

            roots = CatchScene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                root.SetActive(activeCatchScene);
            }

            roots = MapScene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                root.SetActive(activeMapScene);
            }
        }


        public void ChangeGameState(GameState state)
        {
            if (currentState == state) 
            {
                return;
            } 

            //@TK : UI작업 분리
            if(state == GameState.Walk)
            {
                Pookoomin.UI.UIManager.instance.OpenUI<UIUserWorkDataController, UIUserWorkDataView, UserWorkData>(userWorkData);
                Pookoomin.UI.UIManager.instance.OpenUI<UIInGameButtonsController, UIInGamebuttonsView, object>(null);
            }
            else if(state == GameState.ARCamera)
            {
                if (Pookoomin.UI.UIManager.instance.IsOpenedUI<UIUserWorkDataView>())
                {
                    var workDataUI = Pookoomin.UI.UIManager.instance.GetOpenedUI<UIUserWorkDataView>();
                    Pookoomin.UI.UIManager.instance.CloseUI(workDataUI);
                }
            }

            stateStack.Push(state);
            currentState = state;
            onChangeGameState.Invoke(currentState);
        }

        public void ChangeBackState()
        {
            if(stateStack.Count > 1)
            {
                stateStack.Pop();
                ChangeGameState(stateStack.Peek());
                onChangeGameState.Invoke(currentState);
            }
        }

        /// <summary>
        /// Checks if a relevant game object has been hit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool RegisterHitGameObject(PointerEventData data)
        {
            int mask = BuildLayerMask();
            Ray ray = Camera.main.ScreenPointToRay(data.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
            {
                print("Object hit " + hitInfo.collider.gameObject.name);
                var go = hitInfo.collider.gameObject;
                HandleHitGameObject(go);

                return true;
            }
            return false;
        }

        private void HandleHitGameObject(GameObject go)
        {
            if (go.GetComponent<MonsterController>() != null)
            {
                print("Monster hit, need to open catch scene ");
            }
        }

        private int BuildLayerMask()
        {
            return 1 << LayerMask.NameToLayer(MonsterLayerName);
        }
    }
}