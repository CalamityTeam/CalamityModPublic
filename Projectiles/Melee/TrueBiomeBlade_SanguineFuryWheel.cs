using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class SanguineFuryWheel : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (SanguineFury.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[Projectile.owner];

        public float Timer => MaxTime - Projectile.timeLeft;

        public const float MaxTime = 120;



        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 45;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;


            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.95f;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 84 * Projectile.scale;
            float bladeWidth = 76 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - direction * bladeLength / 2, Projectile.Center + direction * bladeLength / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                initialized = true;

                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();

                Projectile.timeLeft = (int)MaxTime;
                Projectile.velocity = direction * 16f;

                Projectile.scale = 1f + ShredRatio; //SWAGGER
                Projectile.netUpdate = true;

            }

            Projectile.velocity *= 0.96f;
            Projectile.position += Projectile.velocity;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.SuperPogoAttunement_WheelProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra").Value;

                float drawAngleWheel = MathHelper.WrapAngle(i * MathHelper.PiOver2 + (Main.GlobalTimeWrappedHourly * 6));
                Vector2 drawOrigin = new Vector2(0f, tex.Height);
                Vector2 drawPositionWheel = Projectile.Center - drawAngleWheel.ToRotationVector2() * 20f - Main.screenPosition;

                //Vector2 drawPosition = Vector2.Lerp(drawPositionShot, drawPositionWheel, transition);

                float opacityFade = Projectile.timeLeft > 15 ? 1 : Projectile.timeLeft / 15f;

                Main.EntitySpriteDraw(tex, drawPositionWheel, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, drawAngleWheel, drawOrigin, Projectile.scale * 0.85f, 0f, 0);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.Crimson : Color.DarkRed, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }
        }
    }
}
