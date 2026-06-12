using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FleshTotem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int manaStorageMax = 600;
        public const int lostSoulDamage = 200;
        public static int MinDelay => 60;
        public static int MaxDelay => 240;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fleshTotem = true;
            modPlayer.fleshTotemMinion = true;
            modPlayer.fleshTotemVisual = !hideVisual;
            player.statManaMax2 += 20;
            player.GetCritChance<MagicDamageClass>() += 5;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<FleshTotemMinion>()] < 1)
                {
                    int damage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(lostSoulDamage);

                    int effigy = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<FleshTotemMinion>(), damage, 2f, Main.myPlayer);
                    if (Main.projectile.IndexInRange(effigy))
                        Main.projectile[effigy].originalDamage = lostSoulDamage;
                }
            }
        }
    }
}
