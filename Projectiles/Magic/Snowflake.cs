using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    [PierceResistException]
    public class Snowflake : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 70;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 280;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.coldDamage = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null)
                return;
            if (player.CantUseHoldout())
                Projectile.Kill();
            if (Projectile.type != ModContent.ProjectileType<Snowflake>() ||
                Main.player[Projectile.owner].HeldItem.type != ModContent.ItemType<SnowstormStaff>() || !Main.player[Projectile.owner].channel)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[2]--;
            }

            // This code uses player-specific fields (such as the mouse), and does not need to be run for anyone
            // other than its owner.
            if (Main.myPlayer != Projectile.owner)
                return;

            Projectile.rotation += 0.2f;
            if (Projectile.localAI[0] < 1f)
            {
                Projectile.localAI[0] += 0.002f;
            }
            else
            {
                Projectile.width = Projectile.height = 50;
            }
            Vector2 projPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float projX = (float)Main.mouseX + Main.screenPosition.X - projPos.X;
            float projY = (float)Main.mouseY + Main.screenPosition.Y - projPos.Y;
            if (player.gravDir == -1f)
            {
                projY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projPos.Y;
            }
            if ((float.IsNaN(projX) && float.IsNaN(projY)) || (projX == 0f && projY == 0f))
            {
                projX = (float)player.direction;
                projY = 0f;
            }
            projPos += new Vector2(projX, projY);
            float speed = 30f;
            float speedScale = 3f;
            Vector2 vectorPos = Projectile.Center;
            if (Vector2.Distance(projPos, vectorPos) < 90f)
            {
                speed = 10f;
                speedScale = 1f;
            }
            if (Vector2.Distance(projPos, vectorPos) < 30f)
            {
                speed = 3f;
                speedScale = 0.3f;
            }
            if (Vector2.Distance(projPos, vectorPos) < 10f)
            {
                speed = 1f;
                speedScale = 0.1f;
            }
            float projectileX = projPos.X - vectorPos.X;
            float projectileY = projPos.Y - vectorPos.Y;
            float projectileAdjust = (float)Math.Sqrt((double)(projectileX * projectileX + projectileY * projectileY));
            projectileAdjust = speed / projectileAdjust;
            projectileX *= projectileAdjust;
            projectileY *= projectileAdjust;
            if (Projectile.velocity.X < projectileX)
            {
                Projectile.velocity.X = Projectile.velocity.X + speedScale;
                if (Projectile.velocity.X < 0f && projectileX > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speedScale;
                }
            }
            else if (Projectile.velocity.X > projectileX)
            {
                Projectile.velocity.X = Projectile.velocity.X - speedScale;
                if (Projectile.velocity.X > 0f && projectileX < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speedScale;
                }
            }
            if (Projectile.velocity.Y < projectileY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + speedScale;
                if (Projectile.velocity.Y < 0f && projectileY > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speedScale;
                }
            }
            else if (Projectile.velocity.Y > projectileY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - speedScale;
                if (Projectile.velocity.Y > 0f && projectileY < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speedScale;
                }
            }
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            float pushForce = 0.15f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible.
                if (!otherProj.active || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == Projectile.type;
                float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                float distancegate = 100f;
                if (sameProjType && taxicabDist < distancegate)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int x = 1; x <= 2; x++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.5f + Main.rand.NextVector2Square(-4, 4), Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type, 0.7f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
            float randOffset = Main.rand.NextFloat(0, 2 * MathHelper.Pi);
            // spawn mini stars on hit with a 4 second cooldown
            if (Projectile.owner == Main.myPlayer && Projectile.ai[2] <= 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 3f + randOffset).ToRotationVector2() * 4f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SnowflakeIceStar>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                    Main.projectile[p].DamageType = DamageClass.Magic;
                }
                SoundEngine.PlaySound(SoundID.Item30, Projectile.Center);
                // set the 4 second cooldown
                Projectile.ai[2] = 240;
            }
        }
    }
}
