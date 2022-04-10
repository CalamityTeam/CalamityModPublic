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

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            int herbMin = 1;
            int herbMax = 3;
            int seedMin = 2;
            int seedMax = 5;
            DropHelper.DropItemChance(s, player, ItemID.Daybloom, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Moonglow, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Waterleaf, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Deathweed, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Shiverthorn, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Fireblossom, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.Blinkroot, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(s, player, ItemID.DaybloomSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.MoonglowSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.WaterleafSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.DeathweedSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.ShiverthornSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.FireblossomSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.BlinkrootSeeds, 0.2f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.GrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.JungleGrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.MushroomGrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(s, player, ItemID.PumpkinSeed, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(s, player, ItemID.CorruptSeeds, !WorldGen.crimson, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(s, player, ItemID.CrimsonSeeds, WorldGen.crimson, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(s, player, ItemID.HallowedSeeds, Main.hardMode, 0.05f, seedMin, seedMax);
            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium != null)
            {
                DropHelper.DropItemChance(s, player, thorium.Find<ModItem>("MarineKelp").Type, 0.25f, herbMin, herbMax);
                DropHelper.DropItemChance(s, player, thorium.Find<ModItem>("MarineKelpSeeds").Type, 0.1f, seedMin, seedMax);
            }
            Mod soa = CalamityMod.Instance.soa;
            if (soa != null)
            {
                DropHelper.DropItemChance(s, player, soa.Find<ModItem>("Welkinbell").Type, 0.25f, herbMin, herbMax);
                DropHelper.DropItemChance(s, player, soa.Find<ModItem>("WelkinbellSeeds").Type, 0.1f, seedMin, seedMax);
                DropHelper.DropItemCondition(s, player, soa.Find<ModItem>("Illumifern").Type, Main.hardMode, 0.25f, herbMin, herbMax);
                DropHelper.DropItemCondition(s, player, soa.Find<ModItem>("IllumifernSeeds").Type, Main.hardMode, 0.1f, seedMin, seedMax);

                // TODO -- There is no way to determine if SoA's Abaddon is dead without reflection.
                // Dan Yami has confirmed that downed calls will be added to SoA eventually.
                //DropHelper.DropItemCondition(s, player, shadowsOfAbaddon.ItemType("Enduflora"), SacredTools.ModdedWorld.downedAbaddon, 0.25f, herbMin, herbMax);
                //DropHelper.DropItemCondition(s, player, shadowsOfAbaddon.ItemType("EndufloraSeeds"), SacredTools.ModdedWorld.downedAbaddon, 0.1f, seedMin, seedMax);
            }
        }
    }
}
