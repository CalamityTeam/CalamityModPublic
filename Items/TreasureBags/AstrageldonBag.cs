using CalamityMod.NPCs;

using CalamityMod.World;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AstrageldonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Astrageldon>();

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
            DropHelper.DropItem(player, ModContent.ItemType<AstralJelly>(), 12, 16);
            DropHelper.DropItem(player, ModContent.ItemType<Stardust>(), 30, 40);
            DropHelper.DropItem(player, ItemID.FallenStar, 30, 50);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<Nebulash>(), 4);

            // Equipment
            DropHelper.DropItemCondition(player, ModContent.ItemType<SquishyBeanMount>(), CalamityWorld.revenge && NPC.downedMoonlord);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AureusMask>(), 7);

            // Other
            DropHelper.DropItemCondition(player, ModContent.ItemType<StarlightFuelCell>(), CalamityWorld.revenge);
            DropHelper.DropItemChance(player, ItemID.HallowedKey, 5);
        }
    }
}
