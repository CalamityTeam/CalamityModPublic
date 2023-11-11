using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NullShot2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                for (int num134 = 0; num134 < 10; num134++)
                {
                    float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)num134;
                    float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)num134;
                    int dust = Dust.NewDust(new Vector2(x, y), 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[dust].alpha = Projectile.alpha;
                    Main.dust[dust].position.X = x;
                    Main.dust[dust].position.Y = y;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            int nullBuff = Main.rand.Next(10);
            if (!target.boss)
            {
                if (nullBuff == 0)
                {
                    target.scale *= 5f;
                }
                else if (nullBuff == 1)
                {
                    target.scale *= 0.1f;
                }
                else if (nullBuff == 2 && target.type != ModContent.NPCType<SuperDummyNPC>())
                {
                    target.damage += 20;
                }
                else if (nullBuff == 3)
                {
                    target.damage -= 20;
                }
                else if (nullBuff == 4)
                {
                    target.knockBackResist = 0f;
                }
                else if (nullBuff == 5)
                {
                    target.knockBackResist = 1f;
                }
                else if (nullBuff == 6)
                {
                    target.defense += 10;
                }
                else if (nullBuff == 7)
                {
                    target.defense -= 10;
                }
                else if (nullBuff == 8)
                {
                    target.velocity.Y = Main.rand.NextBool() ? 30f : -30f;
                }
                else
                {
                    target.velocity.X = Main.rand.NextBool() ? 30f : -30f;
                }
            }
        }
    }
}
