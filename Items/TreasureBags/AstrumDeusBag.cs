using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AstrumDeus;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrumDeusBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AstrumDeusHeadSpectral>();

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
            DropHelper.DropItem(player, ModContent.ItemType<Stardust>(), 60, 90);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<TheMicrowave>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<StarSputter>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Starfall>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<GodspawnHelixStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Quasar>(), DropHelper.RareVariantDropRateInt);

            // Equipment
            float f = Main.rand.NextFloat();
            bool replaceWithRare = f <= DropHelper.RareVariantDropRateFloat; // 1/40 chance of getting Hide of Astrum Deus
            DropHelper.DropItemCondition(player, ModContent.ItemType<AstralBulwark>(), !replaceWithRare);
            DropHelper.DropItemCondition(player, ModContent.ItemType<HideofAstrumDeus>(), replaceWithRare);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AstrumDeusMask>(), 7);
        }
    }
}
