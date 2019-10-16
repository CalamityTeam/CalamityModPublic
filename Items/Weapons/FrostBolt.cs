using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FrostBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Bolt");
            Tooltip.SetDefault("Casts a slow-moving ball of frost");
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.magic = true;
            item.mana = 6;
            item.width = 28;
            item.height = 30;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FrostBoltProjectile>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IceBlock, 20);
            recipe.AddIngredient(ItemID.Shiverthorn, 2);
            recipe.AddIngredient(ItemID.SnowBlock, 10);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
