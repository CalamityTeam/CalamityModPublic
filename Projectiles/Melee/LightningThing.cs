using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Melee;
namespace CalamityMod.Projectiles.Melee
{
	public class LightningThing : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.timeLeft = 90;
            projectile.melee = true;
        }
        public override void Kill(int timeLeft)
        {
            int damage = GaelsGreatsword.BaseDamage;
            for (int i = 0; i < 3; i++)
            {
                int idx = Projectile.NewProjectile(projectile.Center + new Vector2(Main.rand.NextFloat(-35f, 35f), -1600f), Vector2.UnitY * 12f,
                    ProjectileID.CultistBossLightningOrbArc, GaelsGreatsword.BaseDamage, 0f, projectile.owner,
                    MathHelper.PiOver2, Main.rand.Next(100));
                Main.projectile[idx].Calamity().forceMelee = true;
                Main.projectile[idx].usesLocalNPCImmunity = true;
                Main.projectile[idx].localNPCHitCooldown = GaelsGreatsword.ImmunityFrames;
            }
        }
    }
}
