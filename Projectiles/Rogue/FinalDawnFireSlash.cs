using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnFireSlash : ModProjectile
    {
        public bool HasRegeneratedStealth = false;
        public const float StealthReturnRatio = 0.25f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[Projectile.type] = 11;
        }
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 398;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player is null || player.dead)
                Projectile.Kill();

            player.direction = Projectile.direction;
            player.heldProj = Projectile.whoAmI;

            AdjustPlayerPositionValues(player);

            Projectile.ai[0]++;
            if(Projectile.ai[0] >= 4)
            {
                Projectile.ai[1]++;
                Projectile.ai[0] = 0;
                if (Projectile.ai[1] == 5)
                {
                    Projectile.friendly = true;
                    SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                }
            }
            if (Projectile.ai[1] >= 11)
            {
                Projectile.Kill();
                return;
            }
            Projectile.frame = (int)Projectile.ai[1];

            AdjustPlayerItemFrameValues(player);
        }
        public void AdjustPlayerPositionValues(Player player)
        {
            Projectile.Center = player.Center;
            Projectile.position.X += 60 * player.direction;
            Projectile.position.Y -= 30;
        }

        public void AdjustPlayerItemFrameValues(Player player)
        {
            if (Projectile.ai[1] < 5)
            {
                player.bodyFrame.Y = player.bodyFrame.Height;
            }
            else if (Projectile.ai[1] < 8)
            {
                player.bodyFrame.Y = 3 * player.bodyFrame.Height;
            }
            else
            {
                player.bodyFrame.Y = 4 * player.bodyFrame.Height;
            }

            Projectile.spriteDirection = player.direction;
            player.heldProj = Projectile.whoAmI;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CalamityPlayer calamityPlayer = Main.player[Projectile.owner].Calamity();
            // Restore stealth
            if (!HasRegeneratedStealth)
            {
                calamityPlayer.rogueStealth += calamityPlayer.rogueStealthMax * StealthReturnRatio;
                if (calamityPlayer.rogueStealth > calamityPlayer.rogueStealthMax)
                    calamityPlayer.rogueStealth = calamityPlayer.rogueStealthMax;
                HasRegeneratedStealth = true;
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int width = 918 / 3;
            int height = 1990 / 5;

            Vector2 drawCenter = Projectile.Center;
            Rectangle frameRectangle = new Rectangle(Projectile.frame / 5 * width, Projectile.frame % 5 * height, width, height);

            Texture2D scytheTexture = Main.projectileTexture[Projectile.type];
            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/FinalDawnFireSlash_Glow");

            Main.spriteBatch.Draw(scytheTexture,
                                  drawCenter - Main.screenPosition,
                                  frameRectangle,
                                  Projectile.GetAlpha(lightColor),
                                  Projectile.rotation,
                                  frameRectangle.Size() / 2,
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowTexture,
                                  drawCenter - Main.screenPosition,
                                  frameRectangle,
                                  Projectile.GetAlpha(Color.White),
                                  Projectile.rotation,
                                  frameRectangle.Size() / 2,
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
    }
}
