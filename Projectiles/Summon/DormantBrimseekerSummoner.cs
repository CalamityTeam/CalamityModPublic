using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class DormantBrimseekerSummoner : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Items/Weapons/Summon/DormantBrimseeker";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.045f);
            Projectile.velocity *= 0.975f;
            if (Projectile.ai[0]++ == 110f)
            {
                SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
            }
            if (Projectile.ai[0]++ >= 90f)
            {
                for (int i = 0; i < (180 - (int)Projectile.ai[0]) / 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.Brimstone);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 4f);
                    dust.noGravity = true;
                }
                Projectile.alpha = (int)(255 * (Projectile.ai[0] - 90f) / 90f);
            }
            if (Projectile.ai[0] >= 180f)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * 7f, ModContent.ProjectileType<DormantBrimseekerBab>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Projectile.originalDamage;
        }
    }
}
