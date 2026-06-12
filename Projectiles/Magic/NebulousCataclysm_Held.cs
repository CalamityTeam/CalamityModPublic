using System;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulousCataclysm_Held : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<NebulousCataclysm>();
        private const int TotalXFrames = 2;
        private const int TotalYFrames = 8;
        private const int FrameTimer = 6;
        private const int ShootFrame = 3;

        public int frameX = 0;
        public int frameY = 0;

        public int CurrentFrame
        {
            get => frameX * TotalYFrames + frameY;
            set
            {
                frameX = value / TotalYFrames;
                frameY = value % TotalYFrames;
            }
        }

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 124;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % FrameTimer == 0)
            {
                CurrentFrame++;
                if (frameX >= TotalXFrames)
                    CurrentFrame = 0;
            }

            Lighting.AddLight(Projectile.Center, 1.25f, 0f, 0.2f);

            if (CurrentFrame != ShootFrame)
                Projectile.ai[0] = 0f;

            bool shoot = CurrentFrame == ShootFrame && Projectile.ai[0] == 0f;
            bool ableToShoot = false;
            bool weaponInUse = !Owner.CantUseHoldout();
            Vector2 halvedSize = Projectile.Size / 2f;
            Vector2 staffOffset = halvedSize + new Vector2(24f, 24f);

            if (shoot)
            {
                Projectile.ai[0] = 1f;

                if (weaponInUse)
                {
                    if (Owner.CheckMana(Owner.HeldItem.mana))
                        ableToShoot = true;
                    else
                        Projectile.Kill();

                    if (ableToShoot)
                        SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
                }
                else
                    Projectile.Kill();

                if (Main.myPlayer == Projectile.owner && ableToShoot)
                {
                    Owner.CheckMana(Owner.HeldItem.mana, true);

                    int projectileType = ModContent.ProjectileType<NebulaCloudCore>();
                    float coreVelocity = 8f;
                    int weaponDamage = Owner.GetWeaponDamage(Owner.HeldItem);
                    float weaponKnockback = Owner.HeldItem.knockBack;
                    if (weaponInUse)
                    {
                        weaponKnockback = Owner.GetWeaponKnockback(Owner.HeldItem, weaponKnockback);
                        float scaleFactor = Owner.HeldItem.shootSpeed * Projectile.scale;
                        Vector2 projectileSpawnPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                        Vector2 projectileDestination = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - projectileSpawnPosition;

                        if (Owner.gravDir == -1f)
                            projectileDestination.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - projectileSpawnPosition.Y;

                        Vector2 velocity = Vector2.Normalize(projectileDestination);
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                            velocity = -Vector2.UnitY;

                        velocity *= scaleFactor;
                        if (velocity.X != Projectile.velocity.X || velocity.Y != Projectile.velocity.Y)
                            Projectile.netUpdate = true;

                        Projectile.velocity = velocity * 0.5f;

                        Vector2 projectileVelocity = Vector2.Normalize(Projectile.velocity) * coreVelocity;
                        if (float.IsNaN(projectileVelocity.X) || float.IsNaN(projectileVelocity.Y))
                            projectileVelocity = -Vector2.UnitY;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectileSpawnPosition + Vector2.Normalize(projectileVelocity) * staffOffset.Length(), projectileVelocity, projectileType, weaponDamage, weaponKnockback, Projectile.owner);
                    }
                    else
                        Projectile.Kill();
                }
            }

            Projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - halvedSize + Vector2.Normalize(Projectile.velocity) * staffOffset;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.PiOver4 * 3f : MathHelper.PiOver4);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * Projectile.direction), (double)(Projectile.velocity.X * Projectile.direction));
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.frameCounter < 5)
                return;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / new Vector2(TotalXFrames, TotalYFrames) * 0.5f;
            Rectangle frame = texture.Frame(TotalXFrames, TotalYFrames, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, position, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override bool? CanDamage() => false;
    }
}
