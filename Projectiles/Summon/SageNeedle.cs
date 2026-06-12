using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SageNeedle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const int OnDeathHealValue = 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(48f);

            // Don't collide with tiles unless the needle is falling.
            Projectile.tileCollide = Projectile.velocity.Y > 0f;
            if (Projectile.velocity.Y < 12f)
                Projectile.velocity.Y += 0.16f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SagePoison>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 6; i++)
            {
                Dust redGrass = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Grass, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
                redGrass.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 3f);
                redGrass.noGravity = true;
                redGrass.color = Color.Lerp(Color.IndianRed, Color.MediumVioletRed, Main.rand.NextFloat());
            }
        }
    }
}
