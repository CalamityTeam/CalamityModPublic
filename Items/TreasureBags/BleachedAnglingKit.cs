using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class BleachedAnglingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleached Angling Kit");
            Tooltip.SetDefault("Has a chance to contain various fishing gear\n" +
            "{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Different drop rates on Normal and Expert, so define normal first, then expert
            var normalOnly = itemLoot.DefineNormalOnlyDropSet();
            normalOnly.Add(ItemID.AnglerTackleBag, 18);
            normalOnly.Add(ItemID.HighTestFishingLine, 12);
            normalOnly.Add(ItemID.TackleBox, 12);
            normalOnly.Add(ItemID.AnglerEarring, 12);
            normalOnly.Add(ItemID.FishermansGuide, 9);
            normalOnly.Add(ItemID.WeatherRadio, 9);
            normalOnly.Add(ItemID.Sextant, 9);
            normalOnly.Add(ItemID.AnglerHat, 4);
            normalOnly.Add(ItemID.AnglerVest, 4);
            normalOnly.Add(ItemID.AnglerPants, 4);
            normalOnly.Add(ItemID.FishingPotion, 4, 2, 3);
            normalOnly.Add(ItemID.SonarPotion, 4, 2, 3);
            normalOnly.Add(ItemID.CratePotion, 4, 2, 3);
            normalOnly.Add(ItemID.GoldenBugNet, 15);
            itemLoot.Add(normalOnly);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ItemID.AnglerTackleBag, 15);
            expertPlus.Add(ItemID.HighTestFishingLine, 10);
            expertPlus.Add(ItemID.TackleBox, 10);
            expertPlus.Add(ItemID.AnglerEarring, 10);
            expertPlus.Add(ItemID.FishermansGuide, 8);
            expertPlus.Add(ItemID.WeatherRadio, 8);
            expertPlus.Add(ItemID.Sextant, 8);
            expertPlus.Add(ItemID.AnglerHat, 2);
            expertPlus.Add(ItemID.AnglerVest, 2);
            expertPlus.Add(ItemID.AnglerPants, 2);
            expertPlus.Add(ItemID.FishingPotion, 2, 2, 3);
            expertPlus.Add(ItemID.SonarPotion, 2, 2, 3);
            expertPlus.Add(ItemID.CratePotion, 2, 2, 3);
            expertPlus.Add(ItemID.GoldenBugNet, 12);
            itemLoot.Add(expertPlus);
        }
    }
}
