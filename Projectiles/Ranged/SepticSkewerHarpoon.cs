using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SepticSkewerHarpoon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.timeLeft = 900;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 171, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Vector2 playerDist = player.Center - Projectile.Center;
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 5f)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.ai[1] % 8f == 0f && Projectile.owner == Main.myPlayer && Main.rand.NextBool(5))
            {
                Vector2 harpoonPos = playerDist * -1f;
                harpoonPos.Normalize();
                harpoonPos *= Main.rand.Next(45, 65) * 0.1f;
                harpoonPos = harpoonPos.RotatedBy((Main.rand.NextDouble() - 0.5) * MathHelper.PiOver2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, harpoonPos.X, harpoonPos.Y, ModContent.ProjectileType<SepticSkewerBacteria>(), (int)(Projectile.damage * 0.175), Projectile.knockBack * 0.2f, Projectile.owner, -10f, 0f);
            }
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.alpha == 0)
            {
                if (Projectile.position.X + (float)(Projectile.width / 2) > player.position.X + (float)(player.width / 2))
                {
                    player.ChangeDir(1);
                }
                else
                {
                    player.ChangeDir(-1);
                }
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.extraUpdates = 2;
            }
            else
            {
                Projectile.extraUpdates = 3;
            }
            Vector2 halfDist = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float xDist = player.position.X + (float)(player.width / 2) - halfDist.X;
            float yDist = player.position.Y + (float)(player.height / 2) - halfDist.Y;
            float playerDistance = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (Projectile.ai[0] == 0f)
            {
                if (playerDistance > 2000f)
                {
                    Projectile.ai[0] = 1f;
                }
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] > 5f)
                {
                    Projectile.alpha = 0;
                }
                if (Projectile.ai[1] > 8f)
                {
                    Projectile.ai[1] = 8f;
                }
                if (Projectile.ai[1] >= 10f)
                {
                    Projectile.ai[1] = 15f;
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.rotation = (float)Math.Atan2((double)yDist, (double)xDist) - 1.57f;
                float returnSpeed = 20f;
                if (playerDistance < 50f)
                {
                    Projectile.Kill();
                }
                playerDistance = returnSpeed / playerDistance;
                xDist *= playerDistance;
                yDist *= playerDistance;
                Projectile.velocity.X = xDist;
                Projectile.velocity.Y = yDist;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }
    }
}
