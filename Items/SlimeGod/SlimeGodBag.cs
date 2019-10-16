using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
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
            DropHelper.DropItemCondition(player, ModContent.ItemType<ElectrolyteGelPack>(), CalamityWorld.revenge);

            // Vanity
            DropHelper.DropItemFromSetChance(player, 7, ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>());

            // Other
            DropHelper.DropItem(player, ModContent.ItemType<StaticRefiner>());
        }
    }
}
