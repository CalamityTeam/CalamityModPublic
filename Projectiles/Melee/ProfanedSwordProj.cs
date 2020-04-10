using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ProfanedSwordProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Sword");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft > 595)
				return false;

			Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (CalamityGlobalNPC.AquaticScourgeIDs.Contains(target.type) || CalamityGlobalNPC.DestroyerIDs.Contains(target.type))
				projectile.penetrate--;

            target.immune[projectile.owner] = 6;
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
			if (Main.myPlayer == projectile.owner)
			{
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<BrimlanceHellfireExplosion>(), (int)(projectile.damage * 0.5), knockback, projectile.owner);
			}
        }
    }
}
