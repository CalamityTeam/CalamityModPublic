using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class OrichalcumSpikedGemstoneProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/OrichalcumSpikedGemstone";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gemstone");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.aiStyle = 2;
            projectile.penetrate = 6;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 velocity = projectile.velocity;
            if (projectile.velocity.Y != velocity.Y && (velocity.Y < -3f || velocity.Y > 3f))
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, projectile.Center);
            }
            if (projectile.velocity.X != velocity.X)
            {
                projectile.velocity.X = velocity.X * -0.5f;
            }
            if (projectile.velocity.Y != velocity.Y && velocity.Y > 1f)
            {
                projectile.velocity.Y = velocity.Y * -0.5f;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffect(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffect(target.Center);
        }

        private void OnHitEffect(Vector2 targetPos)
        {
            if (Main.myPlayer != projectile.owner || !projectile.Calamity().stealthStrike)
                return;

            for (int i = 0; i < 2; i++)
            {
                int direction = Main.player[projectile.owner].direction;
                float xStart = Main.screenPosition.X;
                if (direction < 0)
                    xStart += Main.screenWidth;
                float yStart = Main.screenPosition.Y + Main.rand.Next(Main.screenHeight);
                Vector2 startPos = new Vector2(xStart, yStart);
                Vector2 pathToTravel = targetPos - startPos;
                pathToTravel.X += Main.rand.NextFloat(-50f, 50f) * 0.1f;
                pathToTravel.Y += Main.rand.NextFloat(-50f, 50f) * 0.1f;
                float speedMult = 24f / pathToTravel.Length();
                pathToTravel.X *= speedMult;
                pathToTravel.Y *= speedMult;
                int petal = Projectile.NewProjectile(startPos, pathToTravel, ProjectileID.FlowerPetal, projectile.damage, 0f, projectile.owner);
                if (petal.WithinBounds(Main.maxProjectiles))
                    Main.projectile[petal].Calamity().forceRogue = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<OrichalcumSpikedGemstone>());
            }
        }
    }
}
