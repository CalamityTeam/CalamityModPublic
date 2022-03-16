using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Dusts;
using CalamityMod.Particles;

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
            projectile.width = 16;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 180;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.ignoreWater = true;
			projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerGrenade";

        private void Explode(bool NPCHit = false)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumplerGrenadeExplosion>(), (int)(projectile.damage), projectile.knockBack, projectile.owner, NPCHit ? 1 : 0);
            }
            projectile.Kill();
        }

        public override void AI()
        {
            if (projectile.timeLeft == 1)
                Explode();

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) && projectile.timeLeft < 165)
            {
                Explode();
                projectile.Kill();
            }
            else
            {
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 16f)
                    projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            Explode(true);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //Damage gets lowered for the main hit, most of the damage comes from the explosion itself.
            damage = (int)(damage * 0.1f);
        }
    }
}
