using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlagueArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlagueExplosionFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                for (int i = 0; i < 6; i++)
                {
                    if (i % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 projPos = Projectile.position;
                        Vector2 projVel = Projectile.oldVelocity;
                        projVel.Normalize();
                        projVel *= 8f;
                        float beeVelX = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float beeVelY = (float)Main.rand.Next(-35, 36) * 0.01f;
                        projPos -= projVel * (float)i;
                        beeVelX += Projectile.oldVelocity.X / 6f;
                        beeVelY += Projectile.oldVelocity.Y / 6f;
                        int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projPos.X, projPos.Y, beeVelX, beeVelY, Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage / 2), Main.player[Projectile.owner].beeKB(0f), Main.myPlayer);
                        if (bee.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[bee].penetrate = 2;
                            Main.projectile[bee].DamageType = DamageClass.Ranged;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
