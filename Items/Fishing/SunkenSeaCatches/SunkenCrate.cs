using CalamityMod.Items.Materials;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.SunkenSea;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SunkenCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Sunken Crate");
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
            Item.createTile = ModContent.TileType<SunkenCrateTile>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ModContent.ItemType<Items.Placeables.Navystone>(), 1, 10, 30);
            itemLoot.Add(ModContent.ItemType<Items.Placeables.EutrophicSand>(), 1, 10, 30);
            itemLoot.AddIf(() => DownedBossSystem.downedDesertScourge, ModContent.ItemType<PrismShard>(), 1, 5, 10);
            itemLoot.AddIf(() => DownedBossSystem.downedDesertScourge, ModContent.ItemType<Items.Placeables.SeaPrism>(), 5, 2, 5);
            itemLoot.AddIf(() => DownedBossSystem.downedCLAMHardMode, ModContent.ItemType<MolluskHusk>(), new Fraction(12, 100), 2, 5);

            // Weapons
            var lcr = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedCLAMHardMode);
            lcr.Add(new OneFromOptionsNotScaledWithLuckDropRule(7, 100,
                ModContent.ItemType<ShellfishStaff>(),
                ModContent.ItemType<ClamCrusher>(),
                ModContent.ItemType<Poseidon>(),
                ModContent.ItemType<ClamorRifle>()));

            //Bait
            itemLoot.Add(ItemID.MasterBait, 10, 1, 2);
            itemLoot.Add(ItemID.JourneymanBait, 5, 1, 3);
            itemLoot.Add(ModContent.ItemType<SeaMinnowItem>(), 5, 1, 3);
            itemLoot.Add(ItemID.ApprenticeBait, 3, 2, 3);

            //Potions
            itemLoot.AddCratePotionRules();

            //Money
            itemLoot.Add(ItemID.SilverCoin, 1, 10, 90);
            itemLoot.Add(ItemID.GoldCoin, 2, 1, 5);
        }

    }
}
