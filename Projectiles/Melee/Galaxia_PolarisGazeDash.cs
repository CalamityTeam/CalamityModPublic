using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class PolarisGazeDash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => 20 - Projectile.timeLeft;

        public Vector2 DashStart;
        public Vector2 DashEnd;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polaris Piercing Lunge");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), DashStart, DashEnd);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = true;

            Particle bloom = new StrongBloom(target.Center, target.velocity, Color.Crimson * 0.5f, 1f, 30);
            GeneralParticleHandler.SpawnParticle(bloom);

            for (int i = 0; i < 3; i++)
            {
                Vector2 sparkSpeed = target.DirectionTo(Owner.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 9f;
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.CornflowerBlue, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }

            //Explode into cosmic bolts
            for (int i = 0; i < 3; i++)
            {
                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Owner.SafeDirectionTo(target.Center, Vector2.Zero).RotatedByRandom(MathHelper.PiOver4) * 30f, ProjectileType<GalaxiaBolt>(), (int)(FourSeasonsGalaxia.PolarisAttunement_SlashBoltsDamage * Owner.GetDamage(DamageClass.Melee).Base), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.01f);
                {
                    blast.timeLeft = 100;
                }
            }
        }



        public override bool PreDraw(ref Color lightColor) //OMw to reuse way too much code from the entangling vines
        {

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D lineTex = Request<Texture2D>("CalamityMod/Particles/ThinEndedLine").Value;

            Vector2 Shake = Projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - Projectile.timeLeft / 5f) * 0.5f;
            float bump = (float)Math.Sin(((20f - Projectile.timeLeft) / 20f) * MathHelper.Pi);
            float raise = (float)Math.Sin(((20f - Projectile.timeLeft) / 20f) * MathHelper.PiOver2);

            float rot = (DashEnd - DashStart).ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(lineTex.Width / 2f, lineTex.Height);
            Vector2 scale = new Vector2(0.2f, (DashEnd - DashStart).Length() / lineTex.Height);

            Main.EntitySpriteDraw(lineTex, DashStart - Main.screenPosition + Shake, null, Color.Lerp(Color.White, Color.CornflowerBlue * 0.7f, raise), rot, origin, scale, SpriteEffects.None, 0);

            Texture2D sparkTexture = Request<Texture2D>("CalamityMod/Particles/ThinSparkle").Value;
            Texture2D bloomTexture = Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)sparkTexture.Width / (float)bloomTexture.Height;


            Rectangle frame = new Rectangle(0, 0, 14, 14);

            Main.EntitySpriteDraw(bloomTexture, DashEnd - Main.screenPosition, null, Color.CornflowerBlue * bump * 0.5f, 0, bloomTexture.Size() / 2f, bump * 6f * properBloomSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkTexture, DashEnd - Main.screenPosition, frame, Color.Lerp(Color.White, Color.CornflowerBlue, raise) * bump, raise * MathHelper.TwoPi, frame.Size() / 2f, bump * 3f, SpriteEffects.None, 0);

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
