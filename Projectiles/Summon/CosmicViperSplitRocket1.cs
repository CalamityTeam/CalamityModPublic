using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSplitRocket1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket Shard");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            float colorScale = (float)projectile.alpha / 255f;
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/CosmicViperSplitRocket2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
        }
    }
}
