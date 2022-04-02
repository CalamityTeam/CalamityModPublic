using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class PurityProjectionSigil : ModProjectile
    {
        private NPC target => Main.npc[(int)projectile.ai[0]];

        public Player Owner => Main.player[projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_PurityProjectionSigil";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purity Sigil");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = TrueBiomeBlade.DefaultAttunement_SigilTime;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AI()
        {

            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;

            if (target.active)
            {
                projectile.Center = target.Center;
            }
            else
                projectile.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.myPlayer != projectile.owner) // don't show for other players
                return false;
            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}