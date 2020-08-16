using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
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

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<PlagueCellCluster>(), 13, 17);
            DropHelper.DropItem(player, ModContent.ItemType<InfectedArmorPlating>(), 16, 20);
            DropHelper.DropItem(player, ItemID.Stinger, 4, 8);

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<VirulentKatana>(), // Virulence
				ModContent.ItemType<DiseasedPike>(),
				ModContent.ItemType<ThePlaguebringer>(), // Pandemic
				ModContent.ItemType<Malevolence>(),
				ModContent.ItemType<PestilentDefiler>(),
				ModContent.ItemType<TheHive>(),
				ModContent.ItemType<MepheticSprayer>(), // Blight Spewer
				ModContent.ItemType<PlagueStaff>(),
				ModContent.ItemType<TheSyringe>(),
				ModContent.ItemType<FuelCellBundle>(),
				ModContent.ItemType<InfectedRemote>());

            float malachiteChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Malachite>(), CalamityWorld.revenge, malachiteChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<ToxicHeart>());
            DropHelper.DropItemChance(player, ModContent.ItemType<BloomStone>(), 10);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<PlagueCaller>(), 10);
        }
    }
}
