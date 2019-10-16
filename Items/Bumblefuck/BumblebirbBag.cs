using CalamityMod.NPCs;
using CalamityMod.Utilities;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BumblebirbBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Bumblefuck>();

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
            DropHelper.DropItem(player, ModContent.ItemType<EffulgentFeather>(), 9, 14);

            // Weapons
            DropHelper.DropItemFromSet(player, ModContent.ItemType<GildedProboscis>(), ModContent.ItemType<GoldenEagle>(), ModContent.ItemType<RougeSlash>());
            DropHelper.DropItemChance(player, ModContent.ItemType<Swordsplosion>(), DropHelper.RareVariantDropRateInt);

            // Equipment
            DropHelper.DropItemCondition(player, ModContent.ItemType<RedLightningContainer>(), CalamityWorld.revenge);
        }
    }
}
