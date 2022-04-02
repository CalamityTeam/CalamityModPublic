using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StarofDestruction";

        public int hitCount = 0;
        private static float Radius = 47f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star of Destruction");
        }

        public override void SetDefaults()
        {
            projectile.width = 94;
            projectile.height = 94;
            projectile.friendly = true;
            projectile.penetrate = 16;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 191, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation += Math.Sign(projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (projectile.Calamity().stealthStrike || hitCount > 16)
                hitCount = 16;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            hitCount++;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            hitCount++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, Radius, targetHitbox);

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            Vector2 vector2 = new Vector2(20f, 20f);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 1.4f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<DestructionBolt>(), (int)(projectile.damage * 0.5), 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
