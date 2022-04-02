using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AdamantiteThrowingAxeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AdamantiteThrowingAxe";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Throwing Axe");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.Shuriken;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.Center.X, (int)projectile.Center.Y, projectile.width, projectile.height, ModContent.ItemType<AdamantiteThrowingAxe>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (projectile.Calamity().stealthStrike && Main.myPlayer == projectile.owner)
            {
                for (int n = 0; n < 3; n++)
                {
                    Projectile lightning = CalamityUtils.ProjectileRain(projectile.Center, 400f, 100f, -800f, -500f, 8f, ModContent.ProjectileType<BlunderBoosterLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
                    lightning.ai[0] = Main.rand.Next(2);
                }
            }
        }
    }
}
