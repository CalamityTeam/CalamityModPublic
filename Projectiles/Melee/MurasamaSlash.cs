using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MurasamaSlash : ModProjectile
    {
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
        public bool Slashing => CurrentFrame % 7 == 0 && projectile.frameCounter % 3 == 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Murasama");
        }

        public override void SetDefaults()
        {
            projectile.width = 236;
            projectile.height = 180;
            projectile.scale = 2f;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ownerHitCheck = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 21;
            projectile.frameCounter = 0;
            projectile.Calamity().trueMelee = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.frameCounter <= 1)
                return false;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / new Vector2(2f, 7f) * 0.5f;
            Rectangle frame = texture.Frame(2, 7, frameX, frameY);
            SpriteEffects spriteEffects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            //Frames and crap
            projectile.frameCounter++;
            if (projectile.frameCounter % 3 == 0)
            {
                CurrentFrame++;
                if (frameX >= 2)
                    CurrentFrame = 0;
            }

            // Play a "droning" noise every so often.
            if (Slashing)
            {
                Main.PlaySound(SoundID.Item15, projectile.Center);
            }

            // Create idle light and dust.
            Vector2 origin = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(origin, 3f, 0.2f, 0.2f);
            if (Main.rand.NextBool(3))
            {
                int redDust = Dust.NewDust(origin - projectile.Size / 2f, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.velocity.X, projectile.velocity.Y, 100, default, 2f);
                Main.dust[redDust].noGravity = true;
                Main.dust[redDust].position -= projectile.velocity;
            }

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                    HandleChannelMovement(player, playerRotatedPoint);
                else
                    projectile.Kill();
            }

            // Rotation and directioning.
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.rotation = velocityAngle + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            float offset = 80f * projectile.scale;
            projectile.position = playerRotatedPoint - projectile.Size * 0.5f + velocityAngle.ToRotationVector2() * offset;

            // Sprite and player directioning.
            projectile.spriteDirection = projectile.direction;
            player.ChangeDir(projectile.direction);

            // Prevents the projectile from dying
            projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.ActiveItem().shoot == projectile.type)
            {
                speed = player.ActiveItem().shootSpeed * projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;    

            // Sync if a velocity component changes.
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
            {
                projectile.netUpdate = true;
            }
            projectile.velocity = newVelocity;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 0, 0, 0);

        //public override bool CanDamage() => Slashing;
    }
}
