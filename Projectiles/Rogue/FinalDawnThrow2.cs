using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnThrow2 : ModProjectile
    {
        bool HasHitEnemy = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.light = 0.0f;
            projectile.extraUpdates = 1;
            projectile.tileCollide = true; // We don't want people getting stuck in walls right
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 13;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //should be self explanatory
            width = 32;
            height = 32;
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Spawn homing flames that chase the HIT enemy only. This is also limited to one burst
            if (Main.myPlayer == projectile.owner && !HasHitEnemy)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = Utils.NextVector2Circular(Main.rand, 7.2f, 7.2f);
                    Projectile.NewProjectile(projectile.Center, velocity,
                                             ModContent.ProjectileType<FinalDawnFireball>(),
                                             (int)(projectile.damage * 0.3), projectile.knockBack, projectile.owner, 0f,
                                             target.whoAmI);
                }
                HasHitEnemy = true;
            }
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player is null || player.dead)
                projectile.Kill();

            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item71, projectile.position);
                projectile.localAI[0] = 1;
            }

            // Kill any hooks from the projectile owner.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];

                if (!proj.active || proj.owner != player.whoAmI || proj.aiStyle != 7)
                    continue;

                if (proj.aiStyle == 7)
                    proj.Kill();
            }

            projectile.spriteDirection = projectile.velocity.X > 0 ? 1 : -1;
            projectile.rotation += 0.25f * projectile.direction;
            player.Center = projectile.Center;
            player.fullRotationOrigin = player.Center - player.position;
            player.fullRotation = projectile.rotation;
            player.direction = projectile.direction;
            player.heldProj = projectile.whoAmI;
            player.bodyFrame.Y = player.bodyFrame.Height;
            player.immuneNoBlink = true;
            player.immuneTime = 4;
            for (int k = 0; k < player.hurtCooldowns.Length; k++)
                player.hurtCooldowns[k] = player.immuneTime;

            // This is to make sure the player doesn't get yeeted out of the world, which crashes the game pretty much all of the time
            bool worldEdge = projectile.Center.X < 1000 || projectile.Center.Y < 1000 || projectile.Center.X > Main.maxTilesX * 16 - 1000 || projectile.Center.Y > Main.maxTilesY * 16 - 1000;

            projectile.ai[0]++;
            if(projectile.ai[0] >= 60 || worldEdge)
            {
                projectile.Kill();
            }

            int idx = Dust.NewDust(projectile.position, projectile.width , projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 2.5f);
            Main.dust[idx].velocity = projectile.velocity * -0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            float scytheRotation = player.fullRotation;

            Texture2D scytheTexture = Main.projectileTexture[projectile.type];
            Texture2D glowScytheTexture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnThrow2_Glow");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;

            Vector2 origin = new Vector2(scytheTexture.Width / 2f + 40f * player.direction, num214 * 1.1f);

            Main.spriteBatch.Draw(scytheTexture,
                                  player.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, y6, scytheTexture.Width, num214)),
                                  projectile.GetAlpha(lightColor),
                                  scytheRotation,
                                  origin,
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowScytheTexture,
                                  player.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, y6, scytheTexture.Width, num214)),
                                  projectile.GetAlpha(Color.White),
                                  scytheRotation,
                                  origin,
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            player.fullRotation = 0;
        }
    }
}