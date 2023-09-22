using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MountedScannerSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public float AngularOffsetRelativeToPlayer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public const int LaserFireRate = 300;

        public const float OffsetDistanceFromPlayer = 60f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 18;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center + AngularOffsetRelativeToPlayer.ToRotationVector2() * OffsetDistanceFromPlayer;
            GrantBuffs(player);

            NPC potentialTarget = Projectile.Center.MinionHoming(960f, player);
            if (potentialTarget is null)
            {
                AdjustVisualValues_Idle(player);
                Projectile.localAI[0] = 0f;
            }
            else
            {
                AttackTarget(potentialTarget);
                Projectile.localAI[0] = 1f;
            }
            Time++;
        }
        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<MountedScannerSummon>();
            player.AddBuff(ModContent.BuffType<MountedScannerBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                    player.Calamity().mountedScanner = false;
                if (player.Calamity().mountedScanner)
                    Projectile.timeLeft = 2;
            }
        }
        public void AdjustVisualValues_Idle(Player player)
        {
            if (player.velocity.Length() > 1.5f)
            {
                Projectile.spriteDirection = (player.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = player.velocity.ToRotation() + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.075f);
            }
        }
        public void AttackTarget(NPC target)
        {
            Projectile.spriteDirection = 1;
            Projectile.rotation = Projectile.AngleTo(target.Center);
            if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                return;

            if (Time % LaserFireRate == LaserFireRate - 1f)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                         Projectile.SafeDirectionTo(target.Center, Vector2.UnitY),
                                         ModContent.ProjectileType<MountedScannerLaser>(),
                                         Projectile.damage,
                                         Projectile.knockBack,
                                         Projectile.owner,
                                         0f,
                                         Projectile.whoAmI);
                }
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Projectile.Center);
            }
        }
        public override bool? CanDamage() => false;
    }
}
