using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MK2FlaskSummon : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Summon/FuelCellBundle";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flask");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 26;
            projectile.height = 26;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.2f;
            projectile.rotation += 0.1f * (projectile.velocity.X > 0).ToDirectionInt();
        }
        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(18, 21); i++)
                {
                    Dust.NewDustPerfect(projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                        (int)CalamityDusts.Plague,
                        Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
                }
                Main.PlaySound(SoundID.Item107, projectile.Center);
                int idx = Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 6f, ModContent.ProjectileType<PlaguebringerMK2>(), projectile.damage, 4f, projectile.owner);
                int beeArrayIndex = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<PlaguebringerMK2>())
                    {
                        Main.projectile[i].ai[1] = beeArrayIndex;
                        beeArrayIndex++;
                    }
                }
            }
        }
        public override bool CanDamage() => false;
    }
}
