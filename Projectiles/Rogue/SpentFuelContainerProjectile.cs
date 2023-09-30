using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpentFuelContainerProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SpentFuelContainer";

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = true;
            Projectile.alpha = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1f; //arbitrary timer
            if (Projectile.ai[0] > 75f)
            {
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
            for (int i = 0; i < 30; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                Dust.NewDust(Projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid, dspeed.X, dspeed.Y, 0, default, 1.1f);
            }

            SoundEngine.PlaySound(SoundID.Item107, Projectile.Bottom);
            Point result;
            if (WorldUtils.Find(Projectile.Top.ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<SulphuricNukesplosion>(), Projectile.damage, 2f, Projectile.owner);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
            }
        }
    }
}
