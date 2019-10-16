using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class HarpyRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harpy Ring");
            Tooltip.SetDefault("Increased movement speed\n" +
                "Boosts your maximum flight time by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.harpyRing = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 2);
            recipe.AddIngredient(ItemID.Feather, 5);
            recipe.AddIngredient(ItemID.FallenStar);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
