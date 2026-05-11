using UnityEngine;

// rarity tag for a loot pickup
// пока только для цвета, дальше можно к этому привязать эффекты
public enum LootRarity
{
    Common,
    Uncommon,
    Rare
}

// tiny script on the loot prefab — just colors the sprite by rarity
public class LootItem : MonoBehaviour
{
    public SpriteRenderer sprite; // drag SpriteRenderer of the loot prefab
    Sprite fallbackSprite;

    // colors per rarity, tweak in inspector if you want
    public Color commonColor = new Color(0.85f, 0.85f, 0.85f);
    public Color uncommonColor = new Color(0.40f, 0.70f, 1.00f);
    public Color rareColor = new Color(1.00f, 0.80f, 0.20f);

    public LootRarity rarity;
    [SerializeField] HerbType herbType;

    [Header("Herb Sprites")]
    [SerializeField] Sprite bellLeafSprite;
    [SerializeField] Sprite lavenderFernSprite;
    [SerializeField] Sprite buttonRootSprite;
    [SerializeField] Sprite honeyCloverSprite;
    [SerializeField] Sprite warmNettleSprite;
    [SerializeField] Sprite sleepGrassSprite;
    [SerializeField] Sprite glowberrySprite;

    void Awake()
    {
        if (sprite != null)
            fallbackSprite = sprite.sprite;
    }

    public void SetRarity(LootRarity r)
    {
        rarity = r;
        if (sprite != null)
            ApplyRarityColor();
    }

    public void SetHerbType(HerbType type)
    {
        herbType = type;
        RefreshVisual();
    }

    public HerbType GetHerbType()
    {
        return herbType;
    }

    void RefreshVisual()
    {
        if (sprite == null) return;

        Sprite herbSprite = GetSpriteForHerbType(herbType);
        if (herbSprite != null)
        {
            sprite.sprite = herbSprite;
            sprite.color = Color.white;
            return;
        }

        if (fallbackSprite != null)
            sprite.sprite = fallbackSprite;

        ApplyRarityColor();
    }

    Sprite GetSpriteForHerbType(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
                return bellLeafSprite;
            case HerbType.LavenderFern:
                return lavenderFernSprite;
            case HerbType.ButtonRoot:
                return buttonRootSprite;
            case HerbType.HoneyClover:
                return honeyCloverSprite;
            case HerbType.WarmNettle:
                return warmNettleSprite;
            case HerbType.SleepGrass:
                return sleepGrassSprite;
            case HerbType.Glowberry:
                return glowberrySprite;
            default:
                return null;
        }
    }

    void ApplyRarityColor()
    {
        if (rarity == LootRarity.Rare)
            sprite.color = rareColor;
        else if (rarity == LootRarity.Uncommon)
            sprite.color = uncommonColor;
        else
            sprite.color = commonColor;
    }

}
