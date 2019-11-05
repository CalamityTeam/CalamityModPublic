using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingBettyProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Betty");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.penetrate = 3;
            projectile.Calamity().rogue = true;
        }
        //This is definitely not a near copy of the Blast Barrel nosiree :>
        private void Explode()
        {
            Main.PlaySound(2, projectile.Center, 14);
            //This is definitely not a near copy of the Blast Barrel nosiree :>
            bool stealthS = projectile.Calamity().stealthStrike;
            int projectileCount = 2;
            if (stealthS)
            {
                projectileCount += 2;
            }
            for (int i = 0; i < projectileCount; i++)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-7, 4f))).RotatedByRandom((double)MathHelper.ToRadians(30f));
                    Projectile.NewProjectile(projectile.Center, projectile.velocity + shrapnelVelocity,
                        ModContent.ProjectileType<BouncingBettyShrapnel>(), BouncingBetty.BaseDamage, 3f, projectile.owner);
                }
                else
                {
                    Vector2 fireVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-7, 4f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int fireIndex = Projectile.NewProjectile(projectile.Center, projectile.velocity + fireVelocity,
                        Main.rand.Next(ProjectileID.MolotovFire, ProjectileID.MolotovFire3 + 1),
                        BlastBarrel.BaseDamage, 1f, projectile.owner);
                    Main.projectile[fireIndex].thrown = false;
                    Main.projectile[fireIndex].Calamity().forceRogue = true;
                    Main.projectile[fireIndex].penetrate = -1;
                    Main.projectile[fireIndex].usesLocalNPCImmunity = true;
                    Main.projectile[fireIndex].localNPCHitCooldown = 9;
                    Main.projectile[fireIndex].timeLeft = 120;
                }
            }
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) &&
                projectile.timeLeft < 575)
            {
                Explode();
                projectile.Kill();
            }
            else
            {
                projectile.velocity.Y += 0.4f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(2, projectile.Center, 14);
            Explode();
            projectile.velocity *= -1f;
        }
    }
}