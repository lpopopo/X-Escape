using UnityEngine;

/// <summary>
/// 最简单的测试脚本，用于验证脚本系统是否工作
/// </summary>
public class SimpleDebugTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("SimpleDebugTest: Awake 被调用！");
    }

    private void Start()
    {
        Debug.Log("SimpleDebugTest: Start 被调用！");
        Debug.Log($"SimpleDebugTest: 游戏对象名称: {gameObject.name}");
    }

    private void Update()
    {
        if (Time.frameCount == 1)
        {
            Debug.Log("SimpleDebugTest: Update 第一次被调用！");
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SimpleDebugTest: 按下了空格键！");
        }
    }
}
