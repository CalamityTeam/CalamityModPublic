using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Mushy : ModBuff
    {
        public static int DefenseBoost = 3;
        public static int RegenBoost = 2;
        public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBoost, RegenBoost.ToRegenPerSecond());

        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().mushy = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Player.Calamity().RandomDebuffVisualSpot, DustID.BlueFairy, Vector2.Zero, 100, default, 0.9f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity.Y -= 0.1f;
                dust.alpha = 200;
            }
            if (Main.rand.NextBool(15))
            {
                Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), 19), Main.rand.NextBool(3) ? 41 : 56, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, -2f)) + Player.velocity / 3, 0, default, 0.9f);
                dust2.alpha = 145;
            }
        }
    }
}
