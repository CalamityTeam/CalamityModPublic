using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YinYo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yin-Yo");
            Tooltip.SetDefault("Fires light or dark shards when enemies are near\n" +
                "Shards fly back and forth\n" +
                "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 50;
            Item.knockBack = 3.5f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<YinYoyo>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DarkShard).
                AddIngredient(ItemID.LightShard).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
