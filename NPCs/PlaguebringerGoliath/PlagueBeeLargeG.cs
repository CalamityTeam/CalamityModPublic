using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueBeeLargeG : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Charger");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.damage = 65;
            npc.width = 36;
            npc.height = 30;
            npc.defense = 20;
            npc.lifeMax = 400;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 100000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = CalamityWorld.bossRushActive ? 0f : 0.95f;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.buffImmune[189] = true;
            npc.buffImmune[153] = true;
            npc.buffImmune[70] = true;
            npc.buffImmune[69] = true;
            npc.buffImmune[44] = true;
            npc.buffImmune[39] = true;
            npc.buffImmune[24] = true;
            npc.buffImmune[20] = true;
            npc.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            npc.buffImmune[ModContent.BuffType<Plague>()] = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge;
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.025f, 0.15f, 0.015f);
            Player player = Main.player[npc.target];
            if (player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = npc.velocity.Y + 3f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 3f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            npc.rotation = npc.velocity.X * 0.04f;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.TargetClosest(true);
            Vector2 vector145 = new Vector2(npc.Center.X, npc.Center.Y);
            float num1258 = Main.player[npc.target].Center.X - vector145.X;
            float num1259 = Main.player[npc.target].Center.Y - vector145.Y;
            float num1260 = (float)Math.Sqrt((double)(num1258 * num1258 + num1259 * num1259));
            float num1261 = revenge ? 22f : 20f;
            if (CalamityWorld.bossRushActive)
                num1261 = 30f;

            num1260 = num1261 / num1260;
            num1258 *= num1260;
            num1259 *= num1260;
            npc.velocity.X = (npc.velocity.X * 100f + num1258) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + num1259) / 101f;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
