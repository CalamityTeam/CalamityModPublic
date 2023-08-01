using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    [LegacyName("PowerCell")]
    public class DraedonPowerCell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.DraedonItems";
        // This is how much Charge each Power Cell is worth when charging. Leave this at 1.
        public const float ChargeValue = 1f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 14;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.maxStack = 9999;

            Item.MakeUsableWithChlorophyteExtractinator();
            Item.useTime = 2;
            Item.value = Item.sellPrice(0, 0, 0, 20);
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
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
