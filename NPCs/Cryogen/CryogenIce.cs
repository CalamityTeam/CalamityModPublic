using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.NPCs
{
    public class CryogenIce : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen's Shield");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.damage = 50;
            npc.width = 240;
            npc.height = 240;
            npc.Calamity().RevPlusDR(0.4f);
            npc.lifeMax = 1400;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 100000;
            }
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
        }

        public override void AI()
        {
            npc.alpha -= 3;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            npc.rotation += 0.15f;
            if (npc.type == ModContent.NPCType<CryogenIce>())
            {
                int num989 = (int)npc.ai[0];
                if (Main.npc[num989].active && Main.npc[num989].type == ModContent.NPCType<Cryogen>())
                {
                    npc.velocity = Vector2.Zero;
                    npc.position = Main.npc[num989].Center;
                    npc.position.X = npc.position.X - (float)(npc.width / 2);
                    npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                    npc.gfxOffY = Main.npc[num989].gfxOffY;
                    return;
                }
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.alpha == 0;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 90, true);
            player.AddBuff(BuffID.Chilled, 60, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num621 = 0; num621 < 25; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 50; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                double deltaAngle = spread / 4f;
                double offsetAngle;
                int num184 = Main.expertMode ? 20 : 23;
                int i;
                for (i = 0; i < 2; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    int ice = Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), ModContent.ProjectileType<IceBlast>(), num184, 0f, Main.myPlayer, 0f, 0f);
                    int ice2 = Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), ModContent.ProjectileType<IceBlast>(), num184, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[ice].timeLeft = 300;
                    Main.projectile[ice2].timeLeft = 300;
                }
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                for (int spike = 0; spike < 4; spike++)
                {
                    randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                    Gore.NewGore(npc.Center, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore4"), 1f);
                    Gore.NewGore(npc.Center, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore5"), 1f);
                    Gore.NewGore(npc.Center, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore6"), 1f);
                    Gore.NewGore(npc.Center, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore7"), 1f);
                }
            }
        }
    }
}
