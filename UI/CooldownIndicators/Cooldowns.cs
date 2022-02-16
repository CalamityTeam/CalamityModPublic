using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.UI.CooldownIndicators
{
    public class NebulousCoreCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true; 
        public override string Name => "Nebulous Core Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/NebulousCore";
        public override Color OutlineColor => Color.Lerp(new Color(252, 109, 203), new Color(58, 91, 146), Completion);
        public override Color CooldownColorStart => new Color(148, 62, 216);
        public override Color CooldownColorEnd => new Color(255, 187, 207);

        public NebulousCoreCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class GlobalDodgeCooldown : CooldownIndicator
    {
        public override string SyncID => "GlobalDodge";
        public override bool DisplayMe => true;
        public override string Name => "Dodge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/GlobalDodge";
        public override Color CooldownColorStart => Color.CornflowerBlue;


        public GlobalDodgeCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class IceShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "SirenHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Ice Shield Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SirenIceShield";
        public override Color OutlineColor => Color.Lerp(new Color(163, 186, 198), new Color(146, 187, 255), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(124, 195, 214);
        public override Color CooldownColorEnd => new Color(147, 230, 253);


        public IceShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}
