using UnityEngine;
using System.Collections;

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
            StartSpawning();
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
