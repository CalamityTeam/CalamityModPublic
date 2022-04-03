using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class RotomPet : ModProjectile
    {
        private bool initialized = false;
        private int form = 0;
        private const int Normal = 0;
        private const int Dex = 1;
        private const int Wash = 2;
        private const int Heat = 3;
        private const int Frost = 4;
        private const int Mow = 5;
        private const int Fan = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotom");
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.rotomPet = false;
            }
            if (modPlayer.rotomPet)
            {
                Projectile.timeLeft = 2;
            }

            if (!initialized)
            {
                DustEffects();
                initialized = true;
            }

            UpdateForm(player);
            UpdateFrames();

            Projectile.FloatingPetAI(true, 0.05f);
        }

        private void UpdateForm(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                form = Dex;
            else if (player.ZoneBeach || player.InSunkenSea() || player.InSulphur() || player.InAbyss())
                form = Wash;
            else if (player.ZoneTowerSolar || player.ZoneDesert || player.ZoneUndergroundDesert || player.ZoneUnderworldHeight || player.InCalamity())
                form = Heat;
            else if (player.ZoneSnow || Main.snowMoon)
                form = Frost;
            else if (player.ZoneJungle)
                form = Mow;
            else if (player.ZoneSkyHeight || player.ZoneMeteor || player.InAstral())
                form = Fan;
            else
                form = Normal;
        }

        private void DustEffects()
        {
            int dustAmt = 25;
            for (int i = 0; i < dustAmt; i++)
            {
                int electric = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 132, 0f, 0f, 0, default, 1f);
                Main.dust[electric].velocity *= 2f;
                Main.dust[electric].scale *= 1.15f;
            }
        }

        private void UpdateFrames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Drawing(spriteBatch, lightColor,
                Main.projectileTexture[Projectile.type],
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomDex"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomWash"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomHeat"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomFrost"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomMow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomFan"));
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Drawing(spriteBatch, Color.White,
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomPetGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomDexGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomWashGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomHeatGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomFrostGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomMowGlow"),
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/RotomFanGlow"));
        }

        private void Drawing(SpriteBatch spriteBatch, Color color, Texture2D normal, Texture2D dex, Texture2D wash, Texture2D heat, Texture2D frost, Texture2D mow, Texture2D fan)
        {
            Texture2D texture = normal;
            switch (form)
            {
                case Dex:
                    texture = dex;
                    break;
                case Wash:
                    texture = wash;
                    break;
                case Heat:
                    texture = heat;
                    break;
                case Frost:
                    texture = frost;
                    break;
                case Mow:
                    texture = mow;
                    break;
                case Fan:
                    texture = fan;
                    break;
                default:
                    break;
            }

            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), color, Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, spriteEffects, 0f);
        }

        public override bool CanDamage() => false;
    }
}
