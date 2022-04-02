using CalamityMod.Events;
using Microsoft.Xna.Framework;
using System.IO;
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
            npc.GetNPCDamage();
            npc.width = 100;
            npc.height = 100;
            npc.defense = 10;
            npc.DR_NERD(0.5f);
            npc.lifeMax = 650;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 1000;
            }
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = false;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            int num989 = (int)npc.ai[0];
            if (Main.npc[num989].active && Main.npc[num989].type == ModContent.NPCType<Siren>())
            {
                if (npc.alpha > 100 && npc.ai[1] == 0f)
                    npc.alpha -= 2;

                if (Main.npc[num989].damage == 0)
                    npc.ai[1] = 1f;
                else
                    npc.ai[1] = 0f;

                if (npc.ai[1] == 1f)
                    npc.alpha = Main.npc[num989].alpha;

                npc.dontTakeDamage = Main.npc[num989].damage == 0;
                npc.rotation = Main.npc[num989].rotation;
                npc.spriteDirection = Main.npc[num989].direction;
                npc.velocity = Vector2.Zero;
                npc.position = Main.npc[num989].Center;
                npc.position.X = npc.position.X - (npc.width / 2) + ((npc.spriteDirection == 1) ? -20f : 20f);
                npc.position.Y = npc.position.Y - (npc.height / 2) - 30;
                npc.gfxOffY = Main.npc[num989].gfxOffY;
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0f, 0.8f, 1.1f);
                return;
            }

            npc.dontTakeDamage = false;
            npc.life = 0;
            npc.HitEffect(0, 10.0);
            npc.active = false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => npc.ai[1] == 0f;

        public override Color? GetAlpha(Color drawColor)
        {
            if (npc.ai[1] == 1f)
                return Color.Transparent;

            return new Color(200, 200, 200, npc.alpha);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (npc.ai[1] == 1f)
                return;

            player.AddBuff(BuffID.Frostburn, 240, true);
        }

        public override bool PreNPCLoot() => false;

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
