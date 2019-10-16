using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class StarnightLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starnight Lance");
            Tooltip.SetDefault("Shoots a starnight beam");
        }

        public override void SetDefaults()
        {
            item.width = 72;
            item.damage = 68;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 23;
            item.useStyle = 5;
            item.useTime = 23;
            item.knockBack = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 72;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<StarnightLanceProjectile>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
