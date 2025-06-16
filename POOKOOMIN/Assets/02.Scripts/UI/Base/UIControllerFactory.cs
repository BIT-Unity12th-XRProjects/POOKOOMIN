using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pookoomin.UI
{

    /// <summary>
    /// View에 따른 Controller 생성 팩토리 클래스
    /// (tk) DI 구현 전까지는 해당 클래스로 컨트롤러 생성
    /// </summary>
    public static class UIControllerFactory 
    {
        private static Dictionary<UIView, object> controllerDict = new Dictionary<UIView, object>();

        public static TController GetOrCreateController<TController, TView, TModel>(TView view, TModel model)
        where TController : UIController<TView, TModel>
        where TView : UIView
        where TModel : class
        {
            if (controllerDict.TryGetValue(view, out var existing))
                return (TController)existing;

            var controller = (TController)Activator.CreateInstance(typeof(TController), view, model);
            controllerDict[view] = controller;
            return controller;
        }

        public static void RemoveController(UIView view)
        {
            controllerDict.Remove(view);
        }
    }
}

