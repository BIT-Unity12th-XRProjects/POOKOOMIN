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
        #endregion

        private Stack<GameState> stateStack;
        private GameState currentState;

        public Action<GameState> onChangeGameState;

        // Use this for initialization
        IEnumerator Start()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            stateStack = new Stack<GameState>();

            yield return SceneManager.LoadSceneAsync(SplashSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(MapSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(CatchSceneName, LoadSceneMode.Additive);
            ActiveAdditiveScene(MapSceneName);
            yield return SceneManager.UnloadSceneAsync(SplashSceneName);
            stateStack.Push(GameState.Lobby);
            currentState = GameState.Lobby;
        }

        //run when a new scene is loaded
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode lsm)
        {
            if (scene.name == MapSceneName)
            {
                MapScene = scene;

                #region Test
                userWorkData = new UserWorkData();
                userWorkData.stepCount.Value = 0;
                userWorkData.coin.Value = 0;
                #endregion

                // Pookoomin.UI.UIManager.instance.OpenUI<UICameraButtonsController, UICameraButtonsView, object>(null);
                Pookoomin.UI.UIManager.instance.OpenUI<UIUserWorkDataController, UIUserWorkDataView, UserWorkData>(userWorkData);
                Pookoomin.UI.UIManager.instance.OpenUI<UIInGameButtonsController, UIInGamebuttonsView, object>(null);
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

            stateStack.Push(state);
            currentState = state;
            onChangeGameState.Invoke(currentState);
        }

        public void ChangeBackState()
        {
            if(stateStack.Count > 1)
            {
                stateStack.Pop();
                currentState = stateStack.Peek();
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