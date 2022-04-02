using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodfireBulletProj : ModProjectile
    {
        private const int Lifetime = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfire Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.extraUpdates = 4;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = projectile.direction;

            // Lighting
            Lighting.AddLight(projectile.Center, 0.9f, 0f, 0.15f);

            // Dust
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                int dustID = 90;
                float scale = Main.rand.NextFloat(0.6f, 0.9f);
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, dustID);
                Vector2 posOffset = projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
                d.position += posOffset - 2f * Vector2.UnitY;
                d.noGravity = true;
                d.velocity *= 0.6f;
                d.velocity += projectile.velocity * 0.15f;
                d.scale = scale;
            }
        }

        // These bullets glow in the dark.
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => damage += OnHitEffect(Main.player[projectile.owner]);
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) => damage += OnHitEffect(Main.player[projectile.owner]);

        // Returns the amount of bonus damage that should be dealt. Boosts life regeneration appropriately as a side effect.
        private int OnHitEffect(Player owner)
        {
            // Adds 2 frames to lifeRegenTime on every hit. This increased value is used for the damage calculation.
            owner.lifeRegenTime += 2;

            // Deals (1.00 + (0.05 * current lifeRegen))% of current lifeRegenTime as flat bonus damage on hit.
            // For example, at 0 life regen, you get 1% of lifeRegenTime as bonus damage.
            // At 10 life regen, you get 2%. At 20 life regen, you get 3%.
            // Negative life regen does not decrease damage.
            int regenForCalc = owner.lifeRegen > 0 ? owner.lifeRegen : 0;
            float regenDamageRatio = 0.01f + 0.0005f * regenForCalc;

            // For the sake of bonus damage, life regen time caps at 3600, aka 60 seconds. This is its natural cap in vanilla.
            int regenTimeForCalc = (int)MathHelper.Clamp(owner.lifeRegenTime, 0f, 3600f);
            return (int)(regenDamageRatio * regenTimeForCalc);
        }

        public override void Kill(int timeLeft)
        {
            int dustID = 90;
            int dustCount = 3;
            for(int i = 0; i < dustCount; ++i)
                _ = Dust.NewDust(projectile.Center, 0, 0, dustID, Scale: 1.2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.Center);
            return true;
        }
    }
}
