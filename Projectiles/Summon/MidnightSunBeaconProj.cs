using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MidnightSunBeaconProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Summon/MidnightSunBeacon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midnight Sun Beacon");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.08f);
            if (Math.Abs(Projectile.rotation + MathHelper.PiOver4) < 0.02f && Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] = 85f;
                Projectile.ai[0] = 1f;
            }

            if (Projectile.ai[1] == 1f)
            {
                int p = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.UnitY * 30f, ModContent.ProjectileType<MidnightSunUFO>(), Projectile.damage, Projectile.knockBack,
                    Projectile.owner);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Projectile.originalDamage;
                Projectile.Kill();
            }
            if (Projectile.ai[1] > 0)
                Projectile.ai[1]--;
            if (Projectile.ai[1] > 1f &&
                Projectile.ai[1] <= 60f)
            {
                Projectile.velocity.Y -= 0.4f;
            }
            else
                Projectile.velocity *= 0.96f;
        }

        public override bool? CanDamage() => false;
    }
}
