using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squash Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerGrenade";

        private void Explode(bool NPCHit = false)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumplerGrenadeExplosion>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, NPCHit ? 1 : 0);
            }
            Projectile.Kill();
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 1)
                Explode();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = Projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) && Projectile.timeLeft < 165)
            {
                Explode();
                Projectile.Kill();
            }
            else
            {
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Explode(true);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //Damage gets lowered for the main hit, most of the damage comes from the explosion itself.
            damage = (int)(damage * 0.1f);
        }
    }
}
