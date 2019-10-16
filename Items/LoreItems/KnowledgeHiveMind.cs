using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeHiveMind : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Tooltip.SetDefault("A hive of clustered microbial-infected flesh.\n" +
                "I do not believe killing it will lessen the corruption here.\n" +
                "Place in your inventory for all of your projectiles to inflict cursed flames when in the corruption.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 3;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ZoneCorrupt)
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.hiveMindLore = true;
            }
        }
    }
}
