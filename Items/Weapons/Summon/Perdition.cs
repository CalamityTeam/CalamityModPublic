using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Perdition : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.damage = 375;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<PerditionBeacon>();
            Item.knockBack = 4f;

            Item.useTime = Item.useAnimation = 10; // 9 because of useStyle 1.
            Item.mana = 10;
            Item.width = Item.height = 56;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.ownedProjectileCounts[type] < 1)
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            else
            {
                // Play some demonic noises prior to a target being selected.
                SoundEngine.PlaySound(SoundID.Zombie93, player.Center);
                SoundEngine.PlaySound(SoundID.Item119, player.Center);
            }
            return false;
        }
    }
}
