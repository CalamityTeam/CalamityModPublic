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
        public override string Name => "Nebulous Core Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/NebulousCore";
        public override Color OutlineColor() => Color.Lerp(new Color(252, 109, 203), new Color(58, 91, 146), Completion);
        public override Color CooldownColor(float completionRatio) => Color.Lerp(new Color(255, 187, 207), new Color(148, 62, 216), completionRatio);

        public NebulousCoreCooldown(int duration)
        {
            Duration = duration;
            TimeLeft = duration;
        }
    }

    public class GlobalDodgeCooldown : CooldownIndicator
    {
        public override string Name => "Dodge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/GlobalDodge";
        public override Color CooldownColor(float completionRatio) => Color.Lerp(Color.LightCyan, Color.White, completionRatio);

        public GlobalDodgeCooldown(int duration)
        {
            Duration = duration;
            TimeLeft = duration;
        }
    }
}
