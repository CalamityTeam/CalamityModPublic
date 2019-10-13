using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AbyssNPCs
{
    public class AquaticScourgeTail : ModNPC
    {
        public bool detectsPlayer = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
            npc.damage = 45;
            npc.width = 30;
            npc.height = 50;
            npc.defense = 25;
            npc.Calamity().RevPlusDR(0.15f);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
			npc.LifeMaxNERD(73000, 85000, 100000, 20000000, 21000000);
			double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.chaseable = false;
            npc.canGhostHeal = false;
            if (Main.expertMode)
            {
                npc.scale = 1.15f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(detectsPlayer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            detectsPlayer = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if ((double)Main.npc[(int)npc.ai[1]].life <= (double)Main.npc[(int)npc.ai[1]].lifeMax * 0.99)
            {
                detectsPlayer = true;
                npc.damage = npc.defDamage;
                npc.boss = true;
                Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AquaticScourge");
                else
                    music = MusicID.Boss2;
            }
            else
            {
                npc.damage = 0;
            }
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            bool flag = false;
            if (npc.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }
            if (!NPC.AnyNPCs(mod.NPCType("AquaticScourgeHead")))
            {
                npc.active = false;
            }
            if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = npc.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;
                if (num191 < 0f)
                {
                    npc.spriteDirection = -1;
                }
                else if (num191 > 0f)
                {
                    npc.spriteDirection = 1;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion)
                damage = (int)((double)damage * 0.5);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASTail"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASTail2"), 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 120, true);
            player.AddBuff(BuffID.Venom, 120, true);
        }
    }
}
