using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ProvidenceBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Providence>();

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
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<UnholyEssence>(), 25, 35);
            DropHelper.DropItem(player, ModContent.ItemType<DivineGeode>(), 15, 25);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<HolyCollider>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SolarFlare>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<TelluricGlare>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<BlissfulBombardier>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<PurgeGuzzler>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<MoltenAmputator>(), 3);

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<SamuraiBadge>(), DropHelper.RareVariantDropRateInt);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<ProvidenceMask>(), 7);

            // Other
            DropHelper.DropItem(player, ModContent.ItemType<RuneofCos>());
        }
    }
}
