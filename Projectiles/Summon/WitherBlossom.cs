using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class WitherBlossom : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float OffsetAngle => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Blossom");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.5f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<WitherBlossom>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<WitherBlossomsBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                    modPlayer.witherBlossom = false;
                if (modPlayer.witherBlossom)
                    Projectile.timeLeft = 2;
            }

            Initialize();

            Time++;
            NPC potentialTarget = Projectile.Center.MinionHoming(950f, Owner);
            if (Time % 50 == 49 && Main.myPlayer == Projectile.owner && potentialTarget != null)
            {
                Vector2 shootVelocity = Projectile.SafeDirectionTo(potentialTarget.Center).RotatedByRandom(0.25f) * 8f;
                int bolt = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<WitherBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[bolt].originalDamage = Projectile.originalDamage;
            }
            Projectile.Center = player.Center + OffsetAngle.ToRotationVector2() * 150f;
            Projectile.rotation += MathHelper.ToRadians(5f);
            OffsetAngle += MathHelper.ToRadians(4f);
        }

        public void Initialize()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 179);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 7f);
                }
                Projectile.localAI[0] += 1f;
            }
        }

        public override bool? CanDamage() => false;
    }
}
