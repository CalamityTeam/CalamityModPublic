using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class MK2FlaskSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Items/Weapons/Summon/FuelCellBundle";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += 0.1f * (Projectile.velocity.X > 0).ToDirectionInt();
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(18, 21); i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                        (int)CalamityDusts.Plague,
                        Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
                }
                SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * 6f, ModContent.ProjectileType<PlaguebringerMK2>(), Projectile.damage, 4f, Projectile.owner);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Projectile.originalDamage;

                int beeArrayIndex = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<PlaguebringerMK2>())
                    {
                        Main.projectile[i].ai[1] = beeArrayIndex;
                        beeArrayIndex++;
                    }
                }
            }
        }
        public override bool? CanDamage() => false;
    }
}
