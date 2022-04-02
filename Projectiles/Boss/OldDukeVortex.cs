using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeVortex : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Vortex");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 408;
            projectile.height = 408;
            projectile.scale = 0.004f;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 1800;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.scale < 1f)
            {
                if (projectile.alpha > 0)
                    projectile.alpha -= 1;

                projectile.scale += 0.004f;
                if (projectile.scale > 1f)
                    projectile.scale = 1f;

                projectile.width = projectile.height = (int)(408f * projectile.scale);
            }
            else
            {
                if (projectile.timeLeft <= 85)
                {
                    if (projectile.alpha < 255)
                        projectile.alpha += 3;

                    projectile.scale += 0.012f;
                    projectile.width = projectile.height = (int)(408f * projectile.scale);
                }
                else
                    projectile.width = projectile.height = 408;
            }

            projectile.velocity = Vector2.Normalize(new Vector2(projectile.ai[0], projectile.ai[1]) - projectile.Center) * 1.5f;

            projectile.rotation -= 0.1f * (float)(1D - (projectile.alpha / 255D));

            float lightAmt = 2f * projectile.scale;
            Lighting.AddLight(projectile.Center, lightAmt, lightAmt * 2f, lightAmt);

            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 174;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVortex"), (int)projectile.Center.X, (int)projectile.Center.Y);
            }

            if (projectile.timeLeft > 85)
            {
                int numDust = (int)(0.2f * MathHelper.TwoPi * 408f * projectile.scale);
                float angleIncrement = MathHelper.TwoPi / (float)numDust;
                Vector2 dustOffset = new Vector2(408f * projectile.scale, 0f);
                dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);

                int var = (int)(408f * projectile.scale);
                for (int i = 0; i < numDust; i++)
                {
                    if (Main.rand.NextBool(var))
                    {
                        dustOffset = dustOffset.RotatedBy(angleIncrement);
                        int dust = Dust.NewDust(projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid);
                        Main.dust[dust].position = projectile.Center + dustOffset;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity = Vector2.Normalize(projectile.Center - Main.dust[dust].position) * 24f;
                        Main.dust[dust].scale = projectile.scale * 3f;
                    }
                }

                float distanceRequired = 800f * projectile.scale;
                float succPower = 0.4f;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];

                    float distance = Vector2.Distance(player.Center, projectile.Center);
                    if (distance < distanceRequired && player.grappling[0] == -1)
                    {
                        if (Collision.CanHit(projectile.Center, 1, 1, player.Center, 1, 1))
                        {
                            float distanceRatio = distance / distanceRequired;

                            float wingTimeSet = (float)Math.Ceiling((float)player.wingTimeMax * 0.5f * distanceRatio);
                            if (player.wingTime > wingTimeSet)
                                player.wingTime = wingTimeSet;

                            float multiplier = 1f - distanceRatio;
                            if (player.Center.X < projectile.Center.X)
                                player.velocity.X += succPower * multiplier;
                            else
                                player.velocity.X -= succPower * multiplier;

                            if (player.Center.Y < projectile.Center.Y)
                                player.velocity.Y += succPower * multiplier;
                            else
                                player.velocity.Y -= succPower * multiplier;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.timeLeft <= 1680 && projectile.timeLeft > 85;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 210f * projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.timeLeft <= 1680 && projectile.timeLeft > 85)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 600);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
