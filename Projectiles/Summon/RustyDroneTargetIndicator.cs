using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class RustyDroneTargetIndicator : ModProjectile
    {
        public float Outwardness = 45f;
        public Vector2 DeltaPositionRelativetoTarget = Vector2.Zero;

        public const int BuffTime = 120;
        public const float TurnRate = 30f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Target Indicator");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 50;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Outwardness);
            writer.WriteVector2(DeltaPositionRelativetoTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Outwardness = reader.ReadSingle();
            DeltaPositionRelativetoTarget = reader.ReadVector2();
        }
        public override void AI()
        {
            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.projectile.Length || Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.npc.Length)
            {
                Projectile.Kill();
                return;
            }
            if (Main.projectile[(int)Projectile.ai[0]].type != ModContent.ProjectileType<RustyDrone>() ||
                !Main.projectile[(int)Projectile.ai[0]].active)
            {
                Projectile.Kill();
            }
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }
            Projectile.Center = Vector2.Lerp(Projectile.Center, Main.npc[(int)Projectile.ai[1]].Center + DeltaPositionRelativetoTarget * Outwardness, 3f / TurnRate);
            if (Projectile.localAI[0] == 0f)
            {
                DeltaPositionRelativetoTarget = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Projectile.localAI[0] = 1f;
            }
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] % TurnRate == TurnRate - 1f)
            {
                Outwardness *= 0.6f;
                DeltaPositionRelativetoTarget = DeltaPositionRelativetoTarget.RotatedByRandom(MathHelper.PiOver4);
                Projectile.scale = 1.5f;
                Projectile.netUpdate = true;
            }
            if (Projectile.scale > 1f && Outwardness > 4f)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.025f);
            }
            else if (Outwardness <= 4f)
            {
                Outwardness *= 0.935f;
                if (Projectile.timeLeft > 40)
                {
                    Projectile.timeLeft = 40;
                    Main.npc[(int)Projectile.ai[1]].AddBuff(BuffID.Poisoned, BuffTime);
                    Main.npc[(int)Projectile.ai[1]].AddBuff(ModContent.BuffType<Irradiated>(), BuffTime);
                }
                Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 255f, 1f / 40f);
                Projectile.scale += 0.05f;
            }
        }
    }
}
