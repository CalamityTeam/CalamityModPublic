using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheLastMourning : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Daybroken>(), BuffID.OnFire3];
        }
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 94;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 600;
            Item.knockBack = 8.5f;
            Item.useAnimation = Item.useTime = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != ModContent.NPCType<DevourerofGodsBody>() || Main.rand.NextBool(3))
            {
                CalamityPlayer.HorsemansBladeOnHit(player, target.whoAmI, Item.damage, Item.knockBack, 0, ModContent.ProjectileType<MourningSkull>());
                CalamityPlayer.HorsemansBladeOnHit(player, target.whoAmI, Item.damage, Item.knockBack, 1);
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            CalamityPlayer.HorsemansBladeOnHit(player, -1, Item.damage, Item.knockBack, 0, ModContent.ProjectileType<MourningSkull>());
            CalamityPlayer.HorsemansBladeOnHit(player, -1, Item.damage, Item.knockBack, 1);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dustType = 5;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        dustType = 5;
                        break;
                    case 1:
                        dustType = 6;
                        break;
                    case 2:
                        dustType = 174;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, dustType, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheHorsemansBlade).
                AddIngredient(ItemID.SoulofNight, 30).
                AddIngredient<ReaperTooth>(5).
                AddIngredient<RuinousSoul>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
