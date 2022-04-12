using CalamityMod.NPCs;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Shred : ModBuff
    {
        internal const int StackFalloffFrames = 320;

        // 75 DPS (15x5) per stack may seem low, but it gets boosted by ranged stats and can supercrit.
        internal static int BaseDamage = 15;
        internal static int FramesPerDamageTick = 12;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shred");
            Description.SetDefault("Someone built ten Tri-Tip Daggers...");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            CalamityGlobalNPC cgn = npc.Calamity();
            if (cgn.somaShredStacks <= 0)
            {
                cgn.somaShredStacks = 1;
                cgn.somaShredFalloff = StackFalloffFrames;
            }
            else
            {
                ++cgn.somaShredStacks;
            }

            // As soon as the debuff is applied to the NPC, subsume it into Calamity's tracking system
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void TickDebuff(NPC target, CalamityGlobalNPC cgn)
        {
            // Decrement the falloff counter every frame.
            cgn.somaShredFalloff -= cgn.somaShredStacks;
            if (cgn.somaShredFalloff <= 0)
            {
                cgn.somaShredFalloff += StackFalloffFrames;
                --cgn.somaShredStacks;
            }

            // If a valid player applied this debuff, then let it deal damage.
            if (cgn.somaShredApplicator >= 0 && cgn.somaShredApplicator < Main.maxPlayers && Main.myPlayer == cgn.somaShredApplicator)
            {
                Player applicator = Main.player[cgn.somaShredApplicator];

                // Only deal damage once every several frames.
                if (applicator.miscCounter % FramesPerDamageTick == 0)
                {
                    int dmg = (int)(BaseDamage * cgn.somaShredStacks * applicator.GetDamage(DamageClass.Ranged).Base);
                    Projectile tick = Projectile.NewProjectileDirect(target.GetSpawnSource_ForProjectile(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), dmg, 0f, applicator.whoAmI, target.whoAmI);
                    tick.DamageType = DamageClass.Ranged; // Uncommon for DirectStrikes, but it needs to be able to crit.
                    tick.Calamity().canSupercrit = true;
                }
            }
        }
    }
}
