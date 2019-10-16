using CalamityMod.World;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class PlaguebringerGoliathBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PlaguebringerGoliath>();

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
            DropHelper.DropItem(player, ModContent.ItemType<PlagueCellCluster>(), 13, 17);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<VirulentKatana>(), 3); // Virulence
            DropHelper.DropItemChance(player, ModContent.ItemType<DiseasedPike>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ThePlaguebringer>(), 3); // Pandemic
            DropHelper.DropItemChance(player, ModContent.ItemType<Malevolence>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<PestilentDefiler>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<TheHive>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<MepheticSprayer>(), 3); // Blight Spewer
            DropHelper.DropItemChance(player, ModContent.ItemType<PlagueStaff>(), 3);
            float malachiteChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Malachite>(), CalamityWorld.revenge, malachiteChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<ToxicHeart>());
            DropHelper.DropItemChance(player, ModContent.ItemType<BloomStone>(), 10);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
        }
    }
}
