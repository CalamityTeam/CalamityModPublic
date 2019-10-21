using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.NPCs.Leviathan
{
    public class SirenClone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Siren Clone");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 70;
            npc.height = 120;
            npc.lifeMax = 3000;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.chaseable = false;
            npc.dontTakeDamage = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.alpha = 255;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.5f, 0.3f);
            if (CalamityGlobalNPC.siren < 0 || !Main.npc[CalamityGlobalNPC.siren].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            if (npc.alpha > 50)
            {
                npc.alpha -= 5;
            }
            npc.TargetClosest(true);
            Vector2 center = npc.Center;
            int num14 = Math.Sign(Main.player[npc.target].Center.X - center.X);
            if (num14 != 0)
            {
                npc.direction = npc.spriteDirection = num14;
            }
            Vector2 direction = Main.player[npc.target].Center - center;
            direction.Normalize();
            direction *= CalamityWorld.bossRushActive ? 16f : 11f;
            npc.ai[0] += 1f;
            if (npc.ai[0] > 45f)
            {
                npc.ai[0] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = Main.expertMode ? 26 : 32;
                    Projectile.NewProjectile(center.X, center.Y, direction.X, direction.Y, ModContent.ProjectileType<WaterSpear>(), damage, 0f, npc.target);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3000;
            npc.damage = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
