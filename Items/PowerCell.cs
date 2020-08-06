using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class PowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon Power Cell");
            Tooltip.SetDefault("Used to charge Draedon's weaponry at a charger\n" +
                               "Also can be processed by the Extractinator for spare parts");
            ItemID.Sets.ExtractinatorMode[item.type] = item.type;
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 14;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.maxStack = 999;

            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 10;
            item.useTime = 5;
            item.autoReuse = true;
            item.useTurn = true;
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
