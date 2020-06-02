using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                int num501 = 25;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 132, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust += 1f;
            }

			CalamityGlobalProjectile.FloatingPetAI(projectile, true, 0.05f);

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
