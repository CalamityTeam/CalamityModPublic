using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ConsecratedWaterProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ConsecratedWater";

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1f; //arbitrary timer
            if (Projectile.ai[0] > 75f)
            {
                //considering making small methods in a projectile utils file. Things like this are everywhere lol
                if (Projectile.velocity.Y < 10f)
                {
                    Projectile.velocity.Y += 0.15f;
                }
            }
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length());
        }
        public override void OnKill(int timeLeft)
        {
            //Dust
            for (int i = 0; i< 30;i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.BlueCrystalShard, dspeed.X, dspeed.Y, 0, default, 1.1f);
            }

            SoundEngine.PlaySound(SoundID.Item107, Projectile.Bottom);
            //normal
            if (Projectile.ai[1] == 0f)
            {
                Point result;
                if (WorldUtils.Find(Projectile.Top.ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<BlueFlamePillar>(), Projectile.damage, 2f, Projectile.owner);
                }
            }
            //stealth strike
            else if (Projectile.ai[1] == 1f)
            {
                //3 pillars instead of 1
                for (float i = -1f; i <= 1f; i += 1f)
                {
                    Point result;
                    if (WorldUtils.Find((Projectile.Top + i * Main.rand.NextFloat(56f, 108f) * Vector2.UnitX).ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<BlueFlamePillar>(), Projectile.damage, 2f, Projectile.owner);
                    }
                }
            }
        }
    }
}
