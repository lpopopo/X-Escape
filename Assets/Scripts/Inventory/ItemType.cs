namespace XEscape.Inventory
{
    /// <summary>
    /// 物品类型枚举
    /// </summary>
    public enum ItemType
    {
        Food,           // 食物
        Disguise,       // 伪装物品
        Tool,           // 工具
        Material,       // 材料
        Other           // 其他
    }

    /// <summary>
    /// 食物类型枚举
    /// </summary>
    public enum FoodType
    {
        Bread,          // 面包
        Water,          // 水
        CannedFood,     // 罐头
        Snack,          // 零食
        Meal            // 正餐
    }

    /// <summary>
    /// 伪装物品类型枚举
    /// </summary>
    public enum DisguiseType
    {
        Hat,            // 帽子
        Glasses,        // 眼镜
        Clothes,        // 衣服
        Mask,           // 面具
        Wig             // 假发
    }
}
