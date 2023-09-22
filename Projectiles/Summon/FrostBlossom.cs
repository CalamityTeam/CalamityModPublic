using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrostBlossom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Owner.Center.MinionHoming(500f, Owner, CalamityPlayer.areThereAnyDamnBosses);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<FrostBlossom>();
            Owner.AddBuff(ModContent.BuffType<FrostBlossomBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (Owner.dead)
                {
                    ModdedOwner.frostBlossom = false;
                }
                if (ModdedOwner.frostBlossom)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.Center = Owner.Center + Vector2.UnitY * (Owner.gfxOffY - 60f);
            if (Owner.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            Projectile.scale = 1f + (float)Math.Sin(Projectile.ai[0]++ / 40f) * 0.085f;
            Projectile.Opacity = 1f - (float)Math.Sin(Projectile.ai[1] / 45f) * 0.075f - 0.075f; // Range of 1f to 0.85f
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 113);
                    dust.noGravity = true;
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 7f);
                }
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Target != null)
                {
                    if (Projectile.ai[1]++ % 35f == 34f && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Target.position, Target.width, Target.height))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(Target.Center) * 20f, ModContent.ProjectileType<FrostBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
