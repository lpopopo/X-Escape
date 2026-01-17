# 修复 "The referenced script (Unknown) is missing!" 错误

## 问题说明

这个错误表示场景中某个 GameObject 引用了已删除或丢失的脚本。

## 解决方法

### 方法 1：找到并删除丢失的脚本引用

1. 在 Unity Console 中点击错误信息
2. Unity 会自动选中包含丢失脚本的 GameObject
3. 在 Inspector 中，你会看到灰色的 "Missing (Script)" 组件
4. 点击该组件右侧的齿轮图标 → `Remove Component`
5. 或者直接点击组件右上角的 `×` 删除

### 方法 2：批量查找丢失的脚本

1. 在 Hierarchy 中，点击搜索框
2. 输入 `t:Missing` 或 `t:MissingScript`
3. 所有包含丢失脚本的 GameObject 会被高亮显示
4. 逐个选中并删除丢失的脚本组件

### 方法 3：使用脚本批量清理（可选）

如果有很多丢失的脚本，可以创建一个编辑器脚本来批量清理：

```csharp
// Assets/Scripts/Editor/CleanMissingScripts.cs
using UnityEngine;
using UnityEditor;

public class CleanMissingScripts : Editor
{
    [MenuItem("Tools/清理丢失的脚本")]
    public static void Clean()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    DestroyImmediate(comp);
                    count++;
                }
            }
        }
        
        Debug.Log($"清理了 {count} 个丢失的脚本引用");
    }
}
```

## 预防措施

1. **删除脚本前**：确保场景中没有 GameObject 使用该脚本
2. **重命名脚本**：Unity 会自动更新引用
3. **移动脚本**：保持脚本在 Assets 文件夹中，不要移到外部

## 检查清单

- [ ] 已找到包含丢失脚本的 GameObject
- [ ] 已删除丢失的脚本组件
- [ ] Console 中不再显示错误
- [ ] 场景已保存（Ctrl+S 或 Cmd+S）
