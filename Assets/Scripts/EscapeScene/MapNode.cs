using UnityEngine;

namespace XEscape.EscapeScene
{
    /// <summary>
    /// 地图节点，代表一个可到达的地点
    /// </summary>
    public class MapNode : MonoBehaviour
    {
        [Header("节点信息")]
        [SerializeField] private string nodeName;
        [SerializeField] private NodeType nodeType;

        [Header("连接节点")]
        [SerializeField] private MapNode[] connectedNodes;

        [Header("节点状态")]
        private bool isVisited = false;
        private bool isVisible = false;

        [Header("视觉效果")]
        [SerializeField] private SpriteRenderer nodeRenderer;
        [SerializeField] private Color visitedColor = Color.gray;
        [SerializeField] private Color unvisitedColor = Color.white;
        [SerializeField] private Color hiddenColor = Color.black;

        private void Start()
        {
            UpdateVisualState();
        }

        /// <summary>
        /// 设置节点可见性
        /// </summary>
        public void SetVisible(bool visible)
        {
            isVisible = visible;
            UpdateVisualState();
        }

        /// <summary>
        /// 访问节点
        /// </summary>
        public void Visit()
        {
            isVisited = true;
            isVisible = true;
            UpdateVisualState();
        }

        /// <summary>
        /// 更新视觉效果
        /// </summary>
        private void UpdateVisualState()
        {
            if (nodeRenderer == null)
                return;

            if (!isVisible)
            {
                nodeRenderer.color = hiddenColor;
            }
            else if (isVisited)
            {
                nodeRenderer.color = visitedColor;
            }
            else
            {
                nodeRenderer.color = unvisitedColor;
            }
        }

        /// <summary>
        /// 点击节点时调用
        /// </summary>
        private void OnMouseDown()
        {
            if (isVisible && !isVisited)
            {
                MapManager.Instance?.SelectNode(this);
            }
        }

        /// <summary>
        /// 获取节点名称
        /// </summary>
        public string GetNodeName()
        {
            return nodeName;
        }

        /// <summary>
        /// 获取节点类型
        /// </summary>
        public NodeType GetNodeType()
        {
            return nodeType;
        }

        /// <summary>
        /// 获取连接的节点
        /// </summary>
        public MapNode[] GetConnectedNodes()
        {
            return connectedNodes;
        }

        /// <summary>
        /// 是否已访问
        /// </summary>
        public bool IsVisited()
        {
            return isVisited;
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible()
        {
            return isVisible;
        }

        /// <summary>
        /// 设置连接的节点
        /// </summary>
        public void SetConnectedNodes(MapNode[] nodes)
        {
            connectedNodes = nodes;
        }
    }

    /// <summary>
    /// 节点类型枚举
    /// </summary>
    public enum NodeType
    {
        Town,       // 城镇（可以搜寻物资）
        Road,       // 道路
        Border,     // 边境（胜利条件）
        Danger      // 危险区域
    }
}

