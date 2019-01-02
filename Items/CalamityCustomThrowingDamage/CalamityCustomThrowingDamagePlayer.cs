using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class CalamityCustomThrowingDamagePlayer : ModPlayer
	{
        public static CalamityCustomThrowingDamagePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<CalamityCustomThrowingDamagePlayer>();
        }

        public float throwingDamage = 1f;
        public float throwingVelocity = 1f;
        public int throwingCrit = 4;
        public bool throwingAmmoCost66 = false;
        public bool throwingAmmoCost50 = false;

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        private void ResetVariables()
        {
            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 4;
            throwingAmmoCost66 = false;
            throwingAmmoCost50 = false;
        }
    }
}