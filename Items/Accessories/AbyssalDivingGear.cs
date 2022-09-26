using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class AbyssalDivingGear : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abyssal Diving Gear");
            Tooltip.SetDefault("Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the abyss\n" +
                "Grants the ability to swim and greatly extends underwater breathing\n" +
                "Provides light underwater and extra mobility on ice\n" +
                "Provides a moderate amount of light in the abyss\n" +
                "Greatly reduces breath loss in the abyss");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
				ArmorIDs.Face.Sets.PreventHairDraw[equipSlot] = true;
				ArmorIDs.Face.Sets.OverrideHelmet[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.depthCharm = true;
            modPlayer.jellyfishNecklace = true;
            player.arcticDivingGear = true;
            player.accFlipper = true;
            player.accDivingHelm = true;
            player.iceSkate = true;
            if (player.wet)
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ArcticDivingGear).
                AddIngredient<DepthCharm>().
                AddIngredient<DepthCells>(10).
                AddIngredient<Lumenyl>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
