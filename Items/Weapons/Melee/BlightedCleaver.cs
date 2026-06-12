using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TyrantYharimsUltisword")]
    public class BlightedCleaver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Venom];
        }

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootsEveryUse = true;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, velocity, type, (int)(damage * 0.75), knockback * 0.5f, player.whoAmI, (float)player.direction * player.gravDir, 32f, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dustType = DustID.Venom;
                switch (Main.rand.Next(5))
                {
                    default:
                    case 0:
                    case 1:
                        break;

                    case 2:
                        dustType = DustID.RedTorch;
                        break;

                    case 3:
                        dustType = DustID.CrimsonTorch;
                        break;

                    case 4:
                        dustType = DustID.GreenFairy;
                        break;
                }
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(1.8f, 2.4f));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0f;
                if (dustType == DustID.Venom)
                    Main.dust[dust].fadeIn = 1.5f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.OnFire3, 240);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.OnFire3, 240);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueCausticEdge>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient(ItemID.ChlorophyteBar, 15).
                AddIngredient(ItemID.VialofVenom, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
