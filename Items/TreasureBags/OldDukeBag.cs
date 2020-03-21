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

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<InsidiousImpaler>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SepticSkewer>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<FetidEmesis>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<VitriolicViper>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ToxicantTwister>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<CadaverousCarrion>(), 3);

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<DukeScales>(), 10);
            DropHelper.DropItem(player, ModContent.ItemType<MutatedTruffle>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<OldDukeMask>(), 7);
        }
    }
}
