using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class DankCreeper : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dank Creeper");
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 74;
            projectile.scale = 0.75f;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.aiStyle = 54;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            aiType = 317;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
        	if (projectile.localAI[0] == 0f)
        	{
        		int num226 = 36;
				for (int num227 = 0; num227 < num226; num227++)
				{
					Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
					vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + projectile.Center;
					Vector2 vector7 = vector6 - projectile.Center;
					int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 14, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.4f);
					Main.dust[num228].noGravity = true;
					Main.dust[num228].noLight = true;
					Main.dust[num228].velocity = vector7;
				}
				projectile.localAI[0] += 1f;
        	}
        	bool flag64 = projectile.type == mod.ProjectileType("DankCreeper");
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			player.AddBuff(mod.BuffType("DankCreeper"), 3600);
			if (flag64)
			{
				if (player.dead)
				{
					modPlayer.dCreeper = false;
				}
				if (modPlayer.dCreeper)
				{
					projectile.timeLeft = 2;
				}
			}
		}
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}