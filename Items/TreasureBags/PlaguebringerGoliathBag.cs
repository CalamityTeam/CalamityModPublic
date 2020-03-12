using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
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
            DropHelper.DropItemChance(player, ModContent.ItemType<TheSyringe>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<FuelCellBundle>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<InfectedRemote>(), 3);
            float malachiteChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Malachite>(), CalamityWorld.revenge, malachiteChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<ToxicHeart>());
            DropHelper.DropItemChance(player, ModContent.ItemType<BloomStone>(), 10);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
        }
    }
}
