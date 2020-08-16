using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrageldonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AstrumAureus>();

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
            item.expert = true;
            item.rare = 9;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<AstralJelly>(), 12, 16);
            DropHelper.DropItem(player, ModContent.ItemType<Stardust>(), 30, 40);
            DropHelper.DropItem(player, ItemID.FallenStar, 30, 50);

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<Nebulash>(),
				ModContent.ItemType<AuroraBlazer>(),
				ModContent.ItemType<AlulaAustralis>(),
				ModContent.ItemType<BorealisBomber>(),
				ModContent.ItemType<AuroradicalThrow>());

            float leonidChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<LeonidProgenitor>(), CalamityWorld.revenge, leonidChance);

            // Equipment
            DropHelper.DropItemCondition(player, ModContent.ItemType<SquishyBeanMount>(), CalamityWorld.revenge && NPC.downedMoonlord);
            DropHelper.DropItem(player, ModContent.ItemType<GravistarSabaton>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AureusMask>(), 7);

            // Other
            DropHelper.DropItemCondition(player, ModContent.ItemType<StarlightFuelCell>(), CalamityWorld.revenge && !player.Calamity().adrenalineBoostTwo);
            DropHelper.DropItemChance(player, ItemID.HallowedKey, 5);
        }
    }
}
