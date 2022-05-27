using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoHydraBody : ModProjectile
    {
        public int TargetNPCIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const float DistanceToCheck = 2800f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra Body");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 86;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<EndoHydraBody>();
            player.AddBuff(ModContent.BuffType<EndoHydraBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.endoHydra = false;
                }
                if (modPlayer.endoHydra)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Vector2 returnLocation = player.Center;
            returnLocation.X -= (18 + player.width / 2) * player.direction;
            returnLocation.Y -= 25f;

            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget != null)
            {
                if (TargetNPCIndex != potentialTarget.whoAmI)
                {
                    TargetNPCIndex = potentialTarget.whoAmI;
                    SoundEngine.PlaySound(SoundID.Zombie53, Projectile.Center); // Ethereal whisper indicating a new target has been spotted.
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.frameCounter++ > (potentialTarget is null ? 8 : 6))
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, returnLocation, 0.25f);
            Projectile.direction = Projectile.spriteDirection = player.direction;
            Lighting.AddLight(Projectile.Center - Vector2.UnitY * 21f, 0.25f, 0.865f, 0.825f);
        }

        public override bool? CanDamage() => false;
    }
}
