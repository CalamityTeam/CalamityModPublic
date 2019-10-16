using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeSkeletronPrime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletron Prime");
            Tooltip.SetDefault("What a silly and pointless contraption for something created with the essence of pure terror.\n" +
                "Draedon obviously took several liberties with its design...I am not impressed.\n" +
                "Place in your inventory to gain a boost to your armor penetration.");
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
            modPlayer.skeletronPrimeLore = true;
        }
    }
}
