using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeDestroyer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Destroyer");
            Tooltip.SetDefault("A machine brought to life by the mighty souls of warriors, and built to excavate massive tunnels in planets to gather resources.\n" +
                "Could have proven useful if Draedon didn't have an obsession with turning everything into a tool of destruction.\n" +
                "Place in your inventory to gain a boost to your pick speed.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.destroyerLore = true;
        }
    }
}
