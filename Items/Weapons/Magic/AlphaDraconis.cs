using System;
using System.Linq;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("ClamorNoctus")]
    public class AlphaDraconis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 74;
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.useAnimation = Item.useTime = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.2f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AlphaDraconisStar>();
            Item.shootSpeed = 8f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.Calamity().StratusStarburst >= 5 || player.ownedProjectileCounts[ModContent.ProjectileType<DracoConstellation>()] > 0;
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().StratusStarburstResetTimer = (int)MathHelper.Max(player.Calamity().StratusStarburstResetTimer, 600);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DracoConstellation>()] > 0)
                {
                    var p = Main.projectile.First(x => x.type == ModContent.ProjectileType<DracoConstellation>() && x.active && x.owner == player.whoAmI);
                    p.timeLeft = (int)MathHelper.Min(p.timeLeft, 60);
                } else
                    Projectile.NewProjectile(source, player.Center + new Vector2(-125,-100), -Vector2.Zero, ModContent.ProjectileType<DracoConstellation>(), (int)(damage * 4f), knockback, player.whoAmI);
                return false;
            }
            var mousePos = player.Calamity().mouseWorld;
            var dir = player.DirectionTo(mousePos);
            for (var i = 0; i < 3; i++)
                Projectile.NewProjectile(source, new Vector2(mousePos.X,player.Center.Y) + new Vector2(Main.rand.Next(500, 1300) * -player.direction, Main.rand.Next(-800,-600)), Vector2.UnitX * player.direction * velocity.Length(), type, damage, knockback, player.whoAmI, mousePos.X, mousePos.Y);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WyvernsCall>().
                AddIngredient<Lumenyl>(6).
                AddIngredient<RuinousSoul>(5).
                AddIngredient<ExodiumCluster>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
