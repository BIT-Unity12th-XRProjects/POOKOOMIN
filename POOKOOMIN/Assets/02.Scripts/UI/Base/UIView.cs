using UnityEngine;

namespace Pookoomin.UI
{
    /// <summary>
    /// MVC ÆÐÅÏ : Model -> View
    /// </summary>
    public class UIView : MonoBehaviour
    {
        public virtual void Initialize(Transform anchor)
        {
            transform.SetParent(anchor);

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.offsetMin = Vector3.zero;
            rectTransform.offsetMax = Vector3.zero;
        }

        public virtual void CloseUI(bool isCloseAll = false)
        {
            if (isCloseAll == false)
            {
                //OnClose?.Invoke();
            }

            //OnClose = null;
            UIManager.instance.CloseUI(this);
        }
    }
}

