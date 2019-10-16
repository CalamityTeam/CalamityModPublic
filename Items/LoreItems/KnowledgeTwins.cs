using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeTwins : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Twins");
            Tooltip.SetDefault("The bio-mechanical watchers of the night, originally created as security using the souls extracted from human eyes.\n" +
                "These creatures did not belong in this world, it's best to be rid of them.\n" +
                "Place in your inventory to gain invisibility at night.");
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
            if (!Main.dayTime)
            {
                player.invis = true;
            }
        }
    }
}
