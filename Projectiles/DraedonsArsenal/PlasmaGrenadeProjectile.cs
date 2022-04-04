using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaGrenadeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/PlasmaGrenade";

        private static readonly float Gravity = 0.09f;

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Grenade");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 2;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 projectileTop = Projectile.Center + new Vector2(0f, Projectile.height * -0.5f).RotatedBy(Projectile.rotation);

            if (Time > 10f)
                Projectile.velocity.Y += Gravity;

            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustPerfect(projectileTop, 107);
                dust.velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 4f);
                dust.velocity += Projectile.velocity * 0.25f;
                dust.scale = Main.rand.NextFloat(0.95f, 1.3f);
            }
            Time++;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/PlasmaGrenadeExplosion"), (int)Projectile.position.X, (int)Projectile.position.Y);
            if (Projectile.Calamity().stealthStrike)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MassivePlasmaExplosion>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 220; i++)
                    {
                        int type = Main.rand.NextBool(2) ? 261 : 107;
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
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
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 360);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 120; i++)
                    {
                        int type = Main.rand.NextBool(3) ? 261 : (int)CalamityDusts.SulfurousSeaAcid;
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
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

