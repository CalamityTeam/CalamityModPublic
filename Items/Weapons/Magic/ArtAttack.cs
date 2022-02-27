using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArtAttack : ModItem
    {
        public const int MaxDamageBoostTime = 270;
        public const float MaxDamageBoostFactor = 2.3f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art Attack");
            Tooltip.SetDefault("Casts a star that follows the mouse that creates a rainbow trail as it moves\n" +
                "Once a full shape is created with the trail all enemies within it take damage proportional to how long it took to draw the shape\n" +
                "And the audience goes wild!");
        }

        public override void SetDefaults()
        {
            item.damage = 52;
            item.magic = true;
            item.mana = 10;
            item.width = 70;
            item.height = 70;
            item.useTime = item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item28;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<ArtAttackHoldout>();
            item.channel = true;
            item.shootSpeed = 12f;
            item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RainbowRod);
            recipe.AddIngredient(ItemID.LargeRuby);
            recipe.AddIngredient(ItemID.CrystalShard);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
