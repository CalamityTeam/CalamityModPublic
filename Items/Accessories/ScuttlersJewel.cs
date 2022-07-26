using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class ScuttlersJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Scuttler's Jewel");
            Tooltip.SetDefault("Stealth strike projectiles spawn a jewel spike when destroyed\n" +
                "Can also be broken down at an extractinator");
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;

            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.scuttlersJewel = true;
        }

        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
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
