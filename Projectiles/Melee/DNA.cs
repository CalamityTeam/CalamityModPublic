using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DNA : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public static int OnHitIFrames = 3;
        public static int TotalSegments = 6;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha -= 100;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[1] = 1f;

                    // This projectile normally does not move by itself, so this will manually move it one time only
                    // This is only for the first segment
                    if (Projectile.ai[0] == 0f)
                    {
                        Projectile.ai[0]++;
                        Projectile.position += Projectile.velocity;
                    }

                    // Spawn the next segment
                    if (Main.myPlayer == Projectile.owner && Projectile.ai[0] < TotalSegments)
                    {
                        int nextSegment = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, nextSegment);
                    }
                }
            }
            else // Begin fading out
            {
                int AlphaPerFrame = 15;
                Projectile.alpha += AlphaPerFrame;
                if (Projectile.alpha == AlphaPerFrame * 14)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 234, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f, 200, default, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                    }
                }

                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Main.rand.NextBool(10))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 234, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f);
        }

        // This is essential for Vilethorn-type projectiles, as velocity is a stored parameter and isn't supposed to actually move the projectile
        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());

        public override void OnKill(int timeLeft)
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 234, Projectile.oldVelocity.X * 0.005f, Projectile.oldVelocity.Y * 0.005f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].GiveIFrames(OnHitIFrames);
        }
    }
}
