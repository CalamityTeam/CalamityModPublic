using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeEyeofCthulhu : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Eye of Cthulhu");
            Tooltip.SetDefault("That eye...how peculiar.\n" +
                "I sensed it watching you more intensely as you grew stronger.\n" +
                "Place in your inventory for night vision at night.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 1;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (!Main.dayTime)
                player.nightVision = true;
        }
    }
}
