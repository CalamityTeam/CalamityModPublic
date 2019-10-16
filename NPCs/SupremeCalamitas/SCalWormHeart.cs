using CalamityMod.World;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class SCalWormHeart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Heart");
        }

        public override void SetDefaults()
        {
            npc.damage = 0; //70
            npc.width = 24; //324
            npc.height = 24; //216
            npc.defense = 0;
            npc.lifeMax = CalamityWorld.revenge ? 180000 : 160000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 240000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 120000 : 90000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<Celestus2>())
            {
                damage = (int)((double)damage * 0.66);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
