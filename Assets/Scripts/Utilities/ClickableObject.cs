using UnityEngine;

namespace XEscape.Utilities
{
    /// <summary>
    /// 可点击对象基类
    /// </summary>
    public class ClickableObject : MonoBehaviour
    {
        [Header("点击设置")]
        [SerializeField] private bool highlightOnHover = true;
        [SerializeField] private Color highlightColor = Color.yellow;

        private Color originalColor;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        private void OnMouseEnter()
        {
            if (highlightOnHover && spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
            }
        }

        private void OnMouseExit()
        {
            if (highlightOnHover && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        private void OnMouseDown()
        {
            OnClicked();
        }

        /// <summary>
        /// 点击时调用，子类重写此方法
        /// </summary>
        protected virtual void OnClicked()
        {
            // 子类实现具体逻辑
        }
    }
}

