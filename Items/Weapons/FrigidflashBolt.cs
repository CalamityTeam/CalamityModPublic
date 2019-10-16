using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FrigidflashBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigidflash Bolt");
            Tooltip.SetDefault("Casts a slow-moving ball of flash-freezing magma");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.magic = true;
            item.mana = 13;
            item.width = 28;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FrigidflashBoltProjectile>();
            item.shootSpeed = 6.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "FrostBolt");
            recipe.AddIngredient(null, "FlareBolt");
            recipe.AddIngredient(null, "EssenceofEleum", 2);
            recipe.AddIngredient(null, "EssenceofChaos", 2);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
