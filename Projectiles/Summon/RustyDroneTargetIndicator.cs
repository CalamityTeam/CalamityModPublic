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
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 50;
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
            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.projectile.Length || projectile.ai[1] < 0 || projectile.ai[1] >= Main.npc.Length)
            {
                projectile.Kill();
                return;
            }
            if (Main.projectile[(int)projectile.ai[0]].type != ModContent.ProjectileType<RustyDrone>() ||
                !Main.projectile[(int)projectile.ai[0]].active)
            {
                projectile.Kill();
            }
            if (!Main.npc[(int)projectile.ai[1]].active)
            {
                projectile.Kill();
            }
            projectile.Center = Vector2.Lerp(projectile.Center, Main.npc[(int)projectile.ai[1]].Center + DeltaPositionRelativetoTarget * Outwardness, 3f / TurnRate);
            if (projectile.localAI[0] == 0f)
            {
                DeltaPositionRelativetoTarget = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                projectile.localAI[0] = 1f;
            }
            projectile.localAI[1]++;
            if (projectile.localAI[1] % TurnRate == TurnRate - 1f)
            {
                Outwardness *= 0.6f;
                DeltaPositionRelativetoTarget = DeltaPositionRelativetoTarget.RotatedByRandom(MathHelper.PiOver4);
                projectile.scale = 1.5f;
                projectile.netUpdate = true;
            }
            if (projectile.scale > 1f && Outwardness > 4f)
            {
                projectile.scale = MathHelper.Lerp(projectile.scale, 1f, 0.025f);
            }
            else if (Outwardness <= 4f)
            {
                Outwardness *= 0.935f;
                if (projectile.timeLeft > 40)
                {
                    projectile.timeLeft = 40;
                    Main.npc[(int)projectile.ai[1]].AddBuff(BuffID.Poisoned, BuffTime);
                    Main.npc[(int)projectile.ai[1]].AddBuff(ModContent.BuffType<Irradiated>(), BuffTime);
                }
                projectile.alpha = (int)MathHelper.Lerp(projectile.alpha, 255f, 1f / 40f);
                projectile.scale += 0.05f;
            }
        }
    }
}
