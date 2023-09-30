using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HomingGammaBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override string Texture => "CalamityMod/Projectiles/Enemy/NuclearBulletMedium";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.alpha = Utils.Clamp(Projectile.alpha - 35, 0, 255);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (targetNPC == -1)
                FindTarget();

            NPC potentialTarget = null;
            if (targetNPC != -1)
                potentialTarget = Main.npc[targetNPC];
            if (potentialTarget != null && !Projectile.WithinRange(potentialTarget.Center, 100f) && Projectile.timeLeft < 320)
                Projectile.velocity = (Projectile.velocity * 6f + Projectile.SafeDirectionTo(potentialTarget.Center) * 15f) / 7f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            previousNPCs.Add(target.whoAmI);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);
            targetNPC = -1;
            FindTarget();
        }

        private void FindTarget()
        {
            Player player = Main.player[Projectile.owner];
            float range = 1000f;
            bool foundTarget = false;
            Vector2 center = player.Center;
            Vector2 half = new Vector2(0.5f);
            half.Y = 0f;
            bool hasHitNPC = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == player.MinionAttackTargetNPC)
                    {
                        hasHitNPC = true;
                    }
                }
                if (npc.CanBeChasedBy(Projectile, false) && !hasHitNPC)
                {
                    Vector2 npcPos = npc.position + npc.Size * half;
                    float npcDist = Vector2.Distance(npcPos, center);
                    if (npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        foundTarget = true;
                        targetNPC = npc.whoAmI;
                    }
                }
            }
			hasHitNPC = false;
            if (!foundTarget)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    for (int j = 0; j < previousNPCs.Count; j++)
                    {
                        if (previousNPCs[j] == k)
                        {
                            hasHitNPC = true;
                        }
                    }
                    if (npc.CanBeChasedBy(Projectile, false) && !hasHitNPC)
                    {
                        Vector2 npcPos = npc.position + npc.Size * half;
                        float npcDist = Vector2.Distance(npcPos, center);
                        if (npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = npcDist;
                            targetNPC = k;
                        }
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 3; i++)
            {
                Dust sulphuricAcid = Dust.NewDustDirect(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                sulphuricAcid.noGravity = true;
                sulphuricAcid.velocity *= 1.8f;

                sulphuricAcid = Dust.CloneDust(sulphuricAcid);
                sulphuricAcid.velocity *= -1f;
            }
        }
    }
}
