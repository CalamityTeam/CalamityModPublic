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
            Item.width = 44;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
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
            CreateRecipe().
                AddIngredient(ItemID.GoldCrown).
                AddIngredient<AerialiteBar>(6).
                AddIngredient(ItemID.Feather, 8).
                AddTile(TileID.SkyMill).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.PlatinumCrown).
                AddIngredient<AerialiteBar>(6).
                AddIngredient(ItemID.Feather, 8).
                AddTile(TileID.SkyMill).
                Register();
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
