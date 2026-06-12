using CalamityMod.DataStructures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Daybroken : ModBuff
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("BuffName.Daybreak");
        public override LocalizedText Description => Language.GetOrRegister("BuffDescription.Daybreak");
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = DebuffData.Daybroken;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().daybroken = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.Next(4) < 3)
            {
                Dust solarDust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.OrangeTorch, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                solarDust.noGravity = true;
                solarDust.velocity *= 2.8f;
                solarDust.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                {
                    solarDust.noGravity = false;
                    solarDust.scale *= 0.5f;
                }
            }

            Lighting.AddLight((int)(Player.position.X / 16f), (int)(Player.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
        }
    }
}
