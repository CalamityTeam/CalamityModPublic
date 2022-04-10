using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SulphuricTreasure : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Treasure");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Green; //Green for thematics
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            // Loops 4 times to compensate for dropping only one treasure now
            for (int i = 0; i < 4; i++)
            {
                if (Main.rand.NextBool(15))
                {
                    int potionType = Utils.SelectRandom(WorldGen.genRand, new int[]
                    {
                        ItemID.SpelunkerPotion,
                        ItemID.MagicPowerPotion,
                        ItemID.ShinePotion,
                        ItemID.WaterWalkingPotion,
                        ItemID.ObsidianSkinPotion,
                        ItemID.WaterWalkingPotion,
                        ItemID.GravitationPotion,
                        ItemID.RegenerationPotion,
                        ModContent.ItemType<TriumphPotion>(),
                        ModContent.ItemType<AnechoicCoating>(),
                        ItemID.GillsPotion,
                        ItemID.EndurancePotion,
                        ItemID.HeartreachPotion,
                        ItemID.FlipperPotion,
                        ItemID.LifeforcePotion,
                        ItemID.InfernoPotion
                    });
                    DropHelper.DropItem(s, player, potionType);
                }
                else
                {
                    switch (Main.rand.Next(10))
                    {
                        case 0:
                            int glowstickAmt = Main.rand.Next(2, 6);
                            if (Main.expertMode)
                            {
                                glowstickAmt += Main.rand.Next(1, 7);
                            }
                            DropHelper.DropItem(s, player, ItemID.Glowstick, glowstickAmt);
                            break;
                        case 1:
                            DropHelper.DropItem(s, player, ItemID.JestersArrow, 10, 20);
                            break;
                        case 2:
                            DropHelper.DropItem(s, player, ItemID.HealingPotion);
                            break;
                        case 3:
                            DropHelper.DropItem(s, player, ItemID.Bomb, 5, 8);
                            break;
                        default:
                            int coinCount = 5000 + Main.rand.Next(-100, 101);
                            while (coinCount > 0)
                            {
                                if (coinCount > 1000000)
                                {
                                    int ptCoinAmt = coinCount / 1000000;
                                    if (ptCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    coinCount -= 1000000 * ptCoinAmt;
                                    DropHelper.DropItem(s, player, ItemID.PlatinumCoin, ptCoinAmt);
                                }
                                else if (coinCount > 10000)
                                {
                                    int auCoinAmt = coinCount / 10000;
                                    if (auCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    coinCount -= 10000 * auCoinAmt;
                                    DropHelper.DropItem(s, player, ItemID.GoldCoin, auCoinAmt);
                                }
                                else if (coinCount > 100)
                                {
                                    int agCoinAmt = coinCount / 100;
                                    if (agCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    coinCount -= 100 * agCoinAmt;
                                    DropHelper.DropItem(s, player, ItemID.SilverCoin, agCoinAmt);
                                }
                                else
                                {
                                    int cuCoinAmt = coinCount;
                                    if (cuCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(4) + 1;
                                    }
                                    if (cuCoinAmt < 1)
                                    {
                                        cuCoinAmt = 1;
                                    }
                                    coinCount -= cuCoinAmt;
                                    DropHelper.DropItem(s, player, ItemID.CopperCoin, cuCoinAmt);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
