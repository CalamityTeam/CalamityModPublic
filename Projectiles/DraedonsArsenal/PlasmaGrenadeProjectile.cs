using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaGrenadeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/PlasmaGrenade";

        private static readonly float Gravity = 0.09f;
        private float rotate = Main.rand.Next(360);

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 450;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(rotate);
            Vector2 projectileTop = Projectile.Center + new Vector2(0f, Projectile.height * -0.5f).RotatedBy(Projectile.rotation);

            if (Time > 10f)
                Projectile.velocity.Y += Gravity;

            if (!Main.dedServ && Main.rand.NextBool()) // 50% chance to spawn smoke
            {
                Color plasmaLime = Color.Lerp(Color.Lime, Color.LimeGreen, Main.rand.NextFloat(1f));
                Color fadeColor = Color.Lerp(Color.LightGreen, plasmaLime, Main.rand.NextFloat(0.5f,1f));
                Particle plasma = new SmallSmokeParticle(projectileTop, Projectile.oldVelocity*0.7f, Color.LightGreen, fadeColor, Main.rand.NextFloat(0.4f,0.9f), 100f);
                GeneralParticleHandler.SpawnParticle(plasma);
            }
            Time++;
            rotate += 10;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(PlasmaGrenade.ExplosionSound, Projectile.position);
            if (Projectile.Calamity().stealthStrike)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MassivePlasmaExplosion>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 220; i++)
                    {
                        int type = Main.rand.NextBool() ? 261 : 107;
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
                Projectile.ExpandHitboxBy(360);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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

