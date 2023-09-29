using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodBreath : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 40;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 9;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Emit blood red light.
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.77f, Projectile.Opacity * 0.15f, Projectile.Opacity * 0.08f);

            // Create blood dust.
            Time++;
            if (Time > 7f)
            {
                float dustScale = 1f;
                switch ((int)Time)
                {
                    case 8:
                        dustScale = 0.25f;
                        break;
                    case 9:
                        dustScale = 0.5f;
                        break;
                    case 10:
                        dustScale = 0.75f;
                        break;
                }
                Time++;
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 3f;
                        dust.velocity *= 2f;
                    }
                    else
                    {
                        dust.scale *= 1.5f;
                    }
                    dust.velocity *= 1.2f;
                    dust.scale *= dustScale;
                }
            }
            Projectile.rotation += 0.3f * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
    }
}
