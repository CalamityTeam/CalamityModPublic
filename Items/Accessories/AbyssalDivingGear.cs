using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //[AutoloadEquip(EquipType.Face)]
    public class AbyssalDivingGear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Diving Gear");
            Tooltip.SetDefault("Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the abyss\n" +
                "Grants the ability to swim and greatly extends underwater breathing\n" +
                "Provides light underwater and extra mobility on ice\n" +
                "Provides a moderate amount of light in the abyss\n" +
                "Greatly reduces breath loss in the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.depthCharm = true;
            modPlayer.jellyfishNecklace = true;
            if (!hideVisual)
                modPlayer.abyssDivingGear = true; //this bool is just used for drawing
            player.arcticDivingGear = true;
            player.accFlipper = true;
            player.accDivingHelm = true;
            player.iceSkate = true;
            if (player.wet)
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }
        }

        public override void UpdateVanity(Player player, EquipType type)
        {
            player.Calamity().abyssDivingGear = true; //this bool is just used for drawing
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ArcticDivingGear);
            recipe.AddIngredient(ModContent.ItemType<DepthCharm>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class AbyssDivingGearHair : EquipTexture
    {
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = false;
            drawAltHair = false;
        }
    }
}
