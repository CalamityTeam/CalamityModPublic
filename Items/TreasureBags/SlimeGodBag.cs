using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SlimeGodBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<SlimeGodRun>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            // Materials
            DropHelper.DropItem(player, ItemID.Gel, 30, 60);
            DropHelper.DropItem(player, ModContent.ItemType<PurifiedGel>(), 30, 50);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<OverloadedBlaster>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<AbyssalTome>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<EldritchTome>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<CorroslimeStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<CrimslimeStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<GelDart>(), 3, 100, 125);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<ManaOverloader>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<ElectrolyteGelPack>(), CalamityWorld.revenge && !player.Calamity().adrenalineBoostOne);

            // Vanity
            DropHelper.DropItemFromSetChance(player, 7, ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>());

            // Other
            DropHelper.DropItem(player, ModContent.ItemType<StaticRefiner>());
        }
    }
}
