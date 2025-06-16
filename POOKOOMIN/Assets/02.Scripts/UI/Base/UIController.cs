using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Pookoomin.UI
{
    /// <summary>
    /// MVC 패턴 : UIController(Controller -> Model)
    /// </summary>
    public class UIController<View, Model>  
        where View : UIView
        where Model : class
    {
        protected Model model;
        protected View view;

        public UIController(View view, Model model)
        {
            this.view = view;
            this.model = model;
        }

        /// <summary>
        /// View Input를 바인드 하기
        /// </summary>
        public virtual void BindInputEvents() { }

        /// <summary>
        /// Model의 값 변경 시 이벤트 바인드
        /// </summary>
        public virtual void BindModelToView() { }
    }
}
