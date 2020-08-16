using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.OldDuke;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class OldDukeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<OldDuke>();

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
            item.rare = 10;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<InsidiousImpaler>(),
				ModContent.ItemType<SepticSkewer>(),
				ModContent.ItemType<FetidEmesis>(),
				ModContent.ItemType<VitriolicViper>(),
				ModContent.ItemType<ToxicantTwister>(),
				ModContent.ItemType<CadaverousCarrion>());

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<DukeScales>(), 10);
            DropHelper.DropItem(player, ModContent.ItemType<MutatedTruffle>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<OldDukeMask>(), 7);
        }
    }
}
