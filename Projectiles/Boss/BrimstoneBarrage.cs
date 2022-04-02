using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBarrage : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Dart");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 690;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;

            if (projectile.velocity.Length() < (projectile.ai[1] == 0f ? (malice ? 17.5f : 14f) : (malice ? 12.5f : 10f)))
                projectile.velocity *= malice ? 1.0125f : 1.01f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            if (projectile.timeLeft < 60)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);

            if (projectile.ai[0] == 2f)
            {
                if (projectile.timeLeft > 570)
                {
                    int player = Player.FindClosest(projectile.Center, 1, 1);
                    Vector2 vector = Main.player[player].Center - projectile.Center;
                    float scaleFactor = projectile.velocity.Length();
                    vector.Normalize();
                    vector *= scaleFactor;
                    projectile.velocity = (projectile.velocity * 15f + vector) / 16f;
                    projectile.velocity.Normalize();
                    projectile.velocity *= scaleFactor;
                }
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;

                if (projectile.ai[0] == 0f)
                    projectile.damage = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) ? projectile.GetProjectileDamage(ModContent.NPCType<SupremeCalamitas>()) : projectile.GetProjectileDamage(ModContent.NPCType<CalamitasRun3>());
            }

            Lighting.AddLight(projectile.Center, 0.75f * projectile.Opacity, 0f, 0f);
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            if (projectile.ai[0] == 0f)
            {
                target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120);
            }
            else
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
