using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CobaltEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int index = 0; index < 2; ++index)
            {
                int ruby = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, Projectile.velocity.X, Projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            if (!hasHitEnemy && Projectile.timeLeft < 575)
            {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 288f, 12f, 20f);
            }
            else if (hasHitEnemy)
            {
                if (targetNPC >= 0)
                {
                    Vector2 newVelocity = Main.npc[targetNPC].Center - Projectile.Center;
                    newVelocity.Normalize();
                    newVelocity *= 15f;
                    Projectile.velocity = newVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float minDist = 999f;
            int index = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                bool hasHitNPC = false;
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == npc.whoAmI)
                    {
                        hasHitNPC = true;
                    }
                }

                if (npc == target)
                {
                    previousNPCs.Add(npc.whoAmI);
                }
                if (npc.CanBeChasedBy(Projectile, false) && npc != target && !hasHitNPC)
                {
                    float dist = (Projectile.Center - npc.Center).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = npc.whoAmI;
                    }
                }
            }

            Vector2 velocityNew;
            if (minDist < 999f)
            {
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - Projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                Projectile.velocity = velocityNew;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int ruby = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 50, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.scale *= 1.25f;
                dust.velocity *= 0.5f;
            }
        }
    }
}
