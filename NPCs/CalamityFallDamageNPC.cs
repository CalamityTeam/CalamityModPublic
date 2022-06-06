using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

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
