using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;
namespace CalamityMod.Items.TreasureBags
{
    public class RavagerBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<RavagerBody>();

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
            int barMin = CalamityWorld.downedProvidence ? 7 : 2;
            int barMax = CalamityWorld.downedProvidence ? 12 : 3;
            int coreMin = CalamityWorld.downedProvidence ? 2 : 1;
            int coreMax = CalamityWorld.downedProvidence ? 4 : 3;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 60, 70);
            DropHelper.DropItem(player, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<DraedonBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<CruptixBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
            DropHelper.DropItemCondition(player, ModContent.ItemType<BarofLife>(), CalamityWorld.downedProvidence);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CoreofCalamity>(), CalamityWorld.downedProvidence, 0.5f);

            // Weapons
            DropHelper.DropItemFromSet(player,
                ModContent.ItemType<UltimusCleaver>(),
                ModContent.ItemType<RealmRavager>(),
                ModContent.ItemType<Hematemesis>(),
                ModContent.ItemType<SpikecragStaff>(),
                ModContent.ItemType<CraniumSmasher>());
            DropHelper.DropItemFromSetChance(player, 0.05f, ModContent.ItemType<CorpusAvertorMelee>(), ModContent.ItemType<CorpusAvertor>());

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodPact>(), 0.5f);
            DropHelper.DropItemChance(player, ModContent.ItemType<FleshTotem>(), 0.5f);
            DropHelper.DropItemCondition(player, ModContent.ItemType<BloodflareCore>(), CalamityWorld.downedProvidence);
            DropHelper.DropItemCondition(player, ModContent.ItemType<InfernalBlood>(), CalamityWorld.revenge);

            // Vanity
            // there is no Ravager mask yet
        }
    }
}
