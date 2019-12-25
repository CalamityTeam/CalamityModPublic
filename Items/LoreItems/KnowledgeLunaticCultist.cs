using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeLunaticCultist : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunatic Cultist");
            Tooltip.SetDefault("The gifted one that terminated my grand summoning so long ago with his uncanny powers over the arcane.\n" +
                "Someone I once held in such contempt for his actions is now...deceased, his sealing ritual undone...prepare for the end.\n" +
                "Your impending doom approaches...\n" +
                "Place in your inventory for an increase to all stats during the lunar event.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 9;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.LunarApocalypseIsUp)
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.lunaticCultistLore = true;
            }
        }
    }
}
