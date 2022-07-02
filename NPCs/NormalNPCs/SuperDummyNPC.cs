using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using ReLogic.Utilities;
using Terraria;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SuperDummyNPC : ModNPC
    {
        public static readonly SoundStyle HummSound = new("CalamityMod/Sounds/Item/WulfrumProthesisSucc") { IsLooped = true };
        private SlotId IdleSoundSlot;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Super Dummy");
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 9999999;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = 0;
        }

        public override void AI()
        {
        }

        public override void OnKill()
        {
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            NPC.lifeRegen += 2000000;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckDead()
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.life = NPC.lifeMax;
                return false;
            }
            return true;
        }
    }
}
