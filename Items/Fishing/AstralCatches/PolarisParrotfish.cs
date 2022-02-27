using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class PolarisParrotfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polaris Parrotfish");
            Tooltip.SetDefault("It carries the mark of the Northern Star\n" +
                "Projectile hits grant buffs to the weapon and the player\n" +
                "Buffs are removed on hit");
            Item.staff[item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.ranged = true;
            item.width = 38;
            item.height = 34;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"); //pew pew
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PolarStar>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.polarisBoostThree) //Homes in and explodes
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PolarStar>(), damage, knockBack, player.whoAmI, 0f, 2f);
                return false;
            }
            else if (modPlayer.polarisBoostTwo) //Splits on enemy or tile hits
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PolarStar>(), (int)(damage * 1.25), knockBack, player.whoAmI, 0f, 1f);
                return false;
            }
            return true;
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }
    }
}
