using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AtlantisSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public bool devourer = DownedBossSystem.downedDoG;
        public static int TotalSegments = 20;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = (Main.zenithWorld && devourer) ? 1 : 0;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = (Main.zenithWorld && devourer) ? 2 : 6;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha -= 100;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[1] = 1f;

                    // This projectile normally does not move by itself, so this will manually move it one time only
                    // This is only for the first segment and the on-kill segments
                    if (Projectile.ai[0] == 0f || Projectile.ai[0] > TotalSegments)
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
                int AlphaPerFrame = 12;
                Projectile.alpha += AlphaPerFrame;
                if (Projectile.alpha == AlphaPerFrame * 14)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Dust blue = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 206, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f, 200, default, 1f);
                        blue.noGravity = true;
                        blue.velocity *= 0.5f;
                    }
                }

                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 206, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f);
        }

        // This is essential for Vilethorn-type projectiles, as velocity is a stored parameter and isn't supposed to actually move the projectile
        public override bool ShouldUpdatePosition() => false;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 206, Projectile.oldVelocity.X * 0.005f, Projectile.oldVelocity.Y * 0.005f);
            }

            // Prevent recursion: the segments that are being spawned here will deliberately be set higher than total segments
            if (Projectile.ai[0] > TotalSegments || Main.myPlayer != Projectile.owner)
                return;

            // Spawn two ungrowing segments to either side on death
            int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            for (int i = 0; i < numProj; i++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, TotalSegments + 1f);
            }
        }
    }
}
