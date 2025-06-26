using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FoodyGo.Utils.DI
{
    public abstract class Scope : MonoBehaviour
    {
        protected Container container;


        [SerializeField] List<MonoBehaviour> _monobehaviours;


        protected virtual void Awake()
        {
            container = new Container();
            Register();
            InjectAll();
        }

        public virtual void Register()
        {
            foreach (var monoBehaviour in _monobehaviours)
            {
                container.RegisterMonobehaviour(monoBehaviour, monoBehaviour.gameObject.name);
            }
        }

        protected virtual void InjectAll()
        {
            MonoBehaviour[] monobehaviours = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (var monoBehaviour in monobehaviours)
            {
                Inject(monoBehaviour, monoBehaviour.gameObject.name);
            }
        }

        /// <summary>
        /// 의존성을 주입함
        /// </summary>
        /// <param name="target"> 주입할 대상</param>
        protected virtual void Inject(object target, string key = null)
        {
            Type type = target.GetType();

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var injectAttr = fieldInfo.GetCustomAttribute<InjectAttribute>();

                if (injectAttr != null)
                {
                    object value = null;
                    if (!string.IsNullOrEmpty(injectAttr.Key))
                        value = container.Resolve(fieldInfo.FieldType, injectAttr.Key);
                    else
                        value = container.Resolve(fieldInfo.FieldType);

                    fieldInfo.SetValue(target, value);
                }
            }
        }
    }
}