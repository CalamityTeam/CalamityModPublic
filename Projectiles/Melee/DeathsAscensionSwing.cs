using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DeathsAscensionSwing : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DeathsAscension>();
        public int frameX = 0;
        public int frameY = 0;

        public int CurrentFrame
        {
            get => frameX * 6 + frameY;
            set
            {
                frameX = value / 6;
                frameY = value % 6;
            }
        }
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 159;
            Projectile.height = 230;
            Projectile.scale = 1.15f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.frameCounter = 0;
        }

        public override void AI()
        {
            //Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
            {
                CurrentFrame++;
                if (frameX >= 2)
                    CurrentFrame = 0;
            }

            if (frameX == 0 && frameY == 3 && Projectile.frameCounter % 3 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
            }

            if ((frameX == 0 && frameY >= 3) || (frameX == 1 && frameY <= 1))
            {
                Projectile.idStaticNPCHitCooldown = 8;
            }
            else if (frameX == 1 && frameY > 1)
            {
                Projectile.idStaticNPCHitCooldown = 12;
            }

            Vector2 playerRotatedPoint = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Owner.channel && !Owner.noItems && !Owner.CCed)
                    HandleChannelMovement(playerRotatedPoint);
                else
                    Projectile.Kill();
            }

            // Rotation and directioning.
            Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            // Sprite and player directioning.
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.direction == 1)
                Projectile.Left = Owner.Center;
            else
                Projectile.Right = Owner.Center;
            Projectile.position.X += Projectile.spriteDirection == -1 ? 26f : -26f;
            Projectile.position.Y -= Projectile.scale * 2f;
            Owner.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void HandleChannelMovement(Vector2 playerRotatedPoint)
        {
            Vector2 newVelocity = Vector2.UnitX * (Main.MouseWorld.X > playerRotatedPoint.X).ToDirectionInt();

            // Sync if a velocity component changes.
            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
                Projectile.netUpdate = true;

            Projectile.velocity = newVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frameCounter <= 1)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition + (Projectile.spriteDirection == -1 ? new Vector2(60, 0) : new Vector2(-60, 0));
            Vector2 origin = texture.Size() / new Vector2(2f, 6f) * 0.5f;
            Rectangle frame = texture.Frame(2, 6, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, position, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 170);

        // Deal damage only once it starts swinging down and then going back up 
        public override bool? CanDamage() => ((frameX == 0 && frameY >= 3) || frameX == 1) && Projectile.frameCounter > 6;
    }
}
