using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Tiles.Crags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class BrimstoneCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 10;
            DisplayName.SetDefault("Brimstone Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.createTile = ModContent.TileType<BrimstoneCrateTile>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetSource_OpenItem(Item.type);

            //Vanilla materials
            DropHelper.DropItem(s, player, ItemID.Obsidian, 2, 5);
            DropHelper.DropItemChance(s, player, ItemID.Hellstone, 0.25f, 2, 5);
            DropHelper.DropItemChance(s, player, ItemID.HellstoneBar, 0.1f, 1, 3);

            //Modded materials
            DropHelper.DropItem(s, player, ModContent.ItemType<DemonicBoneAsh>(), 1, 4);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<UnholyCore>(), DownedBossSystem.downedBrimstoneElemental, 0.1f, 1, 3);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Bloodstone>(), DownedBossSystem.downedProvidence && DownedBossSystem.downedPolterghast, 0.1f, 1, 3);

            // Weapons (none)

            //Bait
            DropHelper.DropItemChance(s, player, ItemID.MasterBait, 10, 1, 2);
            DropHelper.DropItemChance(s, player, ItemID.JourneymanBait, 5, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.ApprenticeBait, 3, 2, 3);

            //Potions
            DropHelper.DropItemChance(s, player, ItemID.ObsidianSkinPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.SwiftnessPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.IronskinPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.NightOwlPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.ShinePotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.MiningPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.HeartreachPotion, 10, 1, 3);
            DropHelper.DropItemChance(s, player, ItemID.TrapsightPotion, 10, 1, 3); //Dangersense Potion
            DropHelper.DropItemChance(s, player, ItemID.InfernoPotion, 10, 1, 3);
            int healingPotID = ItemID.LesserHealingPotion;
            int manaPotID = ItemID.LesserManaPotion;
            if (DownedBossSystem.downedDoG)
            {
                healingPotID = ModContent.ItemType<SupremeHealingPotion>();
                manaPotID = ModContent.ItemType<SupremeManaPotion>();
            }
            else if (DownedBossSystem.downedProvidence)
            {
                healingPotID = ItemID.SuperHealingPotion;
                manaPotID = ItemID.SuperManaPotion;
            }
            else if (NPC.downedMechBossAny)
            {
                healingPotID = ItemID.GreaterHealingPotion;
                manaPotID = ItemID.GreaterManaPotion;
            }
            else if (NPC.downedBoss3)
            {
                healingPotID = ItemID.HealingPotion;
                manaPotID = ItemID.ManaPotion;
            }
            DropHelper.DropItemChance(s, player, Main.rand.NextBool(2) ? healingPotID : manaPotID, 0.25f, 2, 5);

            //Money
            DropHelper.DropItem(s, player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(s, player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
