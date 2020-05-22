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
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<AdamantiteThrowingAxe>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.Calamity().stealthStrike && Main.myPlayer == projectile.owner)
			{
				for (int n = 0; n < 3; n++)
				{
					float xStart = projectile.position.X + Main.rand.Next(-400, 400);
					float yStart = projectile.position.Y + Main.rand.Next(500, 800);
					Vector2 startPos = new Vector2(xStart, yStart);
					Vector2 velocity = projectile.Center - startPos;
					velocity.X += (float)Main.rand.Next(-100, 101);
					float travelDist = velocity.Length();
					float lightningSpeed = 8f;
					travelDist = lightningSpeed / travelDist;
					velocity.X *= travelDist;
					velocity.Y *= travelDist;
					Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<BlunderBoosterLightning>(), projectile.damage, projectile.knockBack, projectile.owner, Main.rand.Next(2), 0f);
				}
			}
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (projectile.Calamity().stealthStrike && Main.myPlayer == projectile.owner)
			{
				for (int n = 0; n < Main.rand.Next(3,6); n++)
				{
					float xStart = projectile.position.X + Main.rand.Next(-400, 400);
					float yStart = projectile.position.Y + Main.rand.Next(500, 800);
					Vector2 startPos = new Vector2(xStart, yStart);
					Vector2 velocity = projectile.Center - startPos;
					velocity.X += (float)Main.rand.Next(-100, 101);
					float travelDist = velocity.Length();
					float lightningSpeed = 16f;
					travelDist = lightningSpeed / travelDist;
					velocity.X *= travelDist;
					velocity.Y *= travelDist;
					Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<BlunderBoosterLightning>(), projectile.damage, projectile.knockBack, projectile.owner, Main.rand.Next(2), 1f);
				}
			}
		}
    }
}
