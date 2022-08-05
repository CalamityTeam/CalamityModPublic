using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class StuffedFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stuffed Fish");
            Tooltip.SetDefault("Right click to extract herbs and seeds");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 34;
            Item.height = 30;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            int herbMin = 1;
            int herbMax = 3;
            int seedMin = 2;
            int seedMax = 5;

            // Herbs
            itemLoot.Add(ItemID.Daybloom, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Blinkroot, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Waterleaf, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Shiverthorn, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Moonglow, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Deathweed, 4, herbMin, herbMax);
            itemLoot.Add(ItemID.Fireblossom, 4, herbMin, herbMax);

            // Herb seeds
            itemLoot.Add(ItemID.DaybloomSeeds, 5, seedMin, seedMax);
            itemLoot.Add(ItemID.BlinkrootSeeds, 5, seedMin, seedMax);
            itemLoot.Add(ItemID.WaterleafSeeds, 5, seedMin, seedMax);
            itemLoot.Add(ItemID.ShiverthornSeeds, 5, seedMin, seedMax);
            itemLoot.Add(ItemID.MoonglowSeeds, 5, seedMin, seedMax);
            itemLoot.Add(ItemID.DeathweedSeeds, 5, herbMin, herbMax);
            itemLoot.Add(ItemID.FireblossomSeeds, 5, seedMin, seedMax);

            // Miscellaneous seeds
            itemLoot.Add(ItemID.GrassSeeds, 10, seedMin, seedMax);
            itemLoot.Add(ItemID.JungleGrassSeeds, 10, seedMin, seedMax);
            itemLoot.Add(ItemID.MushroomGrassSeeds, 10, seedMin, seedMax);
            itemLoot.Add(ItemID.PumpkinSeed, 20, seedMin, seedMax);

            // Biome grass seeds
            itemLoot.AddIf(() => !WorldGen.crimson , ItemID.CorruptSeeds, 20, seedMin, seedMax);
            itemLoot.AddIf(() => WorldGen.crimson , ItemID.CrimsonSeeds, 20, seedMin, seedMax);
            itemLoot.AddIf(() => Main.hardMode , ItemID.HallowedSeeds, 20, seedMin, seedMax);

            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium is not null)
            {
                itemLoot.Add(thorium.Find<ModItem>("MarineKelp").Type, 4, herbMin, herbMax);
                itemLoot.Add(thorium.Find<ModItem>("MarineKelpSeeds").Type, 10, seedMin, seedMax);
            }

            Mod soa = CalamityMod.Instance.soa;
            if (soa is not null)
            {
                itemLoot.Add(soa.Find<ModItem>("Welkinbell").Type, 4, herbMin, herbMax);
                itemLoot.Add(soa.Find<ModItem>("WelkinbellSeeds").Type, 10, seedMin, seedMax);

                itemLoot.AddIf(() => Main.hardMode, soa.Find<ModItem>("Illumifern").Type, 4, herbMin, herbMax);
                itemLoot.AddIf(() => Main.hardMode, soa.Find<ModItem>("IllumifernSeeds").Type, 10, seedMin, seedMax);

                // TODO -- There is no way to determine if SoA's Abaddon is dead without reflection.
                // Dan Yami has confirmed that downed calls will be added to SoA eventually.
                //itemLoot.AddIf(() => SacredTools.ModdedWorld.downedAbaddon, soa.Find<ModItem>("Enduflora").Type, 4, herbMin, herbMax);
                //itemLoot.AddIf(() => SacredTools.ModdedWorld.downedAbaddon, soa.Find<ModItem>("EndufloraSeeds").Type, 10, seedMin, seedMax);
            }
        }
    }
}
