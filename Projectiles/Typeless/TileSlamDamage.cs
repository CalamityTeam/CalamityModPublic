using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TileSlamDamage : DirectStrike, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetInstantKill();
            int damageLength = (int)Math.Pow(target.life.ToString().Length, target.boss ? 2.8f : 1);
            string sevens = "7";
            for (int i = 0; i < damageLength; i++)
            {
                sevens = sevens + "7";
            }
            
            Rectangle location = new Rectangle((int)target.position.X, (int)target.position.Y - 16, target.width, target.height);
            int text = CombatText.NewText(location, Color.DodgerBlue, Language.GetTextValue(sevens), true);
        }

    }
}
