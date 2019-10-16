using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class EidolonTablet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Tablet");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.rare = 9;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(NPCID.CultistBoss) && !NPC.LunarApocalypseIsUp && !NPC.AnyNPCs(NPCID.CultistTablet);
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num1302 = NPC.NewNPC((int)player.Center.X + 30, (int)player.Center.Y - 90, NPCID.CultistBoss, 0, 0f, 0f, 0f, 0f, 255);
                Main.npc[num1302].direction = Main.npc[num1302].spriteDirection = Math.Sign(player.Center.X - (float)player.Center.X - 30f);
            }
            return true;
        }
    }
}
