using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public int dustType = 191;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Main.rand.Next(8) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            if (Projectile.alpha > 0)
                Projectile.alpha -= 8;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            float constant = 5f;
            float scaleFactor = 6f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                if (Projectile.alpha < 128)
                {
                    int index = -1;
                    float otherConstant = 300f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 npcCenter = Main.npc[i].Center;
                            float targetDist = Vector2.Distance(npcCenter, Projectile.Center);
                            if (targetDist < otherConstant && index == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, npcCenter, 1, 1))
                            {
                                otherConstant = targetDist;
                                index = i;
                            }
                        }
                    }
                    if (otherConstant < 4f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (index != -1)
                    {
                        Projectile.ai[1] = constant + 1f;
                        Projectile.ai[0] = (float)index;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.ai[1] > constant)
            {
                Projectile.ai[1] += 1f;
                int otherIndex = (int)Projectile.ai[0];
                if (!Main.npc[otherIndex].active || !Main.npc[otherIndex].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 enemyCenterDist = Main.npc[otherIndex].Center - Projectile.Center;
                    if (enemyCenterDist.Length() < 10f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (enemyCenterDist != Vector2.Zero)
                    {
                        enemyCenterDist.Normalize();
                        enemyCenterDist *= scaleFactor;
                    }
                    Projectile.velocity = (Projectile.velocity * 29f + enemyCenterDist) / 30f;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < constant)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == constant)
                {
                    Projectile.ai[1] = 1f;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha < 128 ? null : false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int j = 0; j < 20; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 45; k++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 5f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 9;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }
    }
}
