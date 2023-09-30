using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodfireBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private const int Lifetime = 600;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            // Lighting
            Lighting.AddLight(Projectile.Center, 0.9f, 0f, 0.15f);

            // Dust
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                int dustID = 90;
                float scale = Main.rand.NextFloat(0.6f, 0.9f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, dustID);
                Vector2 posOffset = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
                d.position += posOffset - 2f * Vector2.UnitY;
                d.noGravity = true;
                d.velocity *= 0.6f;
                d.velocity += Projectile.velocity * 0.15f;
                d.scale = scale;
            }
        }

        // These bullets glow in the dark.
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage.Flat += OnHitEffect(Main.player[Projectile.owner]);

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

        public override void OnKill(int timeLeft)
        {
            int dustID = 90;
            int dustCount = 3;
            for (int i = 0; i < dustCount; ++i)
               Dust.NewDust(Projectile.Center, 0, 0, dustID, Scale: 1.2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            return true;
        }
    }
}
