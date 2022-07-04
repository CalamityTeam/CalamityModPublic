using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rover Drive");
            Tooltip.SetDefault("Activates a protective shield that grants 15 defense for 10 seconds\n" +
            //Actually 10.1 seconds at full power with a dissipation across 0.1666 seconds but whatever
            "The shield then dissipates and recharges for 20 seconds before being reactivated");

            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;

            //Needed for extractination
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.roverDrive = true;
        }

        //Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
        {
            resultType = ModContent.ItemType<WulfrumMetalScrap>();
            resultStack = Main.rand.Next(3, 6);

            if (Main.rand.NextFloat() > 0.8f)
            {
                resultStack = 1;
                resultType = ModContent.ItemType<EnergyCore>();
            }
        }
    }
}
