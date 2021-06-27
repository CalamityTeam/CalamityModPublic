using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CatastropheSummon : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public bool LookingAtPlayer => Time < 45f;
        public override string Texture => "CalamityMod/NPCs/SupremeCalamitas/SupremeCatastrophe";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = false;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = false;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 120;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.Opacity = 0f;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI() => CataclysmSummon.Behavior(projectile, Main.player[projectile.owner], ref Time);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], Color.Red, 1);
            return true;
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 70;

        public override bool CanDamage() => projectile.Opacity >= 1f;
    }
}
