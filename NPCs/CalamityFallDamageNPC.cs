using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.Walls.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.VanillaNPCOverrides.Bosses;
using CalamityMod.NPCs.VanillaNPCOverrides.RegularEnemies;
using CalamityMod.Balancing;
using Terraria.GameContent;
using CalamityMod.Systems;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs
{
    public class CalamityFallDamageNPC : GlobalNPC
    {
        /// <summary>
        /// How much "gravity damage" is this npc slated to recieve once it hits the ground.
        /// </summary>
        private int potentialEnergyDamage = 0;
        /// <summary>
        /// This keeps track of the last recorded velocity, so that we can dispel the potential fall damage if the enemy slows their fall down by any means
        /// </summary>
        private Vector2 OldVelocity = Vector2.Zero;
        /// <summary>
        /// This keeps track of the last recorded velocity, because terraria is dumb and we need two of them
        /// </summary>
        private Vector2 OlderVelocity = Vector2.Zero;
        private float terminalVelocityForFullFallDamage = 0;

        public int PotentialEnergyDamage
        {
            get
            {
                return potentialEnergyDamage;
            }

            private set
            {
                //You can't stack gravity damage. Higher "gravity" simply replaces the gravity damage.
                potentialEnergyDamage = value == 0 ? 0 : Math.Max(potentialEnergyDamage, value);
            }
        }

        public bool FallDamageSusceptible => PotentialEnergyDamage > 0;

        public override bool InstancePerEntity => true;

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityFallDamageNPC myClone = (CalamityFallDamageNPC)base.Clone(npc, npcClone);

            myClone.potentialEnergyDamage = potentialEnergyDamage;
            myClone.OldVelocity = OldVelocity;
            myClone.OlderVelocity = OlderVelocity;
            myClone.terminalVelocityForFullFallDamage = terminalVelocityForFullFallDamage;

            return myClone;
        }

        public override void SetDefaults(NPC npc)
        {
            PotentialEnergyDamage = 0;
            OldVelocity = Vector2.Zero;
            OlderVelocity = Vector2.Zero;
        }

        /// <summary>
        /// Sets up a NPC to recieve fall damage when they hit the ground.
        /// </summary>
        /// <param name="npc">The npc to apply fall damage to</param>
        /// <param name="potentialDamage">The maximum fall damage taken by the npc once they hit the ground</param>
        /// <param name="terminalVelocityForFullDamage">The downwards velocity necessary to recieve the full fall damage. By default 10, the npc max fall speed</param>
        public void ApplyFallDamage(NPC npc, int potentialDamage, float terminalVelocityForFullDamage = 10f)
        {
            //NPCs that don't collide with tiles simply don't get fall damage, lol.
            if (npc.noTileCollide)
                return;

            PotentialEnergyDamage = potentialDamage;
            OldVelocity = npc.velocity;
            OlderVelocity = npc.velocity;
            terminalVelocityForFullFallDamage = terminalVelocityForFullDamage;
        }

        public override void PostAI(NPC npc)
        {
            if (FallDamageSusceptible)
            {
                float newVerticalVelocity = npc.velocity.Y;
                float oldVerticalVelocity = OldVelocity.Y;
                float olderVerticalVelocity = OlderVelocity.Y;

                //If the thing flies back up, cancel the fall dmg
                if (newVerticalVelocity < 0 && oldVerticalVelocity >= 0)
                    PotentialEnergyDamage = 0;

                //Same goes for if it slows its fall. 
                //We use 3 variables cuz there is a frame inbetween the fall and the landing where the velocity is still set to something.
                if (newVerticalVelocity > 0 && olderVerticalVelocity > 0 && oldVerticalVelocity < olderVerticalVelocity)
                    PotentialEnergyDamage = 0;

                //If the npc hit a tile/Came to a stop after falling
                if (newVerticalVelocity == 0 && oldVerticalVelocity > 0)
                {
                    //I don't think a tile check below the npc is necessary but if we find weird edge cases ill add it
                    npc.StrikeNPC((int)(PotentialEnergyDamage * Math.Clamp(olderVerticalVelocity / terminalVelocityForFullFallDamage, 0f, 1f)), 0f, 1, fromNet: true);
                    PotentialEnergyDamage = 0;
                }

                OlderVelocity = OldVelocity;
                OldVelocity = npc.velocity;
            }
        }
    }
}
