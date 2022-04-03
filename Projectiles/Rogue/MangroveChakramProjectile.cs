using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MangroveChakramProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MangroveChakram";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mangrove Chakram");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.WoodenBoomerang;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0f);
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 44, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.Calamity().stealthStrike)
            {
                // Die early.
                if (Projectile.timeLeft < 240)
                    Projectile.Kill();

                Projectile.localAI[0] += Main.rand.Next(0,3);
                if (Projectile.localAI[0] >= 10f)
                {
                    Projectile.localAI[0] = 0f;
                    Vector2 flowerSpawnPosition = Projectile.Center + Main.rand.NextVector2Square(-10f, 10f);
                    Vector2 flowerShootVelocity = Projectile.velocity.RotatedByRandom(0.1f) * 0.25f;
                    Projectile.NewProjectile(flowerSpawnPosition, flowerShootVelocity, ModContent.ProjectileType<MangroveChakramFlower>(), Projectile.damage / 4, 0f, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(BuffID.CursedInferno, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}
