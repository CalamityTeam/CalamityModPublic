using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using Terraria;
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
    }
}
