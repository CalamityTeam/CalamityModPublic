using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class WavePounderProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/WavePounder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Pounder");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.velocity.X != 0f)
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else projectile.rotation = MathHelper.Pi;

            if (projectile.velocity.Y < 12f)
                projectile.velocity.Y += 0.35f;

            // Generate some dust that moves towards the bomb to show that it's sucking in energy.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 2; i++)
                {
                    float offset = Main.rand.NextFloat(38f, 42f);
                    if (projectile.Calamity().stealthStrike)
                        offset *= 1.66f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2CircularEdge(offset, offset), 107);
                    dust.velocity = projectile.DirectionFrom(dust.position) * offset / 12f + projectile.velocity;
                    dust.noGravity = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), projectile.Center);
            if (Main.myPlayer == projectile.owner)
            {
                if (!projectile.Calamity().stealthStrike)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WavePounderBoom>(), projectile.damage, projectile.knockBack, projectile.owner);
                        explosion.ai[1] = Main.rand.NextFloat(110f, 200f) + i * 20f; // Randomize the maximum radius.
                        explosion.localAI[1] = Main.rand.NextFloat(0.18f, 0.3f); // And the interpolation step.
                        explosion.netUpdate = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WavePounderBoom>(), (int)(projectile.damage * 0.3), projectile.knockBack, projectile.owner);
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
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}

