using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class MeteorFistStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Rogue/MeteorFistProj";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 360;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() >= 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float dustyX = 0f;
                    float dustyY = 0f;
                    if (i == 1)
                    {
                        dustyX = Projectile.velocity.X * 0.5f;
                        dustyY = Projectile.velocity.Y * 0.5f;
                    }
                    int meteorDust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustyX, Projectile.position.Y + 3f + dustyY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 0.5f);
                    Main.dust[meteorDust].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[meteorDust].velocity *= 0.2f;
                    Main.dust[meteorDust].noGravity = true;
                }
            }

            // Almost instantly accelerate to very high speed
            if (Projectile.velocity.Length() < 12f)
            {
                Projectile.velocity *= 1.25f;
            }
            else if (Main.rand.NextBool())
            {
                int fieryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[fieryDust].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[fieryDust].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[fieryDust].noGravity = true;
                Main.dust[fieryDust].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
                Main.rand.Next(2);
                fieryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[fieryDust].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[fieryDust].noGravity = true;
                Main.dust[fieryDust].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2 - 6)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
            }

            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] > 10f || Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int j = 0; j < 40; j++)
            {
                int boomDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[boomDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[boomDust].scale = 0.5f;
                    Main.dust[boomDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 70; k++)
            {
                int boomDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[boomDust2].noGravity = true;
                Main.dust[boomDust2].velocity *= 5f;
                boomDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[boomDust2].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
            int boom = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            Projectile.damage /= 2;
            float minDist = 999f;
            int index = 0;
            // Get the closest enemy to the fist
            if (Projectile.penetrate != -1)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false) && npc != target)
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }
                Vector2 newFistVelocity;
                if (minDist < 999f)
                {
                    newFistVelocity = Main.npc[index].Center - Projectile.Center;
                }
                else
                {
                    newFistVelocity = -Projectile.velocity;
                }
                newFistVelocity.Normalize();
                newFistVelocity *= 10f;
                Projectile.velocity = newFistVelocity;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire, 120);
    }
}
