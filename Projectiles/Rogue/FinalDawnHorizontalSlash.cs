using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnHorizontalSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            projectile.width = 600;
            projectile.height = 156;
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

            projectile.Center = player.Center;
            player.heldProj = projectile.whoAmI;
            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();

            projectile.ai[0]++;
            if (projectile.ai[0] >= 4)
            {
                projectile.ai[1]++;
                projectile.ai[0] = 0;
                if (projectile.ai[1] == 3)
                {
                    projectile.friendly = true;
                    Main.PlaySound(SoundID.Item71, projectile.position);

                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FinalDawnFlame>(), projectile.damage / 2, 0f, projectile.owner);
                    }
                }
            }
            if (projectile.ai[1] >= 9)
            {
                projectile.Kill();
                return;
            }

            projectile.frame = (int)projectile.ai[1];

            if (projectile.ai[1] < 2) player.bodyFrame.Y = 1 * player.bodyFrame.Height;
            else player.bodyFrame.Y = 3 * player.bodyFrame.Height;

            if (projectile.ai[1] == 4 || projectile.ai[1] == 5) player.direction = -1 * projectile.spriteDirection;
            else player.direction = 1 * projectile.spriteDirection;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D scytheTexture = Main.projectileTexture[projectile.type];
            Texture2D scytheGlowTexture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnHorizontalSlash_Glow");
            int height = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int yStart = height * projectile.frame;
            Main.spriteBatch.Draw(scytheTexture,
                                  projectile.Center - Main.screenPosition + projectile.gfxOffY * Vector2.UnitY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(lightColor), projectile.rotation,
                                  new Vector2((float)scytheTexture.Width / 2f, (float)height / 2f), projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(scytheGlowTexture,
                                  projectile.Center - Main.screenPosition + projectile.gfxOffY * Vector2.UnitY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(Color.White),
                                  projectile.rotation,
                                  new Vector2((float)scytheTexture.Width / 2f, (float)height / 2f),
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}
