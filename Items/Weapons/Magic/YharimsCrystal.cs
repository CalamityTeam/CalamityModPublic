using System.Collections.Generic;
using System.Linq;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class YharimsCrystal : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 16;
            Item.height = 16;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<YharimsCrystalPrism>();
            Item.shootSpeed = 30f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void UpdateInventory(Player player)
        {
            if (Main.zenithWorld)
                Item.SetNameOverride(this.GetLocalizedValue("GFBName"));
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.zenithWorld)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileID.Dynamite, 250, 0, player.whoAmI);
                return false;
            }
            return true;
        }
    }
}
