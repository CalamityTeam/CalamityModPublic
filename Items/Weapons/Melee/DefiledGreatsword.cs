using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TrueTyrantYharimsUltisword")]
    public class DefiledGreatsword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 102;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                    type = ModContent.ProjectileType<BlazingPhantomBlade>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<HyperBlade>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<SunlightBlade>();
                    break;
                default:
                    break;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, 1f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedCleaver>().
                AddIngredient<CoreofCalamity>().
                AddIngredient<UelibloomBar>(15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
