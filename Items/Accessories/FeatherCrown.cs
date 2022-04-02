using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FeatherCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feather Crown");
            Tooltip.SetDefault("15% increased rogue projectile velocity\n" +
                "Stealth strikes cause feathers to fall from the sky on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.throwingVelocity += 0.15f;
            modPlayer.featherCrown = true;
            if (!hideVisual)
                modPlayer.featherCrownDraw = true; //this bool is just used for drawing
        }

        public override void UpdateVanity(Player player, EquipType type)
        {
            player.Calamity().featherCrownDraw = true; //this bool is just used for drawing
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldCrown);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumCrown);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class FeatherCrownHair : EquipTexture
    {
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = false;
            drawAltHair = false;
        }
    }
}
