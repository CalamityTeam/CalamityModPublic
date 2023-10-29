using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.002f;
            Projectile.timeLeft = 36000;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.type != ModContent.ProjectileType<FlakKrakenProj>() ||
                !Main.projectile[(int)Projectile.ai[1]].active ||
                Main.projectile[(int)Projectile.ai[1]].type != ModContent.ProjectileType<FlakKrakenGun>())
            {
                Projectile.Kill();
                return;
            }

            // This code uses player-specific fields (such as the mouse), and does not need to be run for anyone
            // other than its owner.
            if (Main.myPlayer != Projectile.owner)
                return;

            // This needs to happen retroactively due to Deadshot Brooch and other potential items boosting updates
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;

            Projectile.rotation += 0.2f;
            if (Projectile.localAI[0] < 1f)
            {
                Projectile.localAI[0] += 0.002f;
                Projectile.scale += 0.002f;
                Projectile.width = Projectile.height = (int)(50f * Projectile.scale);
            }
            else
            {
                Projectile.width = Projectile.height = 50;
            }
            Player player = Main.player[Projectile.owner];
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Projectile.localAI[0];
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 200, 50, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
