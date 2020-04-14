using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class IbarakiBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hermit's Box of One Hundred Medicines");
            Tooltip.SetDefault("As the ice melts in the springs\n" +
                "And waves wash the old moss’ hair...\n" +
                "Thank you, Goodbye.\n" +
                "Summons the Third Sage\n" +
                "Use the item with right click to permanently gain the Third Sage's blessing.\n" +
				"With the blessing, the player will spawn with full health rather than half.");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.noMelee = true;
            item.width = 36;
            item.height = 30;
            item.UseSound = SoundID.Item3;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.shoot = ModContent.ProjectileType<ThirdSage>();
            item.buffType = ModContent.BuffType<ThirdSageBuff>();
            item.rare = 5;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (!player.Calamity().healToFull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
			else
			{
				return true;
			}
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyle(Player player)
        {
            if (player.altFunctionUse == 2)
			{
				if (!player.Calamity().healToFull)
				{
					player.Calamity().healToFull = true;
					string key = "Mods.CalamityMod.ThirdSageBlessingText";
					Color messageColor = Color.Violet;
					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					else if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
				}
			}
			else
			{
				if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				{
					player.AddBuff(item.buffType, 3600, true);
				}
			}
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			return false;
		}
    }
}
