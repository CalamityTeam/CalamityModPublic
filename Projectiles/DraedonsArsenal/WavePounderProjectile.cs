using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class WavePounderProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/WavePounder";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.velocity.X != 0f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else Projectile.rotation = MathHelper.Pi;

            if (Projectile.velocity.Y < 12f)
                Projectile.velocity.Y += 0.35f;

            // Generate some dust that moves towards the bomb to show that it's sucking in energy.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 2; i++)
                {
                    float offset = Main.rand.NextFloat(38f, 42f);
                    if (Projectile.Calamity().stealthStrike)
                        offset *= 1.66f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(offset, offset), 107);
                    dust.velocity = Projectile.DirectionFrom(dust.position) * offset / 12f + Projectile.velocity;
                    dust.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(TeslaCannon.FireSound, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Projectile.Calamity().stealthStrike)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WavePounderBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        explosion.ai[1] = Main.rand.NextFloat(110f, 200f) + i * 20f; // Randomize the maximum radius.
                        explosion.localAI[1] = Main.rand.NextFloat(0.18f, 0.3f); // And the interpolation step.
                        explosion.netUpdate = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WavePounderBoom>(), (int)(Projectile.damage * 0.3), Projectile.knockBack, Projectile.owner);
                        if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            explosion.ai[1] = Main.rand.NextFloat(320f, 870f) + i * 45f; // Randomize the maximum radius.
                            explosion.localAI[1] = Main.rand.NextFloat(0.08f, 0.25f); // And the interpolation step.
                            explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, i / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
                            explosion.Calamity().stealthStrike = true;
                            explosion.netUpdate = true;
                        }
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}

