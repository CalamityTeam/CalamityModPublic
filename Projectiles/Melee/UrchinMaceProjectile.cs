using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinMaceProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/UrchinMace";

        public const float MaxWindup = 70;
        public ref float Windup => ref Projectile.ai[0];
        public float WindupProgress => MathHelper.Clamp(Windup, 0, MaxWindup) / MaxWindup;

        public ref float SoundPlayed => ref Projectile.localAI[0];

        Vector2 offset;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Mace");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 70 * Projectile.scale;
            float bladeWidth = 30 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation.ToRotationVector2() * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (offset == null)
                offset = Vector2.Zero;

            if (Owner.controlUp)
            {
                offset.X -= 0.5f;
            }
            if (Owner.controlDown)
            {
                offset.X += 0.5f;
            }

            Main.mouseText = true;
            Main.instance.MouseText(offset.X.ToString());

            Projectile.rotation += WindupProgress * MathHelper.PiOver4 / 23f;


            if (Owner.channel)
            {
                Projectile.timeLeft = 2;
                UpdateOwnerVars();
            }

            Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 0f + Vector2.UnitY * 4;

            if (Owner.direction < 0) //My god why is Owner.direction not properly being updated.
                Projectile.Center += Vector2.UnitX * 12;

            if (Projectile.rotation < 0f)
            {
                SoundPlayed = 0f;
            }
            else if (SoundPlayed == 0f)
            {
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Owner.Center);
                SoundPlayed = 1f;
            }

            Windup++;
        }

        public void UpdateOwnerVars()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D maceTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D whirlpoolTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/RedtideWhirlpool").Value;

            float whirlpoolScale = WindupProgress * 2.4f;
            float whirlpoolOpacity = WindupProgress * 0.2f;

            Main.spriteBatch.Draw(whirlpoolTexture, Owner.Center - Main.screenPosition, null, Lighting.GetColor((int)Owner.Center.X / 16, (int)Owner.Center.Y / 16) * whirlpoolOpacity, Projectile.rotation + Windup * 0.1f, whirlpoolTexture.Size() / 2f, whirlpoolScale, SpriteEffects.None, 0);

            Vector2 handleOrigin = new Vector2(0, maceTexture.Height);
            float maceRotation = Projectile.rotation + MathHelper.PiOver4;
            Main.spriteBatch.Draw(maceTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), maceRotation, handleOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Spawn a whirlpool typhoon after sending it out

            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 172, vector7.X * 2f, vector7.Y * 2f, 100, default, 1.4f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
