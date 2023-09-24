using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AdrenalineMode : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().adrenalineModeActive = true;
            player.Calamity().AdrenalineTrail = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            // 23SEP2023: Ozzatron: Adrenaline emits light directly. Color lifted from AdrenDust
            Vector3 adrenDustLight = new Vector3(0.094f, 0.255f, 0.185f);
            Lighting.AddLight(Player.Center, adrenDustLight * 3);

            for (int i = 0; i < 4; i++)
            {
                int dustID = ModContent.DustType<AdrenDust>();
                Vector2 dustVel = Player.velocity * 0.5f;
                int idx = Dust.NewDust(Main.rand.NextBool(5) ? drawInfo.Position : drawInfo.Position - Player.velocity * 1.5f, Player.width, Player.height, dustID, dustVel.X, dustVel.Y);
                Dust d = Main.dust[idx];
                d.scale = Main.rand.NextFloat(0.4f, 1.2f);
                d.noGravity = true;
                d.noLight = false;
                drawInfo.DustCache.Add(idx);
            }
        }
    }
}
