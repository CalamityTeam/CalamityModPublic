using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgePerforators : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforators and their Hive");
            Tooltip.SetDefault("An abomination of comingled flesh, bone, and organ, infested primarily by blood-slurping worms.\n" +
                "The chunks left over from the brain must have been absorbed by the crimson and reconstituted into it.\n" +
                "Place in your inventory for all of your projectiles to inflict ichor when in the crimson.");
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
            if (player.ZoneCrimson)
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.perforatorLore = true;
            }
        }
    }
}
