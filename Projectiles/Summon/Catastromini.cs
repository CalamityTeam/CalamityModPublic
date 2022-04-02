using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Catastromini : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastromini");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int brim = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Brimstone, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[brim].noGravity = true;
                    Main.dust[brim].velocity = dustVel;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            bool correctMinion = projectile.type == ModContent.ProjectileType<Catastromini>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cEyes = false;
                }
                if (modPlayer.cEyes)
                {
                    projectile.timeLeft = 2;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
            projectile.frameCounter++;
            if (projectile.frameCounter > (projectile.ai[0] == 2f ? 1 : 3))
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }

            projectile.ChargingMinionAI(Calamitamini.Range, Calamitamini.SeparationAnxietyMin, Calamitamini.SeparationAnxietyMax, Calamitamini.SafeDist, 0, 40f, 8f, 4f, new Vector2(0f, -60f), 40f, 8f, false, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int yStart = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, yStart, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
