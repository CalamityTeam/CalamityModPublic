using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SilvaCrystal : ModProjectile
    {
    	public int dust = 3;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Silva Crystal");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.ignoreWater = true;
			projectile.minion = true;
			projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
			if (projectile.localAI[1] == 0f)
			{
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				projectile.localAI[1] += 1f;
			}
			if (Main.player[projectile.owner].minionDamage != projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue)
			{
				int damage2 = (int)(((float)projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue /
					projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue) *
					Main.player[projectile.owner].minionDamage);
				projectile.damage = damage2;
			}
			bool flag64 = projectile.type == mod.ProjectileType("SilvaCrystal");
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (!modPlayer.silvaSummon)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.sCrystal = false;
                }
                if (modPlayer.sCrystal)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
            projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2) + Main.player[projectile.owner].gfxOffY - 60f;
            if (Main.player[projectile.owner].gravDir == -1f)
            {
                projectile.position.Y = projectile.position.Y + 120f;
                projectile.rotation = 3.14f;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (float)((int)projectile.position.X);
            projectile.position.Y = (float)((int)projectile.position.Y);
            float num1072 = 1500f;
            projectile.velocity = Vector2.Zero;
            projectile.alpha -= 5;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.direction == 0)
            {
                projectile.direction = Main.player[projectile.owner].direction;
            }
            if (projectile.alpha == 0 && Main.rand.Next(15) == 0)
            {
                Dust dust34 = Main.dust[Dust.NewDust(projectile.Top, 0, 0, 267, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f)];
                dust34.velocity.X = 0f;
                dust34.noGravity = true;
                dust34.fadeIn = 1f;
                dust34.position = projectile.Center + Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (4f * Main.rand.NextFloat() + 26f);
                dust34.scale = 0.5f;
            }
            float[] var_2_31123_cp_0 = projectile.localAI;
            int var_2_31123_cp_1 = 0;
            float num73 = var_2_31123_cp_0[var_2_31123_cp_1];
            var_2_31123_cp_0[var_2_31123_cp_1] = num73 + 1f;
            if (projectile.localAI[0] >= 60f)
            {
                projectile.localAI[0] = 0f;
            }
            if (projectile.ai[0] < 0f)
            {
                float[] var_2_31169_cp_0 = projectile.ai;
                int var_2_31169_cp_1 = 0;
                float num730 = var_2_31169_cp_0[var_2_31169_cp_1];
                var_2_31169_cp_0[var_2_31169_cp_1] = num730 + 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                int num1074 = -1;
                float num1075 = num1072;
                NPC ownerMinionAttackTargetNPC6 = projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC6 != null && ownerMinionAttackTargetNPC6.CanBeChasedBy(projectile, false))
                {
                    float num1076 = projectile.Distance(ownerMinionAttackTargetNPC6.Center);
                    if (num1076 < num1075 && Collision.CanHitLine(projectile.Center, 0, 0, ownerMinionAttackTargetNPC6.Center, 0, 0))
                    {
                        num1075 = num1076;
                        num1074 = ownerMinionAttackTargetNPC6.whoAmI;
                    }
                }
                if (num1074 < 0)
                {
                    int num30;
                    for (int num1077 = 0; num1077 < 200; num1077 = num30 + 1)
                    {
                        NPC nPC16 = Main.npc[num1077];
                        if (nPC16.CanBeChasedBy(projectile, false))
                        {
                            float num1078 = projectile.Distance(nPC16.Center);
                            if (num1078 < num1075 && Collision.CanHitLine(projectile.Center, 0, 0, nPC16.Center, 0, 0))
                            {
                                num1075 = num1078;
                                num1074 = num1077;
                            }
                        }
                        num30 = num1077;
                    }
                }
                if (num1074 != -1)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = (float)num1074;
                    projectile.netUpdate = true;
                    return;
                }
            }
            if (projectile.ai[0] > 0f)
            {
                int num1079 = (int)projectile.ai[1];
                if (!Main.npc[num1079].CanBeChasedBy(projectile, false))
                {
                    projectile.ai[0] = 0f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                    return;
                }
                float[] var_2_31390_cp_0 = projectile.ai;
                int var_2_31390_cp_1 = 0;
                float num731 = var_2_31390_cp_0[var_2_31390_cp_1];
                var_2_31390_cp_0[var_2_31390_cp_1] = num731 + 1f;
                if (projectile.ai[0] >= 5f)
                {
                    Vector2 vector154 = projectile.DirectionTo(Main.npc[num1079].Center);
                    if (vector154.HasNaNs())
                    {
                        vector154 = Vector2.UnitY;
                    }
                    int num1082 = (vector154.X > 0f) ? 1 : -1;
                    projectile.direction = num1082;
                    projectile.ai[0] = -20f;
                    projectile.netUpdate = true;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Vector2 vector155 = Main.npc[num1079].position + Main.npc[num1079].Size * Utils.RandomVector2(Main.rand, 0f, 1f) - projectile.Center;
                        int num31;
                        for (int num1083 = 0; num1083 < 3; num1083 = num31 + 1)
                        {
                            Vector2 vector156 = projectile.Center + vector155;
                            if (num1083 > 0)
                            {
                                vector156 = projectile.Center + vector155.RotatedByRandom(0.78539818525314331) * (Main.rand.NextFloat() * 0.5f + 0.75f);
                            }
                            float x4 = Main.rgbToHsl(new Color(Main.DiscoR, 203, 103)).X;
                            Projectile.NewProjectile(vector156.X, vector156.Y, 0f, 0f, mod.ProjectileType("SilvaCrystalExplosion"), projectile.damage, projectile.knockBack, projectile.owner, x4, (float)projectile.whoAmI);
                            num31 = num1083;
                        }
                        return;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 127 - projectile.alpha / 2);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            Vector2 vector59 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = Main.projectileTexture[projectile.type];
            Microsoft.Xna.Framework.Rectangle rectangle17 = texture2D34.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Microsoft.Xna.Framework.Color alpha5 = projectile.GetAlpha(color25);
            Vector2 origin11 = rectangle17.Size() / 2f;
            float scaleFactor5 = (float)Math.Cos((double)(6.28318548f * (projectile.localAI[0] / 60f))) + 3f + 3f;
            for (float num286 = 0f; num286 < 4f; num286 += 1f)
            {
                SpriteBatch arg_F907_0 = Main.spriteBatch;
                Texture2D arg_F907_1 = texture2D34;
                Vector2 arg_F8CE_0 = vector59;
                Vector2 arg_F8BE_0 = Vector2.UnitY;
                double arg_F8BE_1 = (double)(num286 * 1.57079637f);
                Vector2 center = default(Vector2);
                arg_F907_0.Draw(arg_F907_1, arg_F8CE_0 + arg_F8BE_0.RotatedBy(arg_F8BE_1, center) * scaleFactor5, new Microsoft.Xna.Framework.Rectangle?(rectangle17), alpha5 * 0.2f, projectile.rotation, origin11, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

		public override bool CanDamage()
		{
			return false;
		}
	}
}