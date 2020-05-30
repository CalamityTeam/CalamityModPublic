using CalamityMod.Projectiles.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Leviathan
{
    public class SirenIce : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Shield");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.damage = 55;
            npc.width = 100;
            npc.height = 100;
            npc.defense = 10;
            npc.Calamity().RevPlusDR(0.5f);
            npc.lifeMax = 650;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 10000;
            }
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
        }

        public override void AI()
        {
            if (npc.alpha > 100)
            {
                npc.alpha -= 2;
            }
            Player player = Main.player[npc.target];
            int num989 = (int)npc.ai[0];
            if (Main.npc[num989].active && Main.npc[num989].type == ModContent.NPCType<Siren>())
            {
                npc.rotation = Main.npc[num989].rotation;
                npc.spriteDirection = Main.npc[num989].direction;
                npc.velocity = Vector2.Zero;
                npc.position = Main.npc[num989].Center;
                npc.position.X = npc.position.X - (float)(npc.width / 2) + ((npc.spriteDirection == 1) ? -20f : 20f);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2) - 30;
                npc.gfxOffY = Main.npc[num989].gfxOffY;
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0f, 0.8f, 1.1f);
                return;
            }
            npc.life = 0;
            npc.HitEffect(0, 10.0);
            npc.active = false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CalamityMod.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
            {
                if (projectile.penetrate == -1 && !projectile.minion)
                {
                    projectile.penetrate = 1;
                }
                else if (projectile.penetrate >= 1)
                {
                    projectile.penetrate = 1;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, npc.alpha);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 240, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
