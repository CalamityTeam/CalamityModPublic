using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class ScuttlersJewel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.MakeUsableWithChlorophyteExtractinator();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.scuttlersJewel = true;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            float dropRand = Main.rand.Next(1, 8);
            resultStack = Main.rand.Next(1, 3);

            if (dropRand == 1f)
                resultType = ItemID.Ruby;
            else if (dropRand == 2f)
                resultType = ItemID.Diamond;
            else if (dropRand == 3f)
                resultType = ItemID.Emerald;
            else if (dropRand == 4f)
                resultType = ItemID.Topaz;
            else if (dropRand == 5f)
                resultType = ItemID.Sapphire;
            else if (dropRand == 6f)
                resultType = ItemID.Amethyst;
            else if (dropRand >= 7f)
                resultType = ItemID.Amber;
        }
    }
}
