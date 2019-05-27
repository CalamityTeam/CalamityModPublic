using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class CosmicPlushie : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Plushie");
			Tooltip.SetDefault("Summons the devourer of the cosmos...?\nSharp objects possibly included\nSuppresses friendly red devils");
		}
		public override void SetDefaults()
		{
            item.damage = 0;
			item.useStyle = 1;
			item.useAnimation = 20;
			item.useTime = 20;
			item.noMelee = true;
			item.width = 28;
            item.height = 36;
            item.value = Item.sellPrice(0, 7, 0, 0);
            item.shoot = mod.ProjectileType("ChibiiDoggo");
            item.buffType = mod.BuffType("ChibiiBuff");
			item.rare = 10;
			item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Meowmere, 5);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 15, true);
            }
        }
	}
}
