using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Tradewinds : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tradewinds");
            Tooltip.SetDefault("Casts fast moving sunlight feathers");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
            item.magic = true;
            item.mana = 7;
            item.width = 28;
            item.height = 30;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item7;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TradewindsProjectile>();
            item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 6);
            recipe.AddIngredient(ItemID.SunplateBlock, 5);
            recipe.AddIngredient(ItemID.Feather, 3);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
