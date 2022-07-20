using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ExobeamSlashCreator : ModProjectile
    {
        public NPC Target => Main.npc[(int)Projectile.ai[0]];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anime Sword Effect");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner && Projectile.timeLeft % 20 == 19)
            {
                float maxOffset = Target.width * 0.4f;
                if (maxOffset > 300f)
                    maxOffset = 300f;
                Vector2 baseDirection = Vector2.UnitX;
                if (Projectile.timeLeft <= 20)
                    baseDirection = -Vector2.UnitY;

                Vector2 spawnOffset = baseDirection.RotatedBy(Main.rand.NextFloatDirection() * 0.63f).SafeNormalize(Vector2.UnitY);
                spawnOffset *= Main.rand.NextFloatDirection() * maxOffset;
                Vector2 sliceVelocity = spawnOffset.SafeNormalize(Vector2.UnitY) * 0.1f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Target.Center + spawnOffset, sliceVelocity, ModContent.ProjectileType<ExobeamSlash>(), Projectile.damage, 0f, Projectile.owner);
            }
        }

        public override bool? CanDamage() => false;
    }
}
