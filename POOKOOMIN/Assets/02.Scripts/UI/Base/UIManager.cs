using FoodyGo.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Pookoomin.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public Transform UIOpenedTransform;
        public Transform UIClosedTransform;

        private UIView frontUI;

        private Dictionary<Type, UIView> OpenUIPool = new Dictionary<Type, UIView>();
        private Dictionary<Type, UIView> CloseUIPool = new Dictionary<Type, UIView>();

        private UIView GetUI<TView>(out bool isAlreadyOpen) where TView : UIView
        {
            Type uiType = typeof(TView);
            Debug.Log(uiType.Name);
            UIView ui = null;
            isAlreadyOpen = false;

            if (OpenUIPool.ContainsKey(uiType))
            {
                ui = OpenUIPool[uiType].GetComponent<UIView>();
                isAlreadyOpen = true;
            }
            else if (CloseUIPool.ContainsKey(uiType))
            {
                ui = CloseUIPool[uiType].GetComponent<UIView>();
                CloseUIPool.Remove(uiType);
            }
            else
            {
                string path = uiType.Name.Replace("View", "");
                GameObject obj = Resources.Load($"UI/{path}") as GameObject;
                var uiObj = Instantiate(obj);
                ui = uiObj.GetComponent<UIView>();
            }

            return ui;
        }

        public void OpenUI<TController, TView, TModel>(TModel model)
        where TController : UIController<TView, TModel>
        where TView : UIView
        where TModel : class
        {
            Type uiType = typeof(TView);
            bool isAlreadyOpen = false;
            TView ui = GetUI<TView>(out isAlreadyOpen) as TView;

            if (ui == null)
            {
                Debug.LogError($"{uiType} prefab dosen't exist in Resources");
                return;
            }

            if (isAlreadyOpen)
            {
                return;
            }

            //GetController 
            var controller = UIControllerFactory.GetOrCreateController<TController, TView, TModel>(ui, model);
            controller.BindInputEvents();
            controller.BindModelToView();

            var sibilingIndex = UIOpenedTransform.childCount;
            ui.Initialize(UIOpenedTransform);
            ui.transform.SetSiblingIndex(sibilingIndex);
            ui.gameObject.SetActive(true);
            //ui.SetInfo(uiData);
            //ui.OpenUI();

            frontUI = ui;
            OpenUIPool.Add(uiType, ui);

        }

        public void CloseUI(UIView ui)
        {
            Type uiType = ui.GetType();
            ui.gameObject.SetActive(false);
            OpenUIPool.Remove(uiType);
            CloseUIPool.Add(uiType, ui);
            ui.transform.SetParent(UIClosedTransform);

            UIControllerFactory.RemoveController(frontUI);

            frontUI = null;

            if (UIOpenedTransform.childCount > 0)
            {
                var lastChild = UIOpenedTransform.GetChild(UIOpenedTransform.childCount - 1);
                if (lastChild != null)
                {
                    frontUI = lastChild.GetComponent<UIView>();
                }
            }
        }

        public T GetOpenedUI<T>() where T : UIView
        {
            Type uiType = typeof(T);
            UIView ui = null;
            if (OpenUIPool.ContainsKey(uiType))
            {
                ui = OpenUIPool[uiType].GetComponent<UIView>();
            }
            return (T)ui;
        }
    }
}

