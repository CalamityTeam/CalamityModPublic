using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnFireSlash : ModProjectile
    {
        public bool HasRegeneratedStealth = false;
        public const float StealthReturnRatio = 0.25f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[projectile.type] = 11;
        }
        public override void SetDefaults()
        {
            projectile.width = 300;
            projectile.height = 398;
            projectile.friendly = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player is null || player.dead)
                projectile.Kill();

            player.direction = projectile.direction;
            player.heldProj = projectile.whoAmI;

            AdjustPlayerPositionValues(player);

            projectile.ai[0]++;
            if(projectile.ai[0] >= 4)
            {
                projectile.ai[1]++;
                projectile.ai[0] = 0;
                if (projectile.ai[1] == 5)
                {
                    projectile.friendly = true;
                    Main.PlaySound(SoundID.Item71, projectile.Center);
                }
            }
            if (projectile.ai[1] >= 11)
            {
                projectile.Kill();
                return;
            }
            projectile.frame = (int)projectile.ai[1];

            AdjustPlayerItemFrameValues(player);
        }
        public void AdjustPlayerPositionValues(Player player)
        {
            projectile.Center = player.Center;
            projectile.position.X += 60 * player.direction;
            projectile.position.Y -= 30;
        }

        public void AdjustPlayerItemFrameValues(Player player)
        {
            if (projectile.ai[1] < 5)
            {
                player.bodyFrame.Y = player.bodyFrame.Height;
            }
            else if (projectile.ai[1] < 8)
            {
                player.bodyFrame.Y = 3 * player.bodyFrame.Height;
            }
            else
            {
                player.bodyFrame.Y = 4 * player.bodyFrame.Height;
            }

            projectile.spriteDirection = player.direction;
            player.heldProj = projectile.whoAmI;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CalamityPlayer calamityPlayer = Main.player[projectile.owner].Calamity();
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

            Vector2 drawCenter = projectile.Center;
            Rectangle frameRectangle = new Rectangle(projectile.frame / 5 * width, projectile.frame % 5 * height, width, height);

            Texture2D scytheTexture = Main.projectileTexture[projectile.type];
            Texture2D glowTexture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnFireSlash_Glow");

            Main.spriteBatch.Draw(scytheTexture,
                                  drawCenter - Main.screenPosition,
                                  frameRectangle,
                                  projectile.GetAlpha(lightColor),
                                  projectile.rotation,
                                  frameRectangle.Size() / 2,
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowTexture, 
                                  drawCenter - Main.screenPosition, 
                                  frameRectangle, 
                                  projectile.GetAlpha(Color.White),
                                  projectile.rotation,
                                  frameRectangle.Size() / 2, 
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
    }
}
