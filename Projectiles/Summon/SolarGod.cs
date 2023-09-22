using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SolarGod : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 90;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<SolarGodSpiritBuff>(), 3600);
            bool correctMinion = Projectile.type == ModContent.ProjectileType<SolarGod>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.SPG = false;
                }
                if (modPlayer.SPG)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            float sizeScale = (float)Main.mouseTextColor / 200f - 0.35f;
            sizeScale *= 0.2f;
            Projectile.scale = sizeScale + 0.95f;
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int fire = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 244, 0f, 0f, 0, default, 1f);
                    Main.dust[fire].velocity *= 2f;
                    Main.dust[fire].scale *= 1.15f;
                }
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                Vector2 targetPos = Projectile.position;
                float maxDistance = 700f;
                bool foundTarget = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcDist = Vector2.Distance(Projectile.Center, npc.Center);
                        if (npcDist < maxDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            targetPos = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (!foundTarget)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float npcDist = Vector2.Distance(Projectile.Center, npc.Center);
                            if (npcDist < maxDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {
                                maxDistance = npcDist;
                                targetPos = npc.Center;
                                foundTarget = true;
                            }
                        }
                    }
                }
                if (foundTarget)
                {
                    float shootSpeed = 15f;
                    Vector2 source = Projectile.Center;
                    Vector2 velocity = targetPos - source;
                    velocity.Normalize();
                    velocity *= shootSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, ModContent.ProjectileType<SolarBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.ai[0] = 20f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool? CanDamage() => false;
    }
}
