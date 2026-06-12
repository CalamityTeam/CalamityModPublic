using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Spikecrag : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 40;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += 0.5f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
                return;
            }

            float maxDistance = 1000f;
            bool homeIn = false;

            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(n.width / 2) + (n.height / 2);

                    if (Vector2.Distance(n.Center, Projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(Projectile.Center, Projectile.width, Projectile.height, n.Center, n.width, n.height))
                    {
                        homeIn = true;
                        break;
                    }
                }
            }

            if (Projectile.owner == Main.myPlayer && homeIn)
            {
                Projectile.ai[1] += 1f;
                if ((Projectile.ai[1] % 10f) == 0f)
                {
                    int amount = Main.rand.Next(1, 3);
                    if (DownedBossSystem.downedProvidence && Main.zenithWorld) // spikecrag gfb buff
                    {
                        amount += 6;
                    }
                    for (int i = 0; i < amount; i++)
                    {
                        float velocityX = Main.rand.NextFloat(-10f, 10f);
                        float velocityY = Main.rand.NextFloat(-15f, -8f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPosition.X + (Projectile.width / 2), Projectile.oldPosition.Y + (Projectile.height / 2), velocityX, velocityY, ModContent.ProjectileType<SpikecragSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool? CanDamage() => false;
    }
}
