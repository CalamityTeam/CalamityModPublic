using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
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
            DropHelper.DropItem(player, ModContent.ItemType<DivineGeode>(), 20, 30);

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<HolyCollider>(),
				ModContent.ItemType<SolarFlare>(),
				ModContent.ItemType<TelluricGlare>(),
				ModContent.ItemType<BlissfulBombardier>(),
				ModContent.ItemType<PurgeGuzzler>(),
				ModContent.ItemType<MoltenAmputator>(),
				ModContent.ItemType<DazzlingStabberStaff>());

            float pristineFuryChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<PristineFury>(), CalamityWorld.revenge, pristineFuryChance);

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<SamuraiBadge>(), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItem(player, ModContent.ItemType<BlazingCore>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<ProvidenceMask>(), 7);
        }
    }
}
