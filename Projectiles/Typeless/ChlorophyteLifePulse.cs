using CalamityMod.Projectiles.VanillaProjectileOverrides;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class ChlorophyteLifePulse : ModProjectile
    {
        public const int Lifetime = 95;
        public float LifetimeCompletion => 1f - projectile.timeLeft / (float)Lifetime;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Pulse");
        }

        public override void SetDefaults()
        {
            projectile.width = 96;
            projectile.height = 96;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 30;
            projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            projectile.Opacity = 1f - (float)Math.Pow(LifetimeCompletion, 1.56);
            projectile.scale = MathHelper.Lerp(0.5f, 12f, LifetimeCompletion);

            // Heal all members of the same team.
            if (projectile.timeLeft == (int)(Lifetime * 0.925f))
            {
                Player owner = Main.player[projectile.owner];
                List<Player> membersOfSameTeam = new List<Player>()
                {
                    owner
                };
                if (owner.team != 0)
                    membersOfSameTeam.AddRange(Main.player.Where(p => p.team == owner.team && p.active && !p.dead));

                foreach (Player player in membersOfSameTeam)
                {
                    if (player.Calamity().ChlorophyteHealDelay > 0)
                        continue;

                    int healQuantity = (int)(ChlorophyteCrystalAI.AmountToHealPerPulse * owner.AverageDamage());
                    player.statLife += healQuantity;
                    player.HealEffect(healQuantity);
                    player.Calamity().ChlorophyteHealDelay = ChlorophyteCrystalAI.DelayBetweenHeals;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(142, 255, 155, 0);
            Color c2 = new Color(0, 142, 113, 92);
            return Color.Lerp(c1, c2, 1f - projectile.Opacity) * projectile.Opacity * 0.67f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Color drawColor = projectile.GetAlpha(lightColor) * 0.4f;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Vector2 drawPosition = projectile.Center - Main.screenPosition + drawOffset;
                spriteBatch.Draw(texture, drawPosition, null, drawColor, 0f, texture.Size() * 0.5f, projectile.scale, 0, 0f);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(projectile.Center, projectile.scale * 48f, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
