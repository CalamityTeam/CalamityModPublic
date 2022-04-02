using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaGrenadeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/PlasmaGrenade";

        private static readonly float Gravity = 0.09f;

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Grenade");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.MaxUpdates = 2;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 projectileTop = projectile.Center + new Vector2(0f, projectile.height * -0.5f).RotatedBy(projectile.rotation);

            if (Time > 10f)
                projectile.velocity.Y += Gravity;

            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustPerfect(projectileTop, 107);
                dust.velocity = projectile.rotation.ToRotationVector2().RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 4f);
                dust.velocity += projectile.velocity * 0.25f;
                dust.scale = Main.rand.NextFloat(0.95f, 1.3f);
            }
            Time++;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaGrenadeExplosion"), (int)projectile.position.X, (int)projectile.position.Y);
            if (projectile.Calamity().stealthStrike)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MassivePlasmaExplosion>(), projectile.damage, projectile.knockBack * 2f, projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 220; i++)
                    {
                        int type = Main.rand.NextBool(2) ? 261 : 107;
                        Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
                        dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                        dust.velocity = Main.rand.NextVector2CircularEdge(75f, 75f);
                        dust.noGravity = true;
                        if (type == 261)
                        {
                            dust.velocity *= 1.5f;
                        }
                    }
                }
            }
            else
            {
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 360);
                if (Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 120; i++)
                    {
                        int type = Main.rand.NextBool(3) ? 261 : (int)CalamityDusts.SulfurousSeaAcid;
                        Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
                        dust.scale = Main.rand.NextFloat(1.3f, 1.5f);
                        dust.velocity = Main.rand.NextVector2CircularEdge(15f, 15f);
                        dust.noGravity = true;
                        if (type == 261)
                        {
                            dust.velocity *= 2f;
                            dust.scale *= 1.8f;
                        }
                    }
                }
            }
        }
    }
}

