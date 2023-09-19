using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WitherBlossom : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<WitherBlossom>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<WitherBlossomsBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.witherBlossom;
        public override float MinionSlots => 0.5f;

        public ref float OffsetAngle => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
        }

        public override void MinionAI()
        {
            Projectile.Center = Owner.Center + OffsetAngle.ToRotationVector2() * 150f;

            Time++;
            if (Time % 50 == 49 && Main.myPlayer == Projectile.owner && Target != null)
            {
                Vector2 shootVelocity = Projectile.SafeDirectionTo(Target.Center).RotatedByRandom(0.25f) * 8f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<WitherBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if (!Main.dedServ)
            {
                Projectile.rotation += MathHelper.ToRadians(5f);
                OffsetAngle += MathHelper.ToRadians(4f);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 179);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 7f);
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
