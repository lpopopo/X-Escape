using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XEscape.PickupScene
{
    /// <summary>
    /// 物资生成器 - 随机从上方扔出物资
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        [Header("生成设置")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnHeight = 10f;
        [SerializeField] private float spawnRangeX = 8f;

        [Header("物资类型")]
        [SerializeField] private ItemType[] availableItemTypes = { ItemType.Food, ItemType.Fuel, ItemType.Medicine };

        private bool isSpawning = false;

        private void Start()
        {
            // 检查 itemPrefab 是否已分配
            if (itemPrefab == null)
            {
                itemPrefab = TryLoadItemPrefab();
                
                if (itemPrefab == null)
                {
                    Debug.LogError("[ItemSpawner] itemPrefab 未分配！请在 Inspector 中分配 ItemPrefab 预制体。");
                    Debug.LogError("[ItemSpawner] 预制体路径应该是: Assets/Prefabs/ItemPrefab.prefab");
                    return;
                }
            }

            StartSpawning();
        }

        /// <summary>
        /// 尝试加载 ItemPrefab，优先从 Resources 加载，编辑器中也可以从 Prefabs 文件夹加载
        /// </summary>
        private GameObject TryLoadItemPrefab()
        {
            // 方法1: 尝试从 Resources 加载（运行时和编辑器都可用）
            GameObject loadedPrefab = Resources.Load<GameObject>("ItemPrefab");
            if (loadedPrefab != null)
            {
                Debug.Log("[ItemSpawner] 已从 Resources 自动加载 ItemPrefab");
                return loadedPrefab;
            }

#if UNITY_EDITOR
            // 方法2: 在编辑器中，尝试从 Prefabs 文件夹直接加载
            loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemPrefab.prefab");
            if (loadedPrefab != null)
            {
                Debug.Log("[ItemSpawner] 已从 Prefabs 文件夹自动加载 ItemPrefab（仅编辑器模式）");
                return loadedPrefab;
            }
#endif

            return null;
        }

        public void StartSpawning()
        {
            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnItems());
            }
        }

        public void StopSpawning()
        {
            isSpawning = false;
            StopAllCoroutines();
        }

        private IEnumerator SpawnItems()
        {
            while (isSpawning)
            {
                SpawnItem();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnItem()
        {
            // 检查 itemPrefab 是否已分配
            if (itemPrefab == null)
            {
                Debug.LogError("[ItemSpawner] itemPrefab 未分配！请在 Inspector 中分配 ItemPrefab 预制体。");
                StopSpawning();
                return;
            }

            // 随机生成位置
            float randomX = Random.Range(-spawnRangeX / 2, spawnRangeX / 2);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);

            // 创建物资实例
            GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // 设置物资类型
            PickupItem pickupItem = item.GetComponent<PickupItem>();
            if (pickupItem != null)
            {
                ItemType randomType = availableItemTypes[Random.Range(0, availableItemTypes.Length)];
                pickupItem.Initialize(randomType);
            }
            else
            {
                Debug.LogWarning("[ItemSpawner] 实例化的物品上没有 PickupItem 组件！");
            }
        }

        private void OnDrawGizmos()
        {
            // 在编辑器中绘制生成区域
            Gizmos.color = Color.yellow;
            Vector3 leftPoint = new Vector3(-spawnRangeX / 2, spawnHeight, 0);
            Vector3 rightPoint = new Vector3(spawnRangeX / 2, spawnHeight, 0);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }
    }
}
