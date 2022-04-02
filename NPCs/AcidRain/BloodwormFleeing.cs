using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class BloodwormFleeing : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodworm");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 12;
            npc.height = 42;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit20;
            npc.DeathSound = SoundID.NPCDeath12;
        }

        public override void AI()
        {
            Player player = Main.player[Player.FindClosest(npc.Center, 1, 1)];
            if (npc.velocity == Vector2.Zero)
                npc.velocity = Vector2.UnitY * 12f;

            float intertia = 24f;

            // Attempt to flee from the nearest player.
            npc.velocity = (npc.velocity * intertia - npc.SafeDirectionTo(player.Center) * 12f) / (intertia + 1f);

            // But always dig downward.
            npc.velocity.Y = Math.Abs(npc.velocity.Y);
            npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
