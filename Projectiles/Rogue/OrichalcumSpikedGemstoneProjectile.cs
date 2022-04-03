using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            Projectile.Calamity().rogue = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 velocity = Projectile.velocity;
            if (Projectile.velocity.Y != velocity.Y && (velocity.Y < -3f || velocity.Y > 3f))
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            if (Projectile.velocity.X != velocity.X)
            {
                Projectile.velocity.X = velocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != velocity.Y && velocity.Y > 1f)
            {
                Projectile.velocity.Y = velocity.Y * -0.5f;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
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
            if (Main.myPlayer != Projectile.owner || !Projectile.Calamity().stealthStrike)
                return;

            for (int i = 0; i < 2; i++)
            {
                int direction = Main.player[Projectile.owner].direction;
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
                int petal = Projectile.NewProjectile(startPos, pathToTravel, ProjectileID.FlowerPetal, Projectile.damage, 0f, Projectile.owner);
                if (petal.WithinBounds(Main.maxProjectiles))
                    Main.projectile[petal].Calamity().forceRogue = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<OrichalcumSpikedGemstone>());
            }
        }
    }
}
