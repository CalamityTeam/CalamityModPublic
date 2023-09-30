using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Melee;
namespace CalamityMod.Projectiles.Melee
{
    public class LightningThing : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnKill(int timeLeft)
        {
            int damage = (int)Main.player[Projectile.owner].GetTotalDamage<MeleeDamageClass>().ApplyTo(GaelsGreatsword.BaseDamage);
            for (int i = 0; i < 3; i++)
            {
                int idx = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.NextFloat(-35f, 35f), -1600f), Vector2.UnitY * 12f,
                    ProjectileID.CultistBossLightningOrbArc, damage, 0f, Projectile.owner,
                    MathHelper.PiOver2, Main.rand.Next(100));
                if (idx.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[idx].usesLocalNPCImmunity = true;
                    Main.projectile[idx].localNPCHitCooldown = GaelsGreatsword.ImmunityFrames;
                    // TODO -- Ozzatron 2JUL2022: what is this? Removing forceMelee, hope it doesn't break...
                    // In death mode (and under a few other select conditions), the lightning has a PreAI return false in global projectile
                    // So forceMelee won't work unless the projectile is friendly (because a friendly exception was added)
                    Main.projectile[idx].friendly = true;
                    Main.projectile[idx].hostile = false;
                    Main.projectile[idx].DamageType = DamageClass.Melee;
                }
            }
        }
    }
}
