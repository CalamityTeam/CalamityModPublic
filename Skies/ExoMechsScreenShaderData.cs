using System;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Skies
{
    public class ExoMechsScreenShaderData : ScreenShaderData
    {
        public ExoMechsScreenShaderData(string passName)
            : base(passName)
        {
        }

        public override void Apply()
        {
            if (CalamityGlobalNPC.draedon != -1 && Draedon.ExoMechIsPresent)
                UseTargetPosition(Main.npc[CalamityGlobalNPC.draedon].Center);

            base.Apply();
        }

        public override void Update(GameTime gameTime)
        {
            if (!ExoMechsSky.CanSkyBeActive)
                Filters.Scene["CalamityMod:ExoMechs"].Deactivate(Array.Empty<object>());
        }
    }
}
