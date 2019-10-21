
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;

namespace CalamityMod.Items.TreasureBags
{
    public class BrimstoneWaifuBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<BrimstoneElemental>();

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

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ItemID.SoulofFright, 25, 40);
            DropHelper.DropItem(player, ModContent.ItemType<EssenceofChaos>(), 5, 9);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 25, 35);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<Brimlance>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SeethingDischarge>(), 3);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<Abaddon>());
            DropHelper.DropItem(player, ModContent.ItemType<Gehenna>());
            DropHelper.DropItemChance(player, ModContent.ItemType<RoseStone>(), 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Brimrose>(), CalamityWorld.revenge && CalamityWorld.downedProvidence);

            // Vanity
            DropHelper.DropItemCondition(player, ModContent.ItemType<CharredRelic>(), CalamityWorld.revenge);
        }
    }
}
