using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MurasamaSlash : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Murasama>();
        public int frameX = 0;
        public int frameY = 0;

        public int CurrentFrame
        {
            get => frameX * 7 + frameY;
            set
            {
                frameX = value / 7;
                frameY = value % 7;
            }
        }

        // A "slash" is only present during 2 specific frames (ones with a slash effect) right before they transition to the next frame.
        // Note: This bool is unused. Murasama formerly only dealt damage when on these frames, but it created a few issues with player usability.
        public bool Slashing => CurrentFrame % 7 == 0 && Projectile.frameCounter % 3 == 2;

        public override void SetDefaults()
        {
            Projectile.width = 236;
            Projectile.height = 180;
            Projectile.scale = 2f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 21;
            Projectile.frameCounter = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frameCounter <= 1)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / new Vector2(2f, 7f) * 0.5f;
            Rectangle frame = texture.Frame(2, 7, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            //Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
            {
                CurrentFrame++;
                if (frameX >= 2)
                    CurrentFrame = 0;
            }

            // Play a "droning" noise every so often.
            if (Slashing)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
            }

            // Create idle light and dust.
            Vector2 origin = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(origin, 3f, 0.2f, 0.2f);
            if (Main.rand.NextBool(3))
            {
                int redDust = Dust.NewDust(origin - Projectile.Size / 2f, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 2f);
                Main.dust[redDust].noGravity = true;
                Main.dust[redDust].position -= Projectile.velocity;
            }

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                    HandleChannelMovement(player, playerRotatedPoint);
                else
                    Projectile.Kill();
            }

            // Rotation and directioning.
            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            float offset = 80f * Projectile.scale;
            Projectile.position = playerRotatedPoint - Projectile.Size * 0.5f + velocityAngle.ToRotationVector2() * offset;

            // Sprite and player directioning.
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.ActiveItem().shoot == Projectile.type)
            {
                speed = player.ActiveItem().shootSpeed * Projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            // Sync if a velocity component changes.
            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 0, 0, 0);

        //public override bool? CanDamage() => Slashing;
    }
}
