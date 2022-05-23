using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class EquanimityDarkShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Shard");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += 0.4f * Projectile.direction;
            if (Projectile.timeLeft < 130)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    velocityNew = Main.npc[index].Center - Projectile.Center;
                    velocityNew.Normalize();
                    Projectile.velocity += velocityNew;
                    if (Projectile.velocity.Length() > 10f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10f;
                    }
                }
            }

            if (Projectile.timeLeft < 51)
            {
                Projectile.alpha += 5;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 130)
            {
                return null;
            }
            else
            {
                return false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return true;
        }
    }
}
