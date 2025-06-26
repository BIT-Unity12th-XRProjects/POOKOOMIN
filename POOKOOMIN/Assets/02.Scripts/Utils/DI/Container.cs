using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodyGo.Utils.DI
{
    public struct DIData
    {
        public Type type;
        public string key;
        public object data;
        public string containterKey;

        public DIData(Type type, object data, string key = null)
        {
            this.type = type;
            this.data = data;
            this.key = key;
            containterKey = type.Name + "_" + key.ToString();
        }

        public string GetKey()
        {
            return containterKey;
        }
    }

    public class Container
    {
        public Container()
        {
            _registration = new Dictionary<string, DIData>();
        }


        Dictionary<string, DIData> _registration;

        /// <summary>
        /// 생성자가 있는 일반 C# 클래스 등록 (생성해서 추가함)
        /// </summary>
        public void Register<T>(string key = null)
            where T : class, new()
        {
            T obj = new T();
            DIData data = new DIData(typeof(T), obj, key);
            _registration[data.GetKey()] = data;
        }

        /// <summary>
        /// Monobehaviour 객체를 생성해서 추가
        /// </summary>
        public void RegisterMonobehaviour<T>(string key = null)
            where T : MonoBehaviour
        {
            T obj = new GameObject(typeof(T).Name).AddComponent<T>();
            DIData data = new DIData(typeof(T), obj, key);
            _registration[data.GetKey()] = data;
        }

        /// <summary>
        /// Hierarchy 에 존재하는 객체를 추가
        /// </summary>
        public void RegisterMonobehaviour(MonoBehaviour monobehaviour, string key = null)
        {
            DIData data = new DIData(monobehaviour.GetType(), monobehaviour, key);
            _registration[data.GetKey()] = data;
        }

        /// <summary>
        /// 등록된거 가져옴
        /// </summary>
        public T Resolve<T>(string key = null)
        {
            string diKey = typeof(T).Name + "_" + key;
            return (T)_registration[diKey].data;
        }

        public object Resolve(Type type, string key = null)
        {
            string diKey = type.Name + "_" + key;
            if (_registration.TryGetValue(diKey, out DIData obj))
                return obj.data;
            else
                return null;
        }
    }
}