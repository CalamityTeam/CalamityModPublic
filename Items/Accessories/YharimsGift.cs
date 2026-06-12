using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class YharimsGift : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int MeteorDamage => CalamityUtils.ScaleWithDifficulty(250);

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.defense = 10;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().yharimsGift = true;
            var source = player.GetSource_Accessory(Item);
            player.moveSpeed += 0.2f;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.noKnockback = true;
            if (player.HasIFrames())
            {
                if (player.miscCounter % 8 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int damage = (int)player.GetBestClassDamage().ApplyTo(MeteorDamage);

                        CalamityUtils.ProjectileRain(source, player.Center, 400f, 400f, 500f, 800f, 22f, ModContent.ProjectileType<SkyFlareFriendly>(), damage, 9f, player.whoAmI).Calamity().conditionalHomingRange = 160;
                    }
                }
            }
        }
    }
}
