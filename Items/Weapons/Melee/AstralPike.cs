using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AstralPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Pike");
            Tooltip.SetDefault("Summons astral star swarms on critical hits");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 128;
            item.crit += 25;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.useStyle = 5;
            item.useTime = 13;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 50;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.shoot = ModContent.ProjectileType<AstralPikeProj>();
            item.shootSpeed = 13f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
