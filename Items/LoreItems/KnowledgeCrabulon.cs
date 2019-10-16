﻿using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeCrabulon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crabulon");
            Tooltip.SetDefault("A crab and its mushrooms, a love story.\n" +
                "It's interesting how creatures can adapt given certain circumstances.\n" +
                "Place in your inventory to gain the Mushy buff while underground or in the mushroom biome.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 2;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ZoneGlowshroom || player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    player.AddBuff(ModContent.BuffType<Mushy>(), 2);
                }
            }
        }
    }
}
