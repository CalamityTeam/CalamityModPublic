using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class EternityBook : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            projectile.Center = player.Center + 19f * player.direction * Vector2.UnitX;

            if (player.channel && !player.noItems && !player.CCed)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] == 1f)
                {
                    NPC target = projectile.Center.ClosestNPCAt(4400f, true, true);

                    if (target != null)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"));
                        int hex = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityHex>(), projectile.damage, 0f, player.whoAmI, target.whoAmI);
                        Main.projectile[hex].localAI[1] = projectile.whoAmI;
                        for (int i = 0; i < 5f; i++)
                        {
                            float theta = MathHelper.TwoPi / 5f * i;
                            int idx = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCrystal>(), 0, 0f, player.whoAmI, target.whoAmI, theta);
                            Main.projectile[idx].frame = i % 2;
                            Main.projectile[idx].localAI[1] = projectile.whoAmI;
                        }
                        for (int i = 0; i < 10f; i++)
                        {
                            float theta = MathHelper.TwoPi / 10f * i;
                            int idx = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCircle>(), 0, 0f, player.whoAmI, target.whoAmI, theta);
                            Main.projectile[idx].localAI[1] = projectile.whoAmI;
                        }
                    }
                }
                projectile.frameCounter++;
                if (projectile.frameCounter > (int)MathHelper.Lerp(10f, 1f, MathHelper.Clamp(projectile.ai[0] / 200f, 0f, 1f)))
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
            else projectile.Kill();

            projectile.spriteDirection = projectile.direction = player.direction;
            projectile.timeLeft = 2;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (projectile.direction * projectile.velocity).ToRotation();
        }
        public override bool CanDamage() => false;
    }
}
