using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    [LegacyName("PowerCell")]
    public class DraedonPowerCell : ModItem
    {
        // This is how much Charge each Power Cell is worth when charging. Leave this at 1.
        public const float ChargeValue = 1f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Draedon Power Cell");
            Tooltip.SetDefault("Used to charge Draedon's weaponry at a Charging Station\n" +
                               "Also can be processed by the Extractinator for spare parts");
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 14;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.maxStack = 999;

            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.value = Item.sellPrice(0, 0, 0, 20);
        }

        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
        {
            float dropRand = Main.rand.NextFloat();
            resultStack = 1;

            // 2.5% chance for Mysterious Circuitry
            // 2.5% chance for Dubious Plating
            // 85% chance for 65-99 Copper Coins
            // 10% chance for 1 Silver Coin
            if (dropRand < 0.025f)
                resultType = ModContent.ItemType<MysteriousCircuitry>();
            else if (dropRand < 0.05f)
                resultType = ModContent.ItemType<DubiousPlating>();
            else if (dropRand < 0.9f)
            {
                resultType = ItemID.CopperCoin;
                resultStack = Main.rand.Next(65, 100);
            }
            else
                resultType = ItemID.SilverCoin;
        }
    }
}
