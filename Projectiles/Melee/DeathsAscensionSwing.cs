using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

        public static Asset<Texture2D> glowTexture = null;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                glowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 159;
            Projectile.height = 230;
            Projectile.scale = 1.15f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.frameCounter = 0;
        }

        public override void AI()
        {
            //Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                CurrentFrame++;
                if (frameX >= 2)
                    CurrentFrame = 0;

                if (frameX == 0 && frameY == 3)
                {
                    SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                }
                Projectile.frameCounter = 0;
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
                if (!Owner.CantUseHoldout())
                    HandleChannelMovement(playerRotatedPoint);
                else
                    Projectile.Kill();
            }

            // Rotation and directioning.
            Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            // Sprite and player directioning.
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.direction == 1)
                Projectile.Left = Owner.MountedCenter;
            else
                Projectile.Right = Owner.MountedCenter;
            Projectile.position.X += Projectile.spriteDirection == -1 ? 26f : -26f;
            Projectile.position.Y -= Projectile.scale * 2f;
            Owner.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Move the player's arm accordingly
            bool ownerFacingRight = Owner.direction == 1;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, GetArmRotation() * (ownerFacingRight ? 1 : -1) - MathHelper.PiOver2 + (ownerFacingRight ? 0 : MathHelper.Pi));

            // Player item-based field manipulation.
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Projectile.ai[2]--;            
        }

        // This hurt my soul to do, but because the swing is done with a sprite animation, tracking specific visual rotation of the weapon is nigh impossible
        public float GetArmRotation()
        {
            return (frameX, frameY) switch
            {
                (0, 0) => -2.12f,
                (0, 1) => -2.59f,
                (0, 2) => -2.29f,
                (0, 3) => -0.97f,
                (0, 4) => 0.75f,
                (0, 5) => 1.07f,
                (1, 0) => 1.4f,
                (1, 1) => 1.14f,
                (1, 2) => 0.85f,
                (1, 3) => 0.4f,
                (1, 4) => -0.28f,
                (1, 5) => -1.8f,
                _ => 0
            };
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
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + (Projectile.spriteDirection == -1 ? new Vector2(60, 0) : new Vector2(-60, 0)) - Projectile.velocity;
            Vector2 origin = texture.Size() / new Vector2(2f, 6f) * 0.5f;
            Rectangle frame = texture.Frame(2, 6, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color playerPos = Lighting.GetColor(Main.LocalPlayer.position.ToTileCoordinates());
            Main.EntitySpriteDraw(texture, position, frame, playerPos, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTexture.Value, position, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 170);

        // Deal damage only once it starts swinging down and then going back up 
        public override bool? CanDamage() => ((frameX == 0 && frameY >= 3) || frameX == 1);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int maxRifts = 1;
            if (Projectile.ai[2] <= 0 && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<DeathsAscensionRift>()] < maxRifts)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + Main.rand.NextVector2Circular(28, 28), Vector2.Zero, ModContent.ProjectileType<DeathsAscensionRift>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Main.rand.NextFloat(0, 3f));
                Main.projectile[p].rotation = Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
                SoundEngine.PlaySound(SoundID.Item165 with { Pitch = -1 }, Projectile.Center);
                Projectile.ai[2] = 40;
                float screenShakePower = 3 * Utils.GetLerpValue(1300f, 0f, target.Distance(Main.LocalPlayer.Center), true);
                Main.LocalPlayer.SetScreenshake(screenShakePower);
            }
        }
    }
}
