using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class StealthNimbus : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Boss/ShadeNimbusHostile";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[0] == 1f)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/StealthNimbus2").Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 3600f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.timeLeft % 8 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int rainSpawnX = (int)(Projectile.position.X + 14f + (float)Main.rand.Next(Projectile.width - 28));
                        int rainSpawnY = (int)(Projectile.position.Y + (float)Projectile.height + 4f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)rainSpawnX, (float)rainSpawnY, 0f, 5f, ModContent.ProjectileType<StealthRain>(), Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], 0f);
                    }
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 10f)
            {
                Projectile.localAI[0] = 0f;
                int projCount = 0;
                int oldestCloud = 0;
                float cloudAge = 0f;
                int projType = Projectile.type;
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == projType && proj.ai[1] < 3600f)
                    {
                        projCount++;
                        if (proj.ai[1] > cloudAge)
                        {
                            oldestCloud = projIndex;
                            cloudAge = proj.ai[1];
                        }
                    }
                }
                if (projCount > 2)
                {
                    Main.projectile[oldestCloud].netUpdate = true;
                    Main.projectile[oldestCloud].ai[1] = 36000f;
                    return;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
