using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class RotomPet : ModProjectile
    {
        private int biome = 0;
		private float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotom");
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			if (CalamityPlayer.areThereAnyDamnBosses)
				biome = 1; //dex
			else if (player.ZoneBeach || player.Calamity().ZoneSunkenSea || player.Calamity().ZoneSulphur || player.Calamity().ZoneAbyss)
				biome = 2; //wash
			else if (player.ZoneTowerSolar || player.ZoneDesert || player.ZoneUndergroundDesert || player.ZoneUnderworldHeight || player.Calamity().ZoneCalamity)
				biome = 3; //heat
			else if (player.ZoneSnow || Main.snowMoon)
				biome = 4; //frost
			else if (player.ZoneJungle)
				biome = 5; //mow
			else if (player.ZoneSkyHeight || player.ZoneMeteor || player.Calamity().ZoneAstral)
				biome = 6; //fan
			else
				biome = 0; //anything else

            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.rotomPet = false;
            }
            if (modPlayer.rotomPet)
            {
                projectile.timeLeft = 2;
            }

            if (dust == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 25;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 132, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }

            float SAImovement = 0.05f;
            for (int index = 0; index < Main.projectile.Length; index++)
            {
				Projectile proj = Main.projectile[index];
                bool flag23 = Main.projPet[proj.type];
                if (index != projectile.whoAmI && proj.active && proj.owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }

            float num16 = 0.5f;
            projectile.tileCollide = false;
            int num17 = 100;
            Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num18 = player.position.X + (float)(player.width / 2) - vector3.X;
            float num19 = player.position.Y + (float)(player.height / 2) - vector3.Y;
            num19 += (float)Main.rand.Next(-10, 21);
            num18 += (float)Main.rand.Next(-10, 21);
            num18 += (float)(60 * -(float)Main.player[projectile.owner].direction);
            num19 -= 60f;
            float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            float num21 = 18f;
            if (num20 < (float)num17 && Main.player[projectile.owner].velocity.Y == 0f &&
                projectile.position.Y + (float)projectile.height <= Main.player[projectile.owner].position.Y + (float)Main.player[projectile.owner].height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (num20 > 2000f)
            {
                projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                projectile.netUpdate = true;
            }
            if (num20 < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (num20 < 100f)
                {
                    num16 = 0.1f;
                }
                if (num20 > 300f)
                {
                    num16 = 1f;
                }
                num20 = num21 / num20;
                num18 *= num20;
                num19 *= num20;
            }
            if (projectile.velocity.X < num18)
            {
                projectile.velocity.X = projectile.velocity.X + num16;
                if (num16 > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + num16;
                }
            }
            if (projectile.velocity.X > num18)
            {
                projectile.velocity.X = projectile.velocity.X - num16;
                if (num16 > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - num16;
                }
            }
            if (projectile.velocity.Y < num19)
            {
                projectile.velocity.Y = projectile.velocity.Y + num16;
                if (num16 > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
                }
            }
            if (projectile.velocity.Y > num19)
            {
                projectile.velocity.Y = projectile.velocity.Y - num16;
                if (num16 > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
                }
            }

			projectile.rotation = projectile.velocity.X * 0.05f;
			if ((double) projectile.velocity.X > 0.25)
				projectile.spriteDirection = projectile.direction = 1;
			else if ((double) projectile.velocity.X < -0.25)
				projectile.spriteDirection = projectile.direction = -1;

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
			if (biome == 1)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomDex");
			if (biome == 2)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomWash");
			if (biome == 3)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomHeat");
			if (biome == 4)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomFrost");
			if (biome == 5)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomMow");
			if (biome == 6)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomFan");
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomPetGlow");
			if (biome == 1)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomDexGlow");
			if (biome == 2)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomWashGlow");
			if (biome == 3)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomHeatGlow");
			if (biome == 4)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomFrostGlow");
			if (biome == 5)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomMowGlow");
			if (biome == 6)
				texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/RotomFanGlow");
			int num214 = texture.Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Color.White, projectile.rotation, new Vector2((float)texture.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
		}

        public override bool CanDamage()
        {
            return false;
        }
    }
}
