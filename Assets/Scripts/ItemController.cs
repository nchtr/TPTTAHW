using UnityEngine;

public class ItemController : MonoBehaviour
{
    public enum ItemType { Power, BigPower, Point, Bomb }
    public ItemType itemType;

    private const float FallSpeed = 1.8f;
    private const float MagnetRadius = 0.45f;
    private const float AutoCollectY = 3.8f;

    public static ItemController SpawnPowerItem(Vector3 pos) =>
        Spawn(pos, ItemType.Power,    new Color(0.3f, 0.5f, 1f),  0.28f, 14);

    public static ItemController SpawnBigPowerItem(Vector3 pos) =>
        Spawn(pos, ItemType.BigPower, new Color(0.1f, 0.2f, 1f),  0.50f, 22);

    public static ItemController SpawnPointItem(Vector3 pos) =>
        Spawn(pos, ItemType.Point,    new Color(1f, 0.5f, 0.8f),  0.28f, 14);

    public static ItemController SpawnBombItem(Vector3 pos) =>
        Spawn(pos, ItemType.Bomb,     new Color(0.1f, 1f, 0.4f),  0.38f, 18);

    static ItemController Spawn(Vector3 pos, ItemType type, Color color, float scale, int texSize)
    {
        var go = new GameObject(type + "Item");
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * scale;
        go.tag = "Item";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = type == ItemType.Bomb
            ? PlaceholderGraphics.CreateCircle(color, texSize)
            : PlaceholderGraphics.CreateDiamond(color, texSize);
        sr.sortingOrder = 2;

        var item = go.AddComponent<ItemController>();
        item.itemType = type;
        return item;
    }

    void Update()
    {
        transform.position += Vector3.down * FallSpeed * Time.deltaTime;
        if (transform.position.y < -7.5f) { Destroy(gameObject); return; }

        var player = PlayerController.Instance;
        if (player == null) return;

        bool autoCollect = player.transform.position.y > AutoCollectY;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (autoCollect || dist < MagnetRadius)
        {
            float pullSpeed = autoCollect ? 14f : 8f;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, pullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.15f)
                Collect();
        }
    }

    void Collect()
    {
        switch (itemType)
        {
            case ItemType.Power:    GameManager.Instance.AddPower(GameManager.SmallPowerAmount); break;
            case ItemType.BigPower: GameManager.Instance.AddPower(GameManager.LargePowerAmount); break;
            case ItemType.Point:    GameManager.Instance.AddScore(GameManager.PointItemScore);   break;
            case ItemType.Bomb:     GameManager.Instance.AddBomb();                              break;
        }
        Destroy(gameObject);
    }
}
