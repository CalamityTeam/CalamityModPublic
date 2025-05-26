using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BalefulHarvesterProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0f)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
                Projectile.frame = 0;

            if (Projectile.ai[0] >= 0f && Projectile.ai[0] < 200f)
            {
                int npcTracker = (int)Projectile.ai[0];
                if (Main.npc[npcTracker].active)
                {
                    Vector2 projDirection = Projectile.Center;
                    float projXVel = Main.npc[npcTracker].position.X - projDirection.X;
                    float projYVel = Main.npc[npcTracker].position.Y - projDirection.Y;
                    float projVelocity = (float)Math.Sqrt((double)(projXVel * projXVel + projYVel * projYVel));
                    projVelocity = 8f / projVelocity;
                    projXVel *= projVelocity;
                    projYVel *= projVelocity;
                    Projectile.velocity.X = (Projectile.velocity.X * 14f + projXVel) / 15f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 14f + projYVel) / 15f;
                }
                else
                {
                    float homingRange = 1000f;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.CanBeChasedBy(Projectile, false))
                        {
                            float npcX = n.position.X + (float)(n.width / 2);
                            float npcY = n.position.Y + (float)(n.height / 2);
                            float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                            if (npcDist < homingRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, n.position, n.width, n.height))
                            {
                                homingRange = npcDist;
                                Projectile.ai[0] = (float)n.whoAmI;
                            }
                        }
                    }
                }

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                for (int j = 0; j < 2; j++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X + 4f, Projectile.position.Y + 4f), Projectile.width - 8, Projectile.height - 8, Main.rand.NextBool() ? 5 : 6, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[dust].position -= Projectile.velocity * 2f;
                    Main.dust[dust].noGravity = true;
                    Dust expr_7A4A_cp_0_cp_0 = Main.dust[dust];
                    expr_7A4A_cp_0_cp_0.velocity.X *= 0.3f;
                    Dust expr_7A65_cp_0_cp_0 = Main.dust[dust];
                    expr_7A65_cp_0_cp_0.velocity.Y *= 0.3f;
                }

                return;
            }

            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Main.rand.Next(0, 128));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int j = 0; j < 5; j++)
            {
                int deathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[deathDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[deathDust].scale = 0.5f;
                    Main.dust[deathDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 10; k++)
            {
                int deathDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[deathDust2].noGravity = true;
                Main.dust[deathDust2].velocity *= 5f;
                deathDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[deathDust2].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240);
            if (Projectile.owner == Main.myPlayer)
            {
                int maxProjectiles = 2;
                for (int k = 0; k < maxProjectiles; k++)
                {
                    Vector2 flareVelocity = Main.rand.NextVector2CircularEdge(3.5f, 3.5f) + Projectile.oldVelocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, flareVelocity, ModContent.ProjectileType<TinyFlare>(), (int)(Projectile.damage * 0.35), Projectile.knockBack * 0.35f, Main.myPlayer);
                }
            }
        }
    }
}
