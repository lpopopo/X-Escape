using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace XEscape.EscapeScene
{
    /// <summary>
    /// 地图管理器，负责生成随机地图和管理节点
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [Header("地图设置")]
        [SerializeField] private int mapWidth = 10;
        [SerializeField] private int mapHeight = 10;
        [SerializeField] private int totalNodes = 20;
        [SerializeField] private int visibleNodeRange = 2; // 可见节点范围

        [Header("节点预制体")]
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private Transform nodeParent;

        [Header("节点类型概率")]
        [SerializeField] private float townProbability = 0.3f;
        [SerializeField] private float borderProbability = 0.1f;
        [SerializeField] private float dangerProbability = 0.2f;

        private List<MapNode> allNodes = new List<MapNode>();
        private MapNode currentNode;
        private MapNode startNode;
        private MapNode borderNode;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            GenerateRandomMap();
            InitializeMap();
        }

        /// <summary>
        /// 生成随机地图
        /// </summary>
        private void GenerateRandomMap()
        {
            allNodes.Clear();

            // 创建节点
            for (int i = 0; i < totalNodes; i++)
            {
                Vector3 position = GetRandomPosition();
                GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity, nodeParent);
                MapNode node = nodeObj.GetComponent<MapNode>();

                if (node == null)
                {
                    node = nodeObj.AddComponent<MapNode>();
                }

                // 随机分配节点类型
                NodeType nodeType = GetRandomNodeType();
                if (i == 0)
                {
                    // 第一个节点作为起点
                    startNode = node;
                    currentNode = node;
                }
                else if (i == totalNodes - 1)
                {
                    // 最后一个节点作为边境
                    borderNode = node;
                    nodeType = NodeType.Border;
                }

                allNodes.Add(node);
            }

            // 连接节点（创建路径）
            ConnectNodes();
        }

        /// <summary>
        /// 获取随机位置
        /// </summary>
        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(-mapWidth / 2f, mapWidth / 2f);
            float y = Random.Range(-mapHeight / 2f, mapHeight / 2f);
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// 获取随机节点类型
        /// </summary>
        private NodeType GetRandomNodeType()
        {
            float rand = Random.Range(0f, 1f);
            
            if (rand < borderProbability)
                return NodeType.Border;
            else if (rand < borderProbability + dangerProbability)
                return NodeType.Danger;
            else if (rand < borderProbability + dangerProbability + townProbability)
                return NodeType.Town;
            else
                return NodeType.Road;
        }

        /// <summary>
        /// 连接节点
        /// </summary>
        private void ConnectNodes()
        {
            // 简单的连接策略：每个节点连接到最近的几个节点
            for (int i = 0; i < allNodes.Count; i++)
            {
                MapNode currentNode = allNodes[i];
                List<MapNode> nearbyNodes = allNodes
                    .Where(n => n != currentNode)
                    .OrderBy(n => Vector3.Distance(currentNode.transform.position, n.transform.position))
                    .Take(3) // 每个节点连接3个最近的节点
                    .ToList();

                // 设置连接的节点
                currentNode.SetConnectedNodes(nearbyNodes.ToArray());
            }
        }

        /// <summary>
        /// 初始化地图
        /// </summary>
        private void InitializeMap()
        {
            if (startNode != null)
            {
                startNode.Visit();
                UpdateVisibleNodes();
            }
        }

        /// <summary>
        /// 更新可见节点
        /// </summary>
        private void UpdateVisibleNodes()
        {
            foreach (MapNode node in allNodes)
            {
                if (node == currentNode)
                {
                    node.SetVisible(true);
                    continue;
                }

                // 检查节点是否在可见范围内
                float distance = Vector3.Distance(currentNode.transform.position, node.transform.position);
                bool isVisible = distance <= visibleNodeRange || node.IsVisited();
                
                node.SetVisible(isVisible);
            }
        }

        /// <summary>
        /// 选择节点（移动到该节点）
        /// </summary>
        public void SelectNode(MapNode node)
        {
            if (node == null || node.IsVisited())
                return;

            // 检查节点是否可达
            if (!IsNodeReachable(node))
            {
                Debug.Log("节点不可达！");
                return;
            }

            // 移动到新节点
            currentNode = node;
            node.Visit();

            // 处理节点事件
            HandleNodeEvent(node);

            // 更新可见节点
            UpdateVisibleNodes();

            // 检查游戏状态
            CheckGameState(node);
        }

        /// <summary>
        /// 检查节点是否可达
        /// </summary>
        private bool IsNodeReachable(MapNode node)
        {
            // 检查是否在当前节点的连接节点中
            if (currentNode != null && currentNode.GetConnectedNodes() != null)
            {
                return currentNode.GetConnectedNodes().Contains(node);
            }
            return false;
        }

        /// <summary>
        /// 处理节点事件
        /// </summary>
        private void HandleNodeEvent(MapNode node)
        {
            switch (node.GetNodeType())
            {
                case NodeType.Town:
                    // 城镇：可以搜寻物资
                    TownManager.Instance?.OpenTownMenu();
                    break;
                case NodeType.Danger:
                    // 危险区域：可能损失资源
                    HandleDangerEvent();
                    break;
                case NodeType.Border:
                    // 边境：胜利条件
                    GameManager.Instance?.CheckVictory();
                    break;
            }
        }

        /// <summary>
        /// 处理危险事件
        /// </summary>
        private void HandleDangerEvent()
        {
            if (GameManager.Instance?.resourceManager != null)
            {
                // 随机损失资源
                float staminaLoss = Random.Range(10f, 30f);
                float fuelLoss = Random.Range(15f, 25f);
                
                GameManager.Instance.resourceManager.ReduceStamina(staminaLoss);
                GameManager.Instance.resourceManager.ReduceFuel(fuelLoss);
            }
        }

        /// <summary>
        /// 检查游戏状态
        /// </summary>
        private void CheckGameState(MapNode node)
        {
            if (node.GetNodeType() == NodeType.Border)
            {
                GameManager.Instance?.CheckVictory();
            }
            else
            {
                GameManager.Instance?.CheckGameOver();
            }
        }

        /// <summary>
        /// 获取当前节点
        /// </summary>
        public MapNode GetCurrentNode()
        {
            return currentNode;
        }
    }
}

