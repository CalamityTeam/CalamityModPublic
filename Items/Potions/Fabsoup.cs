using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class Fabsoup : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/SoupConsumption");
        public SlotId DrinkSoundSlot;

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 50;
            Item.value = 0;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.maxStack = 1;
            Item.consumable = false;
            Item.useAnimation = 901;
            Item.useTime = 901;
            Item.UseSound = null;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
        }
        public override bool? UseItem(Player player)
        {
            DrinkSoundSlot = SoundEngine.PlaySound(UseSound, player.Center);
            return true;
        }

        public override void UseItemFrame(Player player)
        {
            int time = CalamityUtils.SecondsToFrames(1980f); // 33 minutes
            if (player.itemAnimation == 180)
            {
                player.AddBuff(BuffID.WellFed3, time);
            }
            // ow hot
            if (player.itemAnimation == 60)
            {
                player.AddBuff(BuffID.WellFed3, time);
                player.AddBuff(BuffID.OnFire, time);
                player.AddBuff(BuffID.Frostburn, time);
                player.AddBuff(BuffID.CursedInferno, time);
                player.AddBuff(ModContent.BuffType<Shadowflame>(), time);
                player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), time);
                player.AddBuff(ModContent.BuffType<HolyFlames>(), time);
                player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), time);
                player.AddBuff(ModContent.BuffType<Dragonfire>(), time);
                player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), time);
            }

            if (SoundEngine.TryGetActiveSound(DrinkSoundSlot, out var drinkSound) && drinkSound.IsPlaying)
                drinkSound.Position = player.Center;
        }
    }
}
