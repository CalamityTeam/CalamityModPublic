using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class ChaoticGeyser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaotic Geyser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];
            int cap = 3;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.1f;
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * -0.5f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            int num199 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[num199];
            dust.position.X -= 2f;
            dust.position.Y += 2f;
            dust.scale += (float)Main.rand.Next(50) * 0.01f;
            dust.noGravity = true;
            dust.velocity.Y -= 16f;
            if (Main.rand.NextBool(2))
            {
                int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 1f);
                Dust dust2 = Main.dust[num200];
                dust2.position.X -= 2f;
                dust2.position.Y += 2f;
                dust2.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                dust2.noGravity = true;
                dust2.velocity *= 0.8f;
            }
            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
