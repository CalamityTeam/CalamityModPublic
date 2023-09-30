using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AtaraxiaSide : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            DrawOffsetX = -28;
            DrawOriginOffsetY = -2;
            DrawOriginOffsetX = 12;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Light
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);

            // Spawn dust with a 3/4 chance
            if (Main.rand.Next(4) != 3)
            {
                int idx = Dust.NewDust(Projectile.Center, 1, 1, 70);
                Main.dust[idx].position = Projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.25f;
            }

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }

        // Spawns 6 smaller projectiles that slowly glide outward and ignore iframes
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

            // Individual split projectiles deal 5% damage per hit.
            int numSplits = 6;
            int splitID = ModContent.ProjectileType<AtaraxiaSplit>();
            int damage = (int)(Projectile.damage * 0.05f);
            float angleVariance = MathHelper.TwoPi / numSplits;
            Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numSplits; ++i)
            {
                projVec = projVec.RotatedBy(angleVariance);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projVec, splitID, damage, 1.5f, Main.myPlayer);
            }
        }
    }
}
