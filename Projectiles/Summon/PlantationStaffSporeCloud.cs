using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantationStaffSporeCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float RandomTexture => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.timeLeft = 600;
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.985f;
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X);
            Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 180f, 0f, 0f, 255f);

            if (Main.rand.NextBool(10))
            {
                Particle smoke = new SmallSmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), Main.rand.NextVector2Circular(1f, 1f), Color.Green, Color.DarkGreen, Utils.Remap(Projectile.timeLeft, 180f, 0f, 1.2f, 0.2f), Utils.Remap(Projectile.timeLeft, 180f, 0f, 150f, 0f));
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust sporeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 46);
                sporeDust.noGravity = true;
                sporeDust.velocity = Vector2.Zero;
                sporeDust.alpha = (int)Utils.Remap(Projectile.timeLeft, 180f, 0f, 0f, 255f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (RandomTexture == 1f)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PlantationStaffSporeCloud2").Value;
            if (RandomTexture == 2f)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PlantationStaffSporeCloud3").Value;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
