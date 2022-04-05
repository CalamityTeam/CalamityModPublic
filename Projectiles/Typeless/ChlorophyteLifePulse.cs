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
        public float LifetimeCompletion => 1f - Projectile.timeLeft / (float)Lifetime;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Pulse");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            Projectile.Opacity = 1f - (float)Math.Pow(LifetimeCompletion, 1.56);
            Projectile.scale = MathHelper.Lerp(0.5f, 12f, LifetimeCompletion);

            // Heal all members of the same team.
            if (Projectile.timeLeft == (int)(Lifetime * 0.925f))
            {
                Player owner = Main.player[Projectile.owner];
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
            return Color.Lerp(c1, c2, 1f - Projectile.Opacity) * Projectile.Opacity * 0.67f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = Projectile.GetAlpha(lightColor) * 0.4f;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0f, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.scale * 48f, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
