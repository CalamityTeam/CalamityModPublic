using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeRavager : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Tooltip.SetDefault("The flesh golem constructed using twisted necromancy during the time of my conquest to counter my unstoppable forces.\n" +
                "Its creators were slaughtered by it moments after its conception. It is for the best that it has been destroyed.\n" +
                "Place in your inventory to gain an increase to all damage but reduced wing flight time.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 8;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ravagerLore = true;
        }
    }
}
