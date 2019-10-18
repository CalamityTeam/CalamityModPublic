using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Rogue
{
    public class ConsecratedWaterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Consecrated Water");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
            projectile.tileCollide = true;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            projectile.ai[0] += 1f; //arbitrary timer
            if (projectile.ai[0] > 75f)
            {
                //considering making small methods in a projectile utils file. Things like this are everywhere lol
                if (projectile.velocity.Y < 10f)
                {
                    projectile.velocity.Y += 0.15f;
                }
            }
            projectile.rotation += MathHelper.ToRadians(projectile.velocity.Length());
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.Bottom);
            //normal
            if (projectile.ai[1] == 0f)
            {
                Point result;
                if (WorldUtils.Find(projectile.Top.ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
                {
                    Projectile.NewProjectile(result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<BlueFlamePillar>(), ConsecratedWater.BaseDamage, 2f, projectile.owner);
                }
            }
            //stealth strike
            else if (projectile.ai[1] == 1f)
            {
                //3 pillars instead of 1
                for (float i = -1f; i <= 1f; i += 1f)
                {
                    Point result;
                    if (WorldUtils.Find((projectile.Top + i * Main.rand.NextFloat(56f, 108f) * Vector2.UnitX).ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
                    {
                        Projectile.NewProjectile(result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<BlueFlamePillar>(), ConsecratedWater.BaseDamage, 2f, projectile.owner);
                    }
                }
            }
        }
    }
}
