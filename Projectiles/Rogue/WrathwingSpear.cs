using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class WrathwingSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private const float FireballAngleVariance = 0.07f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            // Spit fireballs constantly. Always spits one fireball immediately upon being thrown.
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] <= 0f)
            {
                Projectile.ai[0] = 20f;

                int fireballID = ModContent.ProjectileType<WrathwingFireball>();
                int damage = (int)(Projectile.damage * 0.7f);
                float angleDiff = Main.rand.NextFloat(-FireballAngleVariance, FireballAngleVariance);
                Vector2 velocity = Projectile.velocity.RotatedBy(angleDiff) * 1.04f;
                float kb = Projectile.knockBack * 0.6f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, fireballID, damage, kb, Projectile.owner);
            }

            Projectile.ai[0] -= 1f;

            // Homing
            // The item's default velocity is 28. Homing speed is intentionally a bit lower.
            CalamityUtils.HomeInOnNPC(Projectile, true, 450f, 24f, 30f);

            // Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            // Rotation
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnKill(int timeLeft)
        {
            // Stealth strikes create an eruption on hit.
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
            {
                int eruptionID = ModContent.ProjectileType<WrathwingCinder>();
                int damage = (int)(Projectile.damage * 0.375f);
                float kb = 0f;

                // Spawns 11 erupting fireballs in total.
                for (int x = -5; x <= 5; x++)
                {
                    Vector2 pos = Projectile.Center + Vector2.UnitY * Main.rand.NextFloat(44f, 60f);
                    pos.X += Main.rand.NextFloat(-14f, 14f);
                    float ySpeed = x % 2 == 0 ? -13f : -19f;
                    ySpeed *= Main.rand.NextFloat(0.85f, 1.05f);
                    Vector2 velocity = new Vector2(x, ySpeed);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity, eruptionID, damage, kb, Projectile.owner);
                }
            }

            // Spawn shrapnel dust. Code adapted from Holy Fire Bullets.
            for (int k = 0; k < 36; k++)
            {
                float scale = Main.rand.NextFloat(1.4f, 1.8f);
                Vector2 corner = Projectile.Center - Vector2.One * 2f;
                int dustID = Dust.NewDust(corner, 4, 4, DustID.CopperCoin);
                Main.dust[dustID].noGravity = false;
                Main.dust[dustID].scale = scale;
                float angleDeviation = 0.25f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                float velMult = Main.rand.NextFloat(0.08f, 0.14f);
                Vector2 shrapnelVelocity = Projectile.velocity.RotatedBy(angle) * velMult;
                Main.dust[dustID].velocity = shrapnelVelocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
